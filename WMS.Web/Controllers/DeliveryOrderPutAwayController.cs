using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Migrations;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Utility;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class DeliveryOrderPutAwayController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryOrderPutAwayController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                    filter:
                        m => m.Status == SD.FlagDO_AR,
                    includeProperties:
                        m => m.Include(m => m.MasDataTenant)
                        .Include(m => m.MasHouseCode)
                        .Include(m => m.MasDeliveryOrderCourier)
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
                    m.Status == SD.FlagDO_AR,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.MasDeliveryOrderCourier)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncDeliveryOrderArrivals)
                            .ThenInclude(m => m.InvProductPutaways));

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
        public async Task<IActionResult> Upsert(InvProductPutaway model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid Modeltate!";
                return RedirectToAction("Index");
            }

            var result = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == model.DOProductId &&
                    m.Status == SD.FlagDOProduct_Arrived,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderArrivals).ThenInclude(m => m.InvProductPutaways));

            if (result == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction(nameof(Index));
            }

            var storagecode = await _unitOfWork.StorageCode.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.StorageCode.ToString().ToLower() == model.StorageCode.ToString().ToLower() &&
                    m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode.ToLower() == result.IncDeliveryOrder.HouseCode.ToLower() &&
                    m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.ZoneCode == result.MasProductData.ZoneCode &&
                    m.Flag >= 1);

            if (storagecode == null)
            {
                TempData["error"] = "StorageCode tidak sesuai dengan Product Storage!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
            }

            storagecode.Flag = 3;
            storagecode.Qty = storagecode.Qty + model.Quantity;
            _unitOfWork.StorageCode.Update(storagecode);

            var resultputaway = result.IncDeliveryOrderArrivals.InvProductPutaways.SingleOrDefault(m => m.StorageCode == storagecode.StorageCode);

            var update = true;

            if (resultputaway == null)
            {
                resultputaway = new InvProductPutaway();

                resultputaway.DOProductId = result.DOProductId;
                resultputaway.StorageCode = model.StorageCode;

                update = false;
            }

            if (model.Quantity == null || model.Quantity < 1)
            {
                TempData["error"] = "Quantity harus lebih dari 0!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
            }

            resultputaway.Quantity = resultputaway.Quantity + model.Quantity;
            resultputaway.QtyStock = resultputaway.QtyStock + model.Quantity;
            resultputaway.PutBy = resultputaway.PutBy + User.FindFirst("UserName")?.Value.ToString() + "; ";

            if (resultputaway.Quantity > result.IncDeliveryOrderArrivals.Quantity || resultputaway.Quantity < 1)
            {
                TempData["error"] = "Over Quantity!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
            }

            if (update == true)
            {
                _unitOfWork.PutAway.Update(resultputaway);
            }
            else
            {
                await _unitOfWork.PutAway.AddAsync(resultputaway);
            }

            await _unitOfWork.SaveAsync();

            var resultputaways = await _unitOfWork.PutAway.GetAllAsync(
                filter:
                    m => m.DOProductId == model.DOProductId);

            if (result.IncDeliveryOrderArrivals.Quantity == resultputaways.Sum(m => m.Quantity))
            {
                result.Status = SD.FlagDOProduct_Puted;
                _unitOfWork.DeliveryOrderProduct.Update(result);

                await _unitOfWork.SaveAsync();

                var main = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                    filter:
                        m => m.DONumber == result.DONumber,
                    includeProperties:
                        m => m.Include(m => m.IncDeliveryOrderProducts));

                if (!main.IncDeliveryOrderProducts.Any(m => m.Status == SD.FlagDOProduct_Arrived))
                {
                    result.IncDeliveryOrder.Status = SD.FlagDO_PUT;
                    _unitOfWork.DeliveryOrder.Update(result.IncDeliveryOrder);

                    await _unitOfWork.SaveAsync(); 
                    TempData["success"] = "PutAway Successfully!";
                    return RedirectToAction("Detail", "DeliveryOrderList", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
                }
            }

            TempData["success"] = "PutAway Successfully!";
            return RedirectToAction("Upsert", new { DONumber = result.DONumber, TenantId = result.IncDeliveryOrder.TenantId });
        }
    }
}
