using Microsoft.AspNetCore.Mvc;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class DashboardExpiredController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardExpiredController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.Product.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenant)
                    .Include(m => m.InvProductStocks.Where(m => m.HouseCode == HouseCode))
                    .Include(m => m.IncDeliveryOrderProducts.Where(m => m.Status == "Arrived" || m.Status == "Puted").OrderBy(m => m.DateOfExpired))
                        .ThenInclude(m => m.IncItemProducts.Where(m => m.Status == 3 || m.Status == 4))
                    .Include(m => m.IncDeliveryOrderProducts.Where(m => m.Status == "Arrived" || m.Status == "Puted").OrderBy(m => m.DateOfExpired))
                        .ThenInclude(m => m.IncDeliveryOrderArrivals.InvProductPutaways.Where(m => m.QtyStock > 0)),
                filter:
                    m => m.StorageMethod == "FEFO" &&
                    m.Flag == FlagEnum.Active &&
                    m.MasDataTenant.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode) && m.Flag == FlagEnum.Active);

            return View(model);
        }

        public async Task<IActionResult> Detail(int? ProductId)
        {
            var model = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.InvProductStocks)
                    .Include(m => m.MasProductPackaging).Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageSize).Include(m => m.InvStorageZone)
                    .Include(m => m.IncDeliveryOrderProducts.OrderByDescending(m => m.DOProductId))
                        .ThenInclude(m => m.IncItemProducts.Where(m => m.Status == 3 || m.Status == 4))
                            .ThenInclude(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow)
                    .Include(m => m.IncDeliveryOrderProducts.OrderByDescending(m => m.DOProductId))
                        .ThenInclude(m => m.IncDeliveryOrderArrivals)
                            .ThenInclude(m => m.InvProductPutaways)
                                .ThenInclude(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow),
                filter:
                    m => m.ProductId == ProductId);

            if (model == null)
            {
                TempData["error"] = "Something Wrong!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetStockByTenantIdByHouseCode(Guid TenantId, string HouseCode)
        {
            var incDeliveryOrderProducts = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.MasProductData),
                filter:
                    m => m.MasProductData.StorageMethod == "FEFO" &&
                    m.IncDeliveryOrder.HouseCode == HouseCode &&
                    m.IncDeliveryOrder.TenantId == TenantId);
                
            return Json(incDeliveryOrderProducts);
        }

        [HttpGet]
        public async Task<JsonResult> GetStockByHouseCodeByProductId(string HouseCode, int ProductId)
        {
            var incDeliveryOrderProducts = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.IncItemProducts.Where(m => m.Status == 2 || m.Status == 3))
                    .Include(m => m.MasProductData),
                filter:
                    m => m.ProductId == ProductId &&
                    m.IncDeliveryOrder.HouseCode == HouseCode); 
            
            return Json(incDeliveryOrderProducts);
        }
    }
}
