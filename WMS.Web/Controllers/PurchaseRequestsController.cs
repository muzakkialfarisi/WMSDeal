#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Models
{
    [Authorize(Policy = "Cookie")]
    public class PurchaseRequestsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseRequestsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.PurchaseRequest.GetAllAsync(
                filter:
                    m => m.MasDataTenantWarehouse.HouseCode == HouseCode,
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

            if(model == null)
            {
                TempData["error"] = "Request Order Notfound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)), "TenantId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncRequestPurchase model)
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)), "TenantId", "Name", model.TenantId);
            ViewData["TenantDivisionId"] = new SelectList(await _unitOfWork.TenantDivision.GetAllAsync(m => m.TenantId == model.TenantId), "Id", "Name", model.TenantDivisionId);
            ViewData["TenantHouseId"] = new SelectList(await _unitOfWork.TenantWarehouse.GetAllAsync(filter: m => m.TenantId == model.TenantId, includeProperties: m => m.Include(m => m.MasHouseCode)), "Id", "MasHouseCode.HouseName", model.TenantHouseId);

            if (model.IncRequestPurchaseProducts.Count < 1)
            {
                TempData["error"] = "Product Notfound!";
                return View(model);
            }

            var Code = "REQ";
            var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
            var Last = 1.ToString("D4");

            string RequestNumber = Code + Tanggal + Last;
            if (await _unitOfWork.PurchaseRequest.AnyAsync(m => m.RequestNumber == RequestNumber))
            {
                var checker = await _unitOfWork.PurchaseRequest.GetAllAsync(m => m.RequestNumber.Contains(Code + Tanggal));
                int LastCount = int.Parse(checker.Max(m => m.RequestNumber.Substring(Code.Length + Tanggal.Length)));
                RequestNumber = Code + Tanggal + (LastCount + 1).ToString("000#");
            }

            model.RequestNumber = RequestNumber;
            model.DateRequested = DateTime.Now;
            model.RequestedBy = User.FindFirst("UserName")?.Value;
            model.RequestStatus = "Applied";

            await _unitOfWork.PurchaseRequest.AddAsync(model);

            for (int i = 0; i < model.IncRequestPurchaseProducts.Count; i++)
            {
                model.IncRequestPurchaseProducts[i].RequestId = model.RequestId;
                model.IncRequestPurchaseProducts[i].Status = "Applied";

                await _unitOfWork.PurchaseRequestProduct.AddAsync(model.IncRequestPurchaseProducts[i]);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Request Applied Successfully!";
            return RedirectToAction("Detail", new { RequestId = model.RequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int RequestId)
        {
            if (null == HttpContext.Session.GetString("UserSession"))
            {
                await _unitOfWork.UserManager.SignOut(this.HttpContext);
                return RedirectPermanent("~/Account/login");
            }

            var model = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts),
                filter:
                    m => m.RequestId == RequestId);
                        
            if(model == null)
            {
                TempData["error"] = "Request Notfound!";
                return RedirectToAction("Index");
            }

            model.RequestStatus = "Cancelled";
            model.RequestedBy = User.FindFirst("UserName")?.Value;
            _unitOfWork.PurchaseRequest.Update(model);

            for(int i = 0; i < model.IncRequestPurchaseProducts.Count; i++)
            {
                model.IncRequestPurchaseProducts[i].Status = "Cancelled";
                _unitOfWork.PurchaseRequestProduct.Update(model.IncRequestPurchaseProducts[i]);
            }
            
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Cancelled Successfullly!";
            return RedirectToAction("Detail", new {RequestId = model.RequestId});
        }

        public async Task<JsonResult> GetRequestProduct(int RequestId)
        {
            var model = await _unitOfWork.PurchaseRequestProduct.GetAllAsync(
                filter:
                    m => m.RequestId == RequestId);
            return Json(model);
        }

        public async Task<JsonResult> GetRequest(int RequestId)
        {
            var model = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                filter:
                    m => m.RequestId == RequestId);
            return Json(model);
        }
    }
}
