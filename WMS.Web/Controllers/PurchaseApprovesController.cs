using WMS.DataAccess;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PurchaseApprovesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseApprovesController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = new PurchaseOrderViewModel();
            model.IncRequestPurchases = await _unitOfWork.PurchaseRequest.GetAllAsync(
                filter:
                    m => m.MasDataTenantWarehouse.HouseCode == HouseCode &&
                    m.RequestStatus == "Reviewed" ||
                    m.RequestStatus == "Approved" ||
                    m.RequestStatus == "Rejected" ||
                    m.RequestStatus == "Issued",
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenantWarehouse)
                        .ThenInclude(m => m.MasHouseCode)
                    .Include(m => m.masDataTenantDivision)
                    .Include(m => m.IncRequestPurchaseProducts));

            return View(model);
        }

        public async Task<IActionResult> Detail(int RequestId)
        {
            var model = new PurchaseOrderViewModel();
            
            model.IncRequestPurchase =  await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                filter:
                    m => m.RequestId == RequestId,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenantWarehouse)
                        .ThenInclude(m => m.MasHouseCode)
                    .Include(m => m.masDataTenantDivision)
                    .Include(m => m.IncRequestPurchaseProducts)
                        .ThenInclude(m => m.MasProductData));

            if(model.IncRequestPurchase == null)
            {
                TempData["error"] = "Request Notfound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductApproval(PurchaseOrderViewModel model)
        {
            if(model.IncRequestPurchaseProduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index");
            }

            var requestPurchaseProduct = await _unitOfWork.PurchaseRequestProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RequestProductId == model.IncRequestPurchaseProduct.RequestProductId);

            if(requestPurchaseProduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index");
            }

            requestPurchaseProduct.ApprovedQuantity = model.IncRequestPurchaseProduct.ApprovedQuantity;
            requestPurchaseProduct.ApprovedMemo = model.IncRequestPurchaseProduct.ApprovedMemo;
            requestPurchaseProduct.Status = model.IncRequestPurchaseProduct.Status;

            _unitOfWork.PurchaseRequestProduct.Update(requestPurchaseProduct);

            var requestPurchase = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RequestId == requestPurchaseProduct.RequestId,
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts));

            if(requestPurchase == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index");
            }

            if (!requestPurchase.IncRequestPurchaseProducts.Any(m => m.Status == "Reviewed"))
            {
                if (requestPurchase.IncRequestPurchaseProducts.All(m => m.Status == "Rejected"))
                {
                    requestPurchase.RequestStatus = "Rejected";
                }
                else
                {
                    requestPurchase.RequestStatus = "Approved";
                }
                requestPurchase.ReviewedBy = User.FindFirst("UserName")?.Value;
                requestPurchase.DateApproved = DateTime.Now;

                _unitOfWork.PurchaseRequest.Update(requestPurchase);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Approval Updated Successfullly!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approval(PurchaseOrderViewModel model)
        {
            if (model.IncRequestPurchase == null)
            {
                TempData["error"] = "Request Notfound!";
                return RedirectToAction("Index");
            }

            var requestPurchase = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RequestId == model.IncRequestPurchase.RequestId,
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts));          

            if(model.IncRequestPurchase.RequestStatus == "Rejected")
            {
                for(int i = 0; i < requestPurchase.IncRequestPurchaseProducts.Count; i++)
                {
                    requestPurchase.IncRequestPurchaseProducts[i].Status = "Rejected";
                    requestPurchase.IncRequestPurchaseProducts[i].ApprovedMemo = "Rejected!";
                    requestPurchase.IncRequestPurchaseProducts[i].ApprovedQuantity = 0;
                    _unitOfWork.PurchaseRequestProduct.Update(requestPurchase.IncRequestPurchaseProducts[i]);
                }
                requestPurchase.RequestStatus = "Rejected";
            }
            else
            {
                for (int i = 0; i < requestPurchase.IncRequestPurchaseProducts.Count; i++)
                {
                    requestPurchase.IncRequestPurchaseProducts[i].Status = "Approved";
                    requestPurchase.IncRequestPurchaseProducts[i].ApprovedMemo = "Approved!";
                    requestPurchase.IncRequestPurchaseProducts[i].ApprovedQuantity = requestPurchase.IncRequestPurchaseProducts[i].Quantity;
                    _unitOfWork.PurchaseRequestProduct.Update(requestPurchase.IncRequestPurchaseProducts[i]);
                }
                requestPurchase.RequestStatus = "Approved";
            }

            requestPurchase.ApprovedBy = User.FindFirst("UserName")?.Value;
            requestPurchase.DateApproved = DateTime.Now;
            _unitOfWork.PurchaseRequest.Update(requestPurchase);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Request "+ model.IncRequestPurchase.RequestStatus + " Successfullly!";
            return RedirectToAction(nameof(Index));
        }
    }
}
