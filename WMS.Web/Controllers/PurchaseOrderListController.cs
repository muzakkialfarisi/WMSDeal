using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PurchaseOrderListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var model = await _unitOfWork.PurchaseOrder.GetAllAsync(
                filter:
                    m => m.MasDataTenantWarehouse.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenantWarehouse.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.masDataTenantDivision)
                    .Include(m => m.IncPurchaseOrderProducts),
                orderBy:
                    m => m.OrderByDescending(m => m.DateCreated));
                
            return View(model);
        }

        public async Task<IActionResult> Detail(string? PONumber)
        {
            DeliveryOrderViewModel model = new();

            model.incPurchaseOrder = await _unitOfWork.PurchaseOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.PONumber == PONumber,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.masDataTenantDivision)
                    .Include(m => m.MasDataTenantWarehouse.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.IncPurchaseOrderProducts)
                        .ThenInclude(m => m.MasProductData));

            if(model.incPurchaseOrder == null)
            {
                TempData["error"] = "Purchase Order Notfound!";
                return RedirectToAction("Index");
            }

            model.incDeliveryOrders = await _unitOfWork.DeliveryOrder.GetAllAsync(
                filter:
                    m => m.PONumber == PONumber,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts));

            return View(model);
        }

    }
}
