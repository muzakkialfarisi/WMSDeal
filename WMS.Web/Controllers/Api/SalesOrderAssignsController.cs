using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Migrations;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Utility;
using OutSalesOrderAssign = WMS.Models.OutSalesOrderAssign;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class SalesOrderAssignsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesOrderAssignsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string HouseCode, [FromBody] List<OutSalesOrder> models)
        {
            if (models.Count != 1)
            {
                return BadRequest("Pilih 1 Sales Order!");
            }

            if (HouseCode != User.FindFirst("HouseCode")?.Value)
            {
                return BadRequest("Invalid HouseCode!");
            }

            Guid UserId = new Guid(User.FindFirst("UserId")?.Value);

            var assigns = await _unitOfWork.SalesOrderAssign.GetAllAsync(
                filter:
                    m => m.UserId == UserId &&
                    m.Flag == 1);

            if (assigns.Count > 0)
            {
                return BadRequest("You still have unfinished pick!");
            }

            var Build = await RoutePickGenerator(models);
            if (Build != "Success")
            {
                return BadRequest(Build);
            }

            Guid PickAssignId = Guid.NewGuid();

            var newmodels = new List<OutSalesOrder>();

            for (int i = 0; i < models.Count; i++)
            {
                newmodels.Add(await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(m => m.OrderId == models[i].OrderId));
                if (newmodels[i].FlagPick != 0)
                {
                    return BadRequest(newmodels[i].OrderId + " has been picked!");
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

            return Ok("Sales Order Assigned Successfuly!");
        }

        [HttpGet("GetPickAssignIdByUserBySOStorage")]
        public async Task<IActionResult> GetAction(Guid UserId, int Id)
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

            return BadRequest("Notfound!");
        }

        private async Task<string> RoutePickGenerator(List<OutSalesOrder> models)
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
                    return models[i].OrderId + " Notfound!";
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
                            return "cannot pick, " + item.MasProductData.ProductName + "still on stagging!";
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
                            return "cannot pick, " + product.MasProductData.ProductName + "still on stagging!";
                        }

                        if (newtempatsku.Sum(m => m.QtyStock) < product.Quantity)
                        {
                            return "cannot pick, " + product.MasProductData.ProductName + "still on stagging!";
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

        private async Task RoutePickSorter(List<OutSalesOrder> models, string HouseCode)
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
    }
}