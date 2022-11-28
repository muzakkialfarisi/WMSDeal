using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PurchaseReviewsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseReviewsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.PurchaseRequest.GetAllAsync(
                filter:
                    m => m.RequestStatus != "Request" &&
                    m.RequestStatus != "Cancelled" &&
                    m.MasDataTenantWarehouse.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenantWarehouse.MasHouseCode)
                    .Include(m => m.masDataTenantDivision)
                    .Include(m => m.IncRequestPurchaseProducts));
                
            return View(model);
        }

        public async Task<IActionResult> Detail(int RequestId)
        {
            var model = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                filter:
                    m => m.RequestId == RequestId,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenantWarehouse.MasHouseCode)
                    .Include(m => m.masDataTenantDivision)
                    .Include(m => m.IncRequestPurchaseProducts)
                        .ThenInclude(m => m.MasProductData));

            if (model == null)
            {
                TempData["error"] = "Request Order Notfound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Review(int RequestId)
        {
            var model = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RequestId == RequestId,
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts));

            if (model == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            model.ReviewedBy = User.FindFirst("UserName")?.Value;
            model.DateReviewed = DateTime.Now;
            model.RequestStatus = "Reviewed";
            _unitOfWork.PurchaseRequest.Update(model);

            for (int i = 0; i < model.IncRequestPurchaseProducts.Count; i++)
            {
                model.IncRequestPurchaseProducts[i].Status = "Reviewed";
                _unitOfWork.PurchaseRequestProduct.Update(model.IncRequestPurchaseProducts[i]);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Reviewed Successfullly!";
            return RedirectToAction(nameof(Index));
        }

    }
}
