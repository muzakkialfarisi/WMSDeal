using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Migrations;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Utility;
using ZXing;
using ProductHistoryType = WMS.Models.ProductHistoryType;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class DeliveryOrderArrivalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeliveryOrderArrivalController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                filter:
                    m => m.Status == SD.FlagDO_DO,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.IncDeliveryOrderProducts));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string DONumber, Guid TenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagDO_DO,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.MasDeliveryOrderCourier)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncDeliveryOrderArrivals));

            if (model == null)
            {
                TempData["error"] = "Delivery Order Notfound!";
                return RedirectToAction("Index");
            }

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                if (model.HouseCode != HouseCode)
                {
                    TempData["error"] = "Delivery Order Notfound!";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IncDeliveryOrderArrival model)
        {
            var result = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == model.DOProductId &&
                    m.Status == SD.FlagDOProduct_Booked,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.MasProductData.InvProductStocks)
                    .Include(m => m.IncDeliveryOrderArrivals)
                    .Include(m => m.IncItemProducts)
                    .Include(m => m.IncSerialNumbers));

            if (result == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index");
            }

            if (model.Quantity < 1 || model.Quantity == null)
            {
                TempData["error"] = "Quantity Not Allowed!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
            }

            var update = true;

            if (result.IncDeliveryOrderArrivals == null)
            {
                if (model.Quantity > result.Quantity)
                {
                    TempData["warning"] = "Quantity Melebihi Batas!";
                    return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
                }

                result.IncDeliveryOrderArrivals = new IncDeliveryOrderArrival();
                result.IncDeliveryOrderArrivals.DOProductId = model.DOProductId;

                update = false;
            }

            result.IncDeliveryOrderArrivals.ProductImage = model.DOProductId.ToString() + ".jpg";
            result.IncDeliveryOrderArrivals.Quantity = result.IncDeliveryOrderArrivals.Quantity + model.Quantity;
            result.IncDeliveryOrderArrivals.ArrivedBy = result.IncDeliveryOrderArrivals.ArrivedBy + User.FindFirst("UserName")?.Value.ToString() + "; ";
            result.IncDeliveryOrderArrivals.Note = result.IncDeliveryOrderArrivals.Note + model.Note + "; ";

            if (result.IncDeliveryOrderArrivals.Quantity + result.IncDeliveryOrderArrivals.QtyNotArrived > result.Quantity)
            {
                TempData["error"] = "Quantity Melebihi Batas!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
            }

            if (update == true)
            {
                _unitOfWork.DeliveryOrderArrival.Update(result.IncDeliveryOrderArrivals);
            }
            else
            {
                await _unitOfWork.DeliveryOrderArrival.AddAsync(result.IncDeliveryOrderArrivals);
            }

            var arrivalproduct = new IncDeliveryOrderArrivalProduct();
            arrivalproduct.DOProductId = model.DOProductId;
            arrivalproduct.ProductId = result.ProductId;
            arrivalproduct.Quantity = model.Quantity;
            arrivalproduct.CreatedBy = User.FindFirst("UserName")?.Value;
            arrivalproduct.Note = model.Note + ";";
            arrivalproduct.ImageUrl = arrivalproduct.Id + ".jpg";

            await _unitOfWork.DeliveryOrderArrivalProduct.AddAsync(arrivalproduct);

            var productStock = result.MasProductData.InvProductStocks.SingleOrDefault(m => m.HouseCode == result.IncDeliveryOrder.HouseCode);

            if (productStock == null)
            {
                productStock = new InvProductStock();
                productStock.Id = result.ProductId + result.IncDeliveryOrder.HouseCode;
                productStock.ProductId = (int)result.ProductId;
                productStock.HouseCode = result.IncDeliveryOrder.HouseCode;
                productStock.Stock = model.Quantity;
                await _unitOfWork.ProductStock.AddAsync(productStock);
            }
            else
            {
                productStock.Stock = productStock.Stock + model.Quantity;
                _unitOfWork.ProductStock.Update(productStock);
            }

            if (result.IncSerialNumbers.Count > 0)
            {
                var serials = result.IncSerialNumbers.Where(m => m.Status == SD.FlagSerialNumber_Open).ToList();
                if (serials.Count() < model.Quantity)
                {
                    TempData["error"] = "Quantity Melebihi Batas!";
                    return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
                }

                for (int i = 0; i < model.Quantity; i++)
                {
                    serials[i].Status = SD.FlagSerialNumber_IN;
                    _unitOfWork.SerialNumber.Update(serials[i]);
                }
            }

            InvProductHistory productHistory = new InvProductHistory
            {
                ProductId = (int)result.ProductId,
                HouseCode = result.IncDeliveryOrder.HouseCode,
                HistoryType = ProductHistoryType.In,
                TrxNo = result.DONumber,
                Interest = "Delivery Order",
                Quantity = model.Quantity,
                Note = model.Note + "; ",
                Stock = productStock.Stock + productStock.QtyOrder,
                DatedTime = DateTime.Now,
                UserBy = User.FindFirst("UserName")?.Value
            };

            await _unitOfWork.ProductHistory.AddAsync(productHistory);

            update = false;

            if (result.Quantity == result.IncDeliveryOrderArrivals.Quantity + result.IncDeliveryOrderArrivals.QtyNotArrived)
            {
                result.Status = SD.FlagDOProduct_Arrived;
                _unitOfWork.DeliveryOrderProduct.Update(result);
                update = true;
            }

            await _unitOfWork.SaveAsync();

            if (update == true)
            {
                var resultdo = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.DONumber == result.DONumber &&
                        m.TenantId == result.IncDeliveryOrder.TenantId,
                    includeProperties:
                        m => m.Include(m => m.IncDeliveryOrderProducts));

                if (!resultdo.IncDeliveryOrderProducts.Any(m => m.Status == SD.FlagDOProduct_Booked))
                {
                    resultdo.Status = SD.FlagDO_AR;
                    resultdo.DateArrived = DateTime.Now;
                    _unitOfWork.DeliveryOrder.Update(resultdo);
                    await _unitOfWork.SaveAsync();

                    TempData["success"] = "Delivery Order arrived successfully!";
                    return RedirectToAction("Detail", "DeliveryOrderList", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
                }
            }

            TempData["success"] = "Updated Successfully!";
            return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
        }

        [HttpGet]
        public async Task<IActionResult> UpsertSerialNumber(string DONumber, Guid TenantId, int DOProductId)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.IncDeliveryOrder.TenantId == TenantId &&
                    m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers));

            if (model == null)
            {
                TempData["error"] = "Produk tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            TempData["TenantId"] = TenantId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertSerialNumber(string DONumber, Guid TenantId, int DOProductId, IncSerialNumber model)
        {
            var DOProduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DONumber == DONumber &&
                    m.IncDeliveryOrder.TenantId == TenantId &&
                    m.DOProductId == DOProductId &&
                    m.Status == SD.FlagDOProduct_Booked,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.IncSerialNumbers)
                    .Include(m => m.IncDeliveryOrderArrivals)
                    .Include(m => m.MasProductData.InvProductStocks));

            TempData["TenantId"] = TenantId;

            if (DOProduct == null)
            {
                TempData["error"] = "Produk tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            var serial = DOProduct.IncSerialNumbers.Where(m => m.SerialNumber == model.SerialNumber).SingleOrDefault();

            if (serial == null)
            {
                TempData["error"] = "Serial number tidak ditemukan!";
                return View(DOProduct);
            }

            if (serial.Status != SD.FlagSerialNumber_Open)
            {
                TempData["error"] = "Serial number sudah arrival!";
                return View(DOProduct);
            }

            serial.Status = SD.FlagSerialNumber_IN;
            _unitOfWork.SerialNumber.Update(serial);

            var update = true;

            if (DOProduct.IncDeliveryOrderArrivals == null)
            {
                DOProduct.IncDeliveryOrderArrivals = new IncDeliveryOrderArrival();
                DOProduct.IncDeliveryOrderArrivals.DOProductId = DOProductId;

                update = false;
            }

            DOProduct.IncDeliveryOrderArrivals.Quantity = DOProduct.IncDeliveryOrderArrivals.Quantity + 1;
            DOProduct.IncDeliveryOrderArrivals.ProductImage = DOProductId.ToString() + ".jpg";
            DOProduct.IncDeliveryOrderArrivals.ArrivedBy = User.FindFirst("UserName")?.Value.ToString();

            if (DOProduct.IncDeliveryOrderArrivals.Quantity + DOProduct.IncDeliveryOrderArrivals.QtyNotArrived > DOProduct.Quantity)
            {
                TempData["error"] = "Quantity Melebihi Batas!";
                return View(DOProduct);
            }

            if (update == true)
            {
                _unitOfWork.DeliveryOrderArrival.Update(DOProduct.IncDeliveryOrderArrivals);
            }
            else
            {
                await _unitOfWork.DeliveryOrderArrival.AddAsync(DOProduct.IncDeliveryOrderArrivals);
            }

            var arrivalproduct = new IncDeliveryOrderArrivalProduct();
            arrivalproduct.DOProductId = model.DOProductId;
            arrivalproduct.ProductId = DOProduct.ProductId;
            arrivalproduct.Quantity = 1;
            arrivalproduct.CreatedBy = User.FindFirst("UserName")?.Value;
            arrivalproduct.Note = string.Empty + ";";
            arrivalproduct.ImageUrl = arrivalproduct.Id + ".jpg";

            await _unitOfWork.DeliveryOrderArrivalProduct.AddAsync(arrivalproduct);


            var productStock = DOProduct.MasProductData.InvProductStocks.SingleOrDefault(m => m.HouseCode == DOProduct.IncDeliveryOrder.HouseCode);

            if (productStock == null)
            {
                productStock = new InvProductStock();
                productStock.Id = DOProduct.ProductId + DOProduct.IncDeliveryOrder.HouseCode;
                productStock.ProductId = (int)DOProduct.ProductId;
                productStock.HouseCode = DOProduct.IncDeliveryOrder.HouseCode;
                productStock.Stock = 1;
                await _unitOfWork.ProductStock.AddAsync(productStock);
            }
            else
            {
                productStock.Stock = productStock.Stock + 1;
                _unitOfWork.ProductStock.Update(productStock);
            }

            InvProductHistory productHistory = new InvProductHistory
            {
                ProductId = (int)DOProduct.ProductId,
                HouseCode = DOProduct.IncDeliveryOrder.HouseCode,
                HistoryType = ProductHistoryType.In,
                TrxNo = DOProduct.DONumber,
                Interest = "Delivery Order",
                Quantity = 1,
                Note = string.Empty,
                Stock = productStock.Stock + productStock.QtyOrder,
                DatedTime = DateTime.Now,
                UserBy = User.FindFirst("UserName")?.Value,
                Flag = 1
            };
            await _unitOfWork.ProductHistory.AddAsync(productHistory);

            update = false;

            if (DOProduct.Quantity == DOProduct.IncDeliveryOrderArrivals.Quantity + DOProduct.IncDeliveryOrderArrivals.QtyNotArrived)
            {
                DOProduct.Status = "Arrived";
                _unitOfWork.DeliveryOrderProduct.Update(DOProduct);
                update = true;
            }

            await _unitOfWork.SaveAsync();

            if (update == true)
            {
                var resultdo = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.DONumber == DOProduct.DONumber &&
                        m.TenantId == DOProduct.IncDeliveryOrder.TenantId,
                    includeProperties:
                        m => m.Include(m => m.IncDeliveryOrderProducts));

                if (!resultdo.IncDeliveryOrderProducts.Any(m => m.Status == SD.FlagDOProduct_Booked))
                {
                    resultdo.Status = SD.FlagDO_AR;
                    resultdo.DateArrived = DateTime.Now;
                    _unitOfWork.DeliveryOrder.Update(resultdo);
                    await _unitOfWork.SaveAsync();

                    TempData["success"] = "Delivery Order arrived successfully!";
                    return RedirectToAction("Detail", "DeliveryOrderList", new { DONumber = resultdo.DONumber, TenantId = resultdo.TenantId });
                }
            }


            TempData["success"] = "Updated Successfully!";
            return View(DOProduct);
        }

        [HttpPost]
        public async Task<IActionResult> NotArrived(IncDeliveryOrderArrival model)
        {
            var doproduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == model.DOProductId,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderArrivals)
                    .Include(m => m.IncItemProducts));

            var updatedo = false;

            if (doproduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index");
            }

            if (model.QtyNotArrived > doproduct.Quantity)
            {
                TempData["error"] = "Kelebihan Kuantitas!";
                return RedirectToAction("Detail", new { DONumber = doproduct.DONumber });
            }

            if (doproduct.IncDeliveryOrderArrivals != null)
            {
                if (doproduct.IncDeliveryOrderArrivals.Quantity + doproduct.IncDeliveryOrderArrivals.QtyNotArrived + model.QtyNotArrived > doproduct.Quantity)
                {
                    TempData["error"] = "Kelebihan Kuantitas!";
                    return RedirectToAction("Detail", new { DONumber = doproduct.DONumber });
                }

                doproduct.IncDeliveryOrderArrivals.QtyNotArrived = doproduct.IncDeliveryOrderArrivals.QtyNotArrived + model.QtyNotArrived;
                doproduct.IncDeliveryOrderArrivals.NoteNotArrived = doproduct.IncDeliveryOrderArrivals.NoteNotArrived + "|" + model.NoteNotArrived;

                _unitOfWork.DeliveryOrderArrival.Update(doproduct.IncDeliveryOrderArrivals);

                if (doproduct.IncDeliveryOrderArrivals.QtyNotArrived == doproduct.Quantity)
                {
                    doproduct.Status = SD.FlagDOProduct_NotArrived;
                    updatedo = true;
                }
                else
                {
                    if (doproduct.IncDeliveryOrderArrivals.Quantity + doproduct.IncDeliveryOrderArrivals.QtyNotArrived == doproduct.Quantity)
                    {
                        doproduct.Status = SD.FlagDOProduct_Arrived;
                        updatedo = true;
                    }
                }
            }
            else
            {
                var newmodel = new IncDeliveryOrderArrival
                {
                    DOProductId = model.DOProductId,
                    Quantity = 0,
                    Note = string.Empty,
                    ProductImage = model.DOProductId.ToString() + ".jpg",
                    NotaImage = doproduct.DONumber + ".jpg",
                    DateArrived = DateTime.Now,
                    ArrivedBy = User.FindFirst("UserName")?.Value,
                    QtyNotArrived = model.QtyNotArrived,
                    NoteNotArrived = model.NoteNotArrived
                };

                await _unitOfWork.DeliveryOrderArrival.AddAsync(newmodel);

                if (newmodel.QtyNotArrived == doproduct.Quantity)
                {
                    doproduct.Status = SD.FlagDOProduct_NotArrived;
                    doproduct.ClosedNote = model.NoteNotArrived;
                    updatedo = true;
                }
            }

            _unitOfWork.DeliveryOrderProduct.Update(doproduct);

            if (updatedo == true)
            {
                var incdo = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.DONumber == doproduct.DONumber,
                    includeProperties:
                        m => m.Include(m => m.IncDeliveryOrderProducts));

                if (!incdo.IncDeliveryOrderProducts.Any(m => m.Status == SD.FlagDOProduct_Booked))
                {
                    incdo.Status = SD.FlagDO_AR;
                    if (incdo.IncDeliveryOrderProducts.All(m => m.Status == SD.FlagDOProduct_NotArrived))
                    {
                        incdo.Status = SD.FlagDO_NOAR;
                    }
                }
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Product Updated Successfully!";
            return RedirectToAction("Detail", new { DONumber = doproduct.DONumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid TenantId, IncDeliveryOrderProduct model)
        {
            var result = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == model.DOProductId &&
                    m.DONumber == model.DONumber &&
                    m.IncDeliveryOrder.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.IncDeliveryOrderArrivals)
                    .Include(m => m.IncSerialNumbers));

            if (result != null)
            {
                result.Status = SD.FlagDOProduct_Booked;
                result.Quantity = model.Quantity;

                if (result.IncDeliveryOrderArrivals != null)
                {
                    if (result.IncDeliveryOrderArrivals.Quantity >= result.Quantity)
                    {
                        result.IncDeliveryOrderArrivals.Quantity = result.Quantity;
                        result.Status = SD.FlagDOProduct_Arrived;
                    }
                    _unitOfWork.DeliveryOrderArrival.Update(result.IncDeliveryOrderArrivals);
                }

                _unitOfWork.DeliveryOrderProduct.Update(result);
                await _unitOfWork.SaveAsync();

                if (result.Status == SD.FlagDOProduct_Arrived)
                {
                    result.IncDeliveryOrder = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                        disableTracking:
                            false,
                        filter:
                            m => m.DONumber == result.DONumber &&
                            m.TenantId == TenantId,
                        includeProperties:
                            m => m.Include(m => m.IncDeliveryOrderProducts));

                    if (!result.IncDeliveryOrder.IncDeliveryOrderProducts.Any(m => m.Status == SD.FlagDOProduct_Booked))
                    {
                        result.IncDeliveryOrder.Status = SD.FlagDO_AR;
                        _unitOfWork.DeliveryOrder.Update(result.IncDeliveryOrder);
                        _unitOfWork.SaveAsync();

                        TempData["success"] = "Delivery Order Arrived Successfully!";
                        return RedirectToAction("Detail", "DeliveryOrderList", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
                    }
                }

                TempData["success"] = "Updated successfully!";
            }
            else
            {
                TempData["error"] = "Invalid Modelstate!";
            }

            return RedirectToAction("Upsert", new { DONumber = result?.DONumber, TenantId = result?.IncDeliveryOrder.TenantId });
        }

        [HttpGet]
        public async Task<JsonResult> GetDeliveryOrderProductByDOProductId(int DOProductId)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderArrivals));

            return Json(model);
        }
    }
}