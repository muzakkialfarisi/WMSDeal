using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Models.ViewModels.ApiViewModel.Maui;
using WMS.Utility;

namespace WMS.Web.Controllers.Maui.OutGoing
{
    [Route("maui/Outgoing/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class PickController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ErrorResponseViewModel _error = new ErrorResponseViewModel();

        public PickController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        [HttpGet("SalesOrder")]
        public async Task<IActionResult> Get(string DateOrdered = null, int? FlagPick = null, Guid? TenantId = null, string Status = null, string? PeriodeOrder = null)
        {
            try
            {
                var model = await _unitOfWork.SalesOrder.GetAllAsync(
                    includeProperties:
                        m => m.Include(m => m.OutSalesOrderConsignee)
                              .Include(m => m.MasSalesType)
                              .Include(m => m.OutSalesOrderProducts)
                              .Include(m => m.MasDataTenant),
                    filter:
                        m => m.HouseCode==User.FindFirst("HouseCode").Value);


                //model = model.Where(m => m.HouseCode == User.FindFirst("HouseCode").Value).ToList();

                if (DateOrdered != null)
                {
                    model = model.Where(m => m.DateOrdered.ToString("yyyy-MM-dd") == DateOrdered).ToList();
                }
                if (PeriodeOrder!= null)
                {
                    model=model.Where(m => m.DateOrdered.ToString("MMyyyy")==PeriodeOrder).ToList();
                }
                if (FlagPick != null)
                {
                    model = model.Where(m => m.FlagPick == FlagPick).ToList();
                }
                if (TenantId != null)
                {
                    model = model.Where(m => m.TenantId == TenantId).ToList();
                }
                if (Status != null)
                {
                    char[] delimiterChars = { ',' };
                    string[] words = Status.Split(delimiterChars);

                    var tempStatus = new List<OutSalesOrder>();
                    for (int i = 0; i < words.Length; i++)
                    {
                        tempStatus.AddRange(model.Where(m => m.Status == short.Parse(words[i])));
                    }

                    model = tempStatus;
                }

                var MOD = model.Select(x => new
                {
                    x.OrderId,
                    SalesType = x.MasSalesType.StyName,
                    x.StoreName,
                    Tenant = x.MasDataTenant.Name,
                    ImageTenant = x.MasDataTenant.ProfileImageUrl,
                    x.OutSalesOrderConsignee.CneeName,
                    x.OutSalesOrderConsignee.CneeCity,
                    x.DateOrdered,
                    x.Status,
                    x.HouseCode,
                    Total = x.OutSalesOrderProducts.Count,
                }).ToList();

                return Ok(MOD);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "IC0001";
                return BadRequest(_error);
            }
        }

        [HttpGet("PickAssign")]
        public async Task<IActionResult> GetAssign(Guid UserId, int Id)
        {
            try
            {
                var model = await _unitOfWork.SalesOrderStorage.GetSingleOrDefaultAsync(
                                filter:
                                    m => m.OutSalesOrderProduct.OutSalesOrder.OutSalesOrderAssign.UserId == UserId &&
                                    m.Id == Id,
                                includeProperties:
                                    m => m.Include(m => m.OutSalesOrderProduct.OutSalesOrder.OutSalesOrderAssign));

                if (model != null)
                {
                    if (model.OutSalesOrderProduct.OutSalesOrder.OutSalesOrderAssign != null)
                    {
                        return Ok(model.OutSalesOrderProduct.OutSalesOrder.OutSalesOrderAssign);
                    }
                }

                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = "Notfound";
                _error.Code = "PO0001";
                return BadRequest(_error);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "PO0001";
                return BadRequest(_error);
            }
        }

        [HttpPost("Assign")]
        public async Task<IActionResult> Post(string HouseCode, [FromBody] List<AssignSalesOrderViewModel> models)
        {
            try
            {
                if (models.Count != 1)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Pilih Minimum 1 Sales Order!";
                    _error.Code = "PA0010";
                    return BadRequest(_error);
                }

                if (HouseCode != User.FindFirst("HouseCode")?.Value)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Invalid HouseCode!";
                    _error.Code = "PA0010";
                    return BadRequest(_error);
                }

                Guid UserId = new Guid(User.FindFirst("UserId")?.Value);

                var assigns = await _unitOfWork.SalesOrderAssign.GetAllAsync(
                    filter:
                        m => m.UserId == UserId &&
                        m.Flag == 1);

                if (assigns.Count > 0)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Masih ada list pick!";
                    _error.Code = "PA0010";
                    return BadRequest(_error);
                }

                var Build = await RoutePickGenerator(models);
                if (Build != "Success")
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = Build;
                    _error.Code = "PA0010";
                    return BadRequest(_error);
                }

                Guid PickAssignId = Guid.NewGuid();

                var newmodels = new List<OutSalesOrder>();

                for (int i = 0; i < models.Count; i++)
                {
                    newmodels.Add(await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(m => m.OrderId == models[i].OrderId));
                    if (newmodels[i].FlagPick != 0)
                    {
                        _error.StatusCode = "400";
                        _error.Error = "Error";
                        _error.Message = newmodels[i].OrderId + " sudah di pick!";
                        _error.Code = "PA0010";
                        return BadRequest(_error);
                    }

                    var outSalesOrderAssign = new OutSalesOrderAssign()
                    {
                        OrderId = models[i].OrderId,
                        PickAssignId = PickAssignId,
                        UserId = UserId,
                        DateAssigned = DateTime.Now,
                        Flag = 1
                    };
                    await _unitOfWork.SalesOrderAssign.AddAsync(outSalesOrderAssign);
                }

                await _unitOfWork.SaveAsync();

                await RoutePickSorter(models, HouseCode);

                return Ok("Sales order sukses di assign!");
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "PO0001";
                return BadRequest(_error);
            }
        }

        private async Task<string> RoutePickGenerator(List<AssignSalesOrderViewModel> models)
        {

            var HouseCode = User.FindFirst("HouseCode")?.Value;

            for (int i = 0; i < models.Count; i++)
            {
                var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == models[i].OrderId &&
                    m.Status == SD.FlagSOProduct_Booked &&
                    m.FlagPick == 0 &&
                    m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData));

                if (model == null)
                {
                    return models[i].OrderId + " tidak ketemu!";
                }

                var ikuoutproducts = model.OutSalesOrderProducts.Where(m => m.MasProductData.ProductLevel == "IKU");
                var skuoutproducts = model.OutSalesOrderProducts.Where(m => m.MasProductData.ProductLevel == "SKU");

                List<SalesOrderPickViewModel> vmmodeliku = new List<SalesOrderPickViewModel>();
                List<SalesOrderPickViewModel> vmmodelsku = new List<SalesOrderPickViewModel>();

                if (ikuoutproducts.Any())
                {
                    foreach (var item in ikuoutproducts)
                    {
                        var newtempatiku = await _unitOfWork.ItemProduct.GetAllAsync(
                            disableTracking:
                                false,
                            filter:
                                m => m.Status == SD.FlagItemProduct_PUTD &&
                                m.IncDeliveryOrderProduct.ProductId == item.ProductId &&
                                m.IncDeliveryOrderProduct.IncDeliveryOrder.HouseCode == HouseCode,
                            includeProperties:
                                m => m.Include(m => m.IncDeliveryOrderProduct.IncDeliveryOrder)
                                .Include(m => m.InvStorageCode));

                        if (newtempatiku.Count() < 1)
                        {
                            return  item.MasProductData.ProductName + " masih di stagging!";
                        }

                        if (item.MasProductData.StorageMethod == "FEFO")
                        {
                            newtempatiku = newtempatiku.OrderBy(m => m.IncDeliveryOrderProduct.DateOfExpired).ToList();
                        }
                        else
                        {
                            newtempatiku = newtempatiku.OrderBy(m => m.DateArrived).ToList();
                        }

                        int counterqtyiku = item.Quantity;
                        foreach (var itemlagi in newtempatiku)
                        {
                            if (counterqtyiku > 0)
                            {
                                counterqtyiku = counterqtyiku - 1;
                                vmmodeliku.Add(new SalesOrderPickViewModel
                                {
                                    OrdProductId = item.OrdProductId,
                                    IKU = itemlagi.IKU,
                                    ProductId = item.ProductId,
                                    StorageCode = itemlagi.StorageCode,
                                    DOProductId = itemlagi.DOProductId,
                                    QtyPick = 1,
                                    QtyStock = itemlagi.InvStorageCode.Qty
                                });

                                itemlagi.Status = SD.FlagItemProduct_SOLD;
                                _unitOfWork.ItemProduct.Update(itemlagi);

                                itemlagi.InvStorageCode.QtyOrder = itemlagi.InvStorageCode.QtyOrder + 1;
                                itemlagi.InvStorageCode.Qty = itemlagi.InvStorageCode.Qty - 1;
                                _unitOfWork.StorageCode.Update(itemlagi.InvStorageCode);
                            }
                        }
                    }

                    foreach (var item in vmmodeliku)
                    {
                        OutSalesOrderStorage modeliku = new OutSalesOrderStorage
                        {
                            OrdProductId = item.OrdProductId,
                            IKU = item.IKU,
                            QtyPick = item.QtyPick,
                            StorageCode = item.StorageCode,
                            Sequence = 0
                        };
                        await _unitOfWork.SalesOrderStorage.AddAsync(modeliku);
                    }

                }

                if (skuoutproducts.Any())
                {
                    foreach (var product in skuoutproducts)
                    {
                        var newtempatsku = await _unitOfWork.PutAway.GetAllAsync(
                            disableTracking:
                                false,
                            filter:
                                m => m.IncDeliveryOrderArrival.IncDeliveryOrderProduct.IncDeliveryOrder.HouseCode == HouseCode &&
                                m.IncDeliveryOrderArrival.IncDeliveryOrderProduct.ProductId == product.ProductId &&
                                m.QtyStock > 0,
                            includeProperties:
                                m => m.Include(m => m.IncDeliveryOrderArrival.IncDeliveryOrderProduct.IncDeliveryOrder)
                                .Include(m => m.InvStorageCode));

                        if (newtempatsku.Count() < 1)
                        {
                            return  product.MasProductData.ProductName + " masih di stagging!";
                        }

                        if (newtempatsku.Sum(m => m.QtyStock) < product.Quantity)
                        {
                            return   product.MasProductData.ProductName + " masih di stagging!";
                        }

                        if (product.MasProductData.StorageMethod == "FEFO")
                        {
                            newtempatsku = newtempatsku.OrderBy(m => m.IncDeliveryOrderArrival.IncDeliveryOrderProduct.DateOfExpired).ToList();
                        }
                        else
                        {
                            newtempatsku = newtempatsku.OrderBy(m => m.DatePutaway).ToList();
                        }

                        int counterqtysku = product.Quantity;
                        foreach (var putaway in newtempatsku)
                        {
                            if (counterqtysku > 0)
                            {
                                if (putaway.QtyStock >= counterqtysku)
                                {
                                    vmmodelsku.Add(new SalesOrderPickViewModel
                                    {
                                        OrdProductId = product.OrdProductId,
                                        ProductId = product.ProductId,
                                        Id = putaway.Id,
                                        StorageCode = putaway.StorageCode,
                                        DOProductId = putaway.DOProductId,
                                        QtyPick = counterqtysku,
                                        QtyStock = putaway.QtyStock,
                                    });

                                    putaway.QtyStock = putaway.QtyStock - counterqtysku;
                                    _unitOfWork.PutAway.Update(putaway);

                                    putaway.InvStorageCode.Qty = putaway.InvStorageCode.Qty - counterqtysku;
                                    putaway.InvStorageCode.QtyOrder = putaway.InvStorageCode.QtyOrder + counterqtysku;
                                    _unitOfWork.StorageCode.Update(putaway.InvStorageCode);

                                    counterqtysku = counterqtysku - counterqtysku;
                                }
                                else
                                {
                                    vmmodelsku.Add(new SalesOrderPickViewModel
                                    {
                                        OrdProductId = product.OrdProductId,
                                        ProductId = product.ProductId,
                                        Id = putaway.Id,
                                        StorageCode = putaway.StorageCode,
                                        DOProductId = putaway.DOProductId,
                                        QtyPick = putaway.QtyStock,
                                        QtyStock = putaway.QtyStock
                                    });

                                    counterqtysku = counterqtysku - putaway.QtyStock;

                                    putaway.InvStorageCode.Qty = putaway.InvStorageCode.Qty - putaway.QtyStock;
                                    putaway.InvStorageCode.QtyOrder = putaway.InvStorageCode.QtyOrder + putaway.QtyStock;
                                    _unitOfWork.StorageCode.Update(putaway.InvStorageCode);

                                    putaway.QtyStock = putaway.QtyStock - putaway.QtyStock;
                                    _unitOfWork.PutAway.Update(putaway);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    foreach (var item in vmmodelsku)
                    {
                        OutSalesOrderStorage modelsku = new OutSalesOrderStorage
                        {
                            OrdProductId = item.OrdProductId,
                            IdPutAway = item.Id,
                            QtyPick = item.QtyPick,
                            StorageCode = item.StorageCode,
                            Sequence = 0
                        };
                        await _unitOfWork.SalesOrderStorage.AddAsync(modelsku);
                    }
                }
            }

            return "Success";
        }

        private async Task RoutePickSorter(List<AssignSalesOrderViewModel> models, string HouseCode)
        {
            var orderProducts = await _unitOfWork.SalesOrderProduct.GetAllAsync(
                filter:
                    m => models.Select(m => m.OrderId).Contains(m.OrderId));

            var orderStorages = await _unitOfWork.SalesOrderStorage.GetAllAsync(
                    disableTracking:
                        false,
                    filter:
                        m => orderProducts.Select(m => m.OrdProductId).Contains(m.OrdProductId),
                    includeProperties:
                        m => m.Include(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn),
                    orderBy:
                        m => m.OrderBy(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.ZoneCode)
                        .OrderBy(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.RowCode)
                        .OrderBy(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.ColumnCode));

            var route = await GetPickingRouteByHouseCode(HouseCode);

            if (route != null)
            {
                var pickStorages = await _unitOfWork.RoutePickColumn.GetAllAsync(
                    filter:
                        m => m.RouteCode == route.RouteCode &&
                        orderStorages.Select(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.ColumnCode).Contains(m.ColumnCode),
                    orderBy:
                        m => m.OrderBy(m => m.Order));

                for (int i = 0; i < pickStorages.Count; i++)
                {
                    for (int j = 0; j < orderStorages.Count; j++)
                    {
                        if (orderStorages[j].InvStorageCode.InvStorageBin.InvStorageLevel.ColumnCode == pickStorages[i].ColumnCode)
                        {
                            orderStorages[j].Sequence = i;
                            _unitOfWork.SalesOrderStorage.Update(orderStorages[j]);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < orderStorages.Count; i++)
                {
                    orderStorages[i].Sequence = i;
                    _unitOfWork.SalesOrderStorage.Update(orderStorages[i]);
                }
            }

            var orders = await _unitOfWork.SalesOrder.GetAllAsync(
                disableTracking:
                    false,
                filter:
                    m => models.Select(m => m.OrderId).Contains(m.OrderId),
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts));

            for (int i = 0; i < orders.Count; i++)
            {
                orders[i].FlagPick = 1;
                _unitOfWork.SalesOrder.Update(orders[i]);
            }

            await _unitOfWork.SaveAsync();
        }

        private async Task<InvPickingRoute> GetPickingRouteByHouseCode(string HouseCode)
        {
            var route = await _unitOfWork.RoutePick.GetSingleOrDefaultAsync(
                filter:
                    m => m.HouseCode == HouseCode &&
                    m.Flag == FlagEnum.Active);

            return route;
        }

        [HttpGet("PickOrder")]
        public async Task<IActionResult> Get()
        {
            try
            {
                //var model = await _unitOfWork.SalesOrderAssign.GetAllAsync(
                //    includeProperties:
                //        m => m.Include(m => m.OutSalesOrder.OutSalesOrderProducts)
                //                .ThenInclude(m => m.OutSalesOrderStorages)
                //                    .ThenInclude(m => m.InvStorageCode)
                //                        .ThenInclude(m => m.InvStorageBin)
                //           ,

                var model = await _unitOfWork.SalesOrder.GetAllAsync(
                    includeProperties:
                            m => m.Include(m => m.OutSalesOrderAssign)
                                  .Include(m => m.OutSalesOrderConsignee)
                                  .Include(m => m.OutSalesOrderProducts)
                                    .ThenInclude(m => m.OutSalesOrderStorages)
                                      .ThenInclude(m => m.InvStorageCode)
                                        .ThenInclude(m => m.InvStorageBin)
                                .Include(m => m.OutSalesOrderProducts)
                                    .ThenInclude(m => m.MasProductData)
                                        .ThenInclude(m => m.MasDataTenant)
                                       ,
                    filter:
                        m => m.OutSalesOrderAssign.UserId.ToString() == User.FindFirst("UserId").Value &&
                        m.OutSalesOrderAssign.Flag == 1);

                var result = new List<OutSalesOrderStorage>();

                foreach (var order in model)
                {
                    foreach (var product in order.OutSalesOrderProducts)
                    {
                        result.AddRange(product.OutSalesOrderStorages);
                    }
                }

                result = result.OrderBy(m => m.Sequence).ToList();

                var MOD = result.Select(x => new
                {
                    x.Id,
                    x.OutSalesOrderProduct.OutSalesOrder.OutSalesOrderAssign.PickAssignId,
                    x.OutSalesOrderProduct.OrderId,
                    x.OutSalesOrderProduct.OutSalesOrder.OutSalesOrderConsignee.CneeName,
                    TenantName = x.OutSalesOrderProduct.MasProductData.MasDataTenant.Name,
                    x.OutSalesOrderProduct.OutSalesOrder.StoreName,
                    x.OutSalesOrderProduct.OutSalesOrder.DateOrdered,
                    x.OutSalesOrderProduct.MasProductData.ProductId,
                    x.OutSalesOrderProduct.MasProductData.SKU,
                    x.OutSalesOrderProduct.MasProductData.ProductName,
                    x.OutSalesOrderProduct.MasProductData.BeautyPicture,
                    x.OutSalesOrderProduct.MasProductData.ActualWeight,
                    x.OutSalesOrderProduct.MasProductData.StorageMethod,
                    x.OutSalesOrderProduct.MasProductData.ProductCondition,
                    x.OutSalesOrderProduct.MasProductData.SerialNumber,
                    x.OutSalesOrderProduct.MasProductData.Unit,
                    x.StorageCode,
                    x.InvStorageCode.BinCode,
                    BinName = (x.InvStorageCode.InvStorageBin.BinName).Trim(),
                    x.Sequence,
                    x.QtyPick,
                    x.PickedStatus,
                });

                return Ok(MOD);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "PO0001";
                return BadRequest(_error);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post(int Id, string storageCode)
        {
            try
            {

                if (Id == 0 || storageCode == null)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Id atau Storage code diwajibkan!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                var result = await _unitOfWork.SalesOrderStorage.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.Id == Id,
                    includeProperties:
                        m => m.Include(m => m.OutSalesOrderProduct.MasProductData.InvProductStocks)
                        .Include(m => m.OutSalesOrderProduct.OutSalesOrder)
                        .Include(m => m.IncItemProduct)
                        .Include(m => m.InvStorageCode));

                if (result == null)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Product notfound!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                if (result.StorageCode.ToString().ToLower() != storageCode.ToLower())
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Storage code tidak sesuai!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                if (result.PickedStatus != "Ordered")
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Product gagal di pick!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                result.DatePicked = DateTime.Now;
                result.PickedBy = User.FindFirst("UserName")?.Value;
                result.PickedStatus = "Picked";

                //if (model.QualityCheckedStatus != null)
                //{
                result.QualityCheckedStatus = "Accept";
                //    if (model.QualityCheckedStatus == "Rejected")
                //    {
                //        result.QualityCheckedRemark = model.QualityCheckedRemark;
                //    }
                result.DateQualityChecked = DateTime.Now;
                result.QualityCheckedBy = User.FindFirst("UserName")?.Value;
                //}

                _unitOfWork.SalesOrderStorage.Update(result);

                //if (result.InvStorageCode.QtyOrder < 1)
                //{
                //    return BadRequest("Minus storage quanitity!");
                //}

                result.InvStorageCode.QtyOrder = result.InvStorageCode.QtyOrder - result.QtyPick;
                if (result.InvStorageCode.Qty < 1)
                {
                    result.InvStorageCode.Flag = 1;
                }
                _unitOfWork.StorageCode.Update(result.InvStorageCode);

                var productStock = result.OutSalesOrderProduct.MasProductData.InvProductStocks
                    .SingleOrDefault(m => m.HouseCode == result.OutSalesOrderProduct.OutSalesOrder.HouseCode);

                productStock.QtyOrder = productStock.QtyOrder - result.QtyPick;
                if (productStock.QtyOrder < 0)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Minus product stock quantity!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }
                _unitOfWork.ProductStock.Update(productStock);

                var history = new InvProductHistory
                {
                    ProductId = result.OutSalesOrderProduct.ProductId,
                    HouseCode = result.OutSalesOrderProduct.OutSalesOrder.HouseCode,
                    HistoryType = Models.ProductHistoryType.Out,
                    TrxNo = result.OutSalesOrderProduct.OrderId,
                    Interest = "SO",
                    Quantity = result.QtyPick,
                    Note = string.Empty,
                    Stock = productStock.Stock + productStock.QtyOrder,
                    DatedTime = DateTime.Now,
                    UserBy = User.FindFirst("UserName")?.Value,
                    Flag = 1
                };

                await _unitOfWork.ProductHistory.AddAsync(history);

                var soProduct = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                    disableTracking:
                       false,
                    filter:
                        m => m.OrdProductId == result.OrdProductId,
                    includeProperties:
                        m => m.Include(m => m.OutSalesOrderStorages));

                if (!soProduct.OutSalesOrderStorages.Any(m => m.PickedStatus == "Ordered"))
                {
                    soProduct.Flag = 3;
                    _unitOfWork.SalesOrderProduct.Update(soProduct);

                    var order = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                        disableTracking:
                            false,
                        filter:
                            m => m.OrderId == soProduct.OrderId,
                        includeProperties:
                            m => m.Include(m => m.OutSalesOrderProducts));

                    if (!order.OutSalesOrderProducts.Any(m => m.Flag == 2))
                    {
                        order.Status = 3;
                        _unitOfWork.SalesOrder.Update(order);

                        await _unitOfWork.SaveAsync();

                        return Ok("Semua product telah di pick!");
                    }
                }

                await _unitOfWork.SaveAsync();

                return Ok("Product sukses di pick!");

            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "PI0001";
                return BadRequest(_error);
            }
        }

        [HttpPost("staging")]
        public async Task<IActionResult> Post(string userId, string pickAssignId)
        {
            try
            {
                var result = await _unitOfWork.SalesOrderAssign.GetAllAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.UserId.ToString() == userId &&
                        m.PickAssignId.ToString() == pickAssignId,
                    includeProperties:
                        m => m.Include(m => m.OutSalesOrder));

                if (userId == null || pickAssignId == null)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "UserId atau PickAssignId diwajibkan!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                if (!result.Any(m => m.Flag == 1)) // assign kosong
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "No data picked!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                if (result.Any(m => m.OutSalesOrder.Status == 2)) // so blm selesai pick
                {
                    _error.StatusCode = "400";
                    _error.Error = "Error";
                    _error.Message = "Pick gagal!";
                    _error.Code = "PI0010";
                    return BadRequest(_error);
                }

                result = result.Where(m => m.Flag == 1).ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    //if (model.ImageStaged == null)
                    //{
                    //    return BadRequest("Image cannot be null!");
                    //}
                    //byte[] bytes = Convert.FromBase64String(model.ImageStaged);

                    //string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/SalesOrder/Stages");
                    //var fileName = result[i].OrderId + ".jpg";
                    //string filePath = Path.Combine(uploadsFolder, fileName);

                    //using (MemoryStream ms = new MemoryStream(bytes))
                    //{
                    //    Image pic = Image.FromStream(ms);
                    //    pic.Save(filePath);
                    //}

                    //result[i].ImageStaged = result[i].OrderId + ".jpg";
                    result[i].Flag = 2;
                    result[i].DateStaged = DateTime.Now;
                    _unitOfWork.SalesOrderAssign.Update(result[i]);

                    result[i].OutSalesOrder.Status = 4;
                    _unitOfWork.SalesOrder.Update(result[i].OutSalesOrder);
                }

                await _unitOfWork.SaveAsync();
                return Ok("Sukses upload!");
            }
            catch (Exception ex)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = ex.Message;
                _error.Code = "PI0001";
                return BadRequest(_error);
            }
        }
    }
}
