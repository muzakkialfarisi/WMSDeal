using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PurchaseOrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var model = await _unitOfWork.PurchaseRequest.GetAllAsync(
                filter:
                    m => m.RequestStatus == "Approved" &&
                    m.MasDataTenantWarehouse.HouseCode == HouseCode,
                includeProperties:
                 m => m.Include(m => m.MasDataTenant)
                .Include(m => m.MasDataTenantWarehouse.MasHouseCode)
                .Include(m => m.masDataTenantDivision));
                
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
        public async Task<IActionResult> Create(PurchaseOrderViewModel model)
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(filter: m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)), "TenantId", "Name", model.IncPurchaseOrder.TenantId);
            ViewData["TenantDivisionId"] = new SelectList(await _unitOfWork.TenantDivision.GetAllAsync(m => m.TenantId == model.IncPurchaseOrder.TenantId), "Id", "Name", model.IncPurchaseOrder.TenantDivisionId);
            ViewData["TenantHouseId"] = new SelectList(await _unitOfWork.TenantWarehouse.GetAllAsync(filter: m => m.TenantId == model.IncPurchaseOrder.TenantId, includeProperties: m => m.Include(m => m.MasHouseCode)), "Id", "MasHouseCode.HouseName", model.IncPurchaseOrder.TenantHouseId);
            ViewData["SupplierId"] = new SelectList(await _unitOfWork.Supplier.GetAllAsync(filter: m => m.TenantId == model.IncPurchaseOrder.TenantId), "SupplierId", "Name", model.IncPurchaseOrder.SupplierId);

            if (model.IncPurchaseOrder.IncPurchaseOrderProducts.Count < 1)
            {
                TempData["error"] = "Product Notfound!";
                return View(model);
            }

            var Code = "PO";
            var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
            var Last = 1.ToString("D4");

            string PONumber = Code + Tanggal + Last;
            if (await _unitOfWork.PurchaseOrder.AnyAsync(m => m.PONumber == Code + Tanggal + Last))
            {
                var temp = await _unitOfWork.PurchaseOrder.GetAllAsync(m => m.PONumber.Contains(Code + Tanggal));
                int LastCount = int.Parse(temp.Max(m => m.PONumber.Substring(Code.Length + Tanggal.Length)));
                PONumber = Code + Tanggal + (LastCount + 1).ToString("000#");
            }

            model.IncPurchaseOrder.PONumber = PONumber;
            model.IncPurchaseOrder.Status = "Pending";
            model.IncPurchaseOrder.DateCreated = DateTime.Now;
            model.IncPurchaseOrder.CreatedBy = User.FindFirst("UserName")?.Value;

            await _unitOfWork.PurchaseOrder.AddAsync(model.IncPurchaseOrder);

            for (int i = 0; i < model.IncPurchaseOrder?.IncPurchaseOrderProducts?.Count; i++)
            {
                model.IncPurchaseOrder.IncPurchaseOrderProducts[i].PONumber = PONumber;
                model.IncPurchaseOrder.IncPurchaseOrderProducts[i].SubTotal = model.IncPurchaseOrder.IncPurchaseOrderProducts[i].Quantity * model.IncPurchaseOrder.IncPurchaseOrderProducts[i].UnitPrice;
                model.IncPurchaseOrder.IncPurchaseOrderProducts[i].Status = "Pending";

                await _unitOfWork.PurchaseOrderProduct.AddAsync(model.IncPurchaseOrder.IncPurchaseOrderProducts[i]);
            }

            await _unitOfWork.SaveAsync();
            TempData["success"] = "Purchase Order Added Successgully!";
            return RedirectToAction("Detail", "PurchaseOrderList", new {PONumber = PONumber});
        }

        [HttpGet]
        public new async Task<IActionResult> Request(int RequestId)
        {
            if (!await _unitOfWork.PurchaseRequest.AnyAsync(m => m.RequestId == RequestId))
            {
                TempData["error"] = "Request Order Notfound!";
                return RedirectToAction("Index");
            }

            var model = new PurchaseOrderViewModel();
            model.IncRequestPurchase = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                filter:
                    m => m.RequestId == RequestId,
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts)
                        .ThenInclude(m => m.MasProductData));
            
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.TenantId == model.IncRequestPurchase.TenantId), "TenantId", "Name", model.IncRequestPurchase.TenantId);
            ViewData["TenantHouseId"] = new SelectList(await _unitOfWork.TenantWarehouse.GetAllAsync(m => m.Id == model.IncRequestPurchase.TenantHouseId, includeProperties: m => m.Include(m => m.MasHouseCode)), "Id", "MasHouseCode.HouseName", model.IncRequestPurchase.TenantHouseId);
            ViewData["SupplierId"] = new SelectList(await _unitOfWork.Supplier.GetAllAsync(m => m.TenantId == model.IncRequestPurchase.TenantId), "SupplierId", "Name");

            ViewData["TenantDivisionId"] = new SelectList(await _unitOfWork.TenantDivision.GetAllAsync(m => m.TenantId == model.IncRequestPurchase.TenantId), "Id", "Name", model.IncRequestPurchase.TenantDivisionId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Request(PurchaseOrderViewModel model)
        {
            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.TenantId == model.IncRequestPurchase.TenantId), "TenantId", "Name", model.IncRequestPurchase.TenantId);
            ViewData["TenantHouseId"] = new SelectList(await _unitOfWork.TenantWarehouse.GetAllAsync(m => m.Id == model.IncRequestPurchase.TenantHouseId, includeProperties: m => m.Include(m => m.MasHouseCode)), "Id", "MasHouseCode.HouseName", model.IncRequestPurchase.TenantHouseId);
            ViewData["SupplierId"] = new SelectList(await _unitOfWork.Supplier.GetAllAsync(m => m.TenantId == model.IncRequestPurchase.TenantId), "SupplierId", "Name", model.IncPurchaseOrder.SupplierId);
            ViewData["TenantHouseId"] = new SelectList(await _unitOfWork.TenantWarehouse.GetAllAsync(filter: m => m.TenantId == model.IncRequestPurchase.TenantId, includeProperties: m => m.Include(m => m.MasHouseCode)), "Id", "MasHouseCode.HouseName", model.IncRequestPurchase.TenantHouseId);

            var newmodel = new PurchaseOrderViewModel();
            newmodel.IncRequestPurchase = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts)
                        .ThenInclude(m => m.MasProductData),
                filter:
                    m => m.RequestId == model.IncRequestPurchase.RequestId);
            
            if(model.IncPurchaseOrder != null)
            {
                if (model.IncPurchaseOrder.IncPurchaseOrderProducts.Count < 1)
                {
                    TempData["error"] = "Product Notfound!"; 
                    return View(newmodel);
                }
            }

            var Code = "PO";
            var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
            var Last = 1.ToString("D4");

            string PONumber = Code + Tanggal + Last;
            if (await _unitOfWork.PurchaseOrder.AnyAsync(m => m.PONumber == Code + Tanggal + Last))
            {
                var temp = await _unitOfWork.PurchaseOrder.GetAllAsync(m => m.PONumber.Contains(Code + Tanggal));
                int LastCount = int.Parse(temp.Max(m => m.PONumber.Substring(Code.Length + Tanggal.Length)));
                PONumber = Code + Tanggal + (LastCount + 1).ToString("000#");
            }

            model.IncPurchaseOrder.PONumber = PONumber;
            model.IncPurchaseOrder.RequestId = model.IncRequestPurchase.RequestId;
            model.IncPurchaseOrder.TenantId = model.IncRequestPurchase.TenantId;
            model.IncPurchaseOrder.TenantDivisionId = model.IncRequestPurchase.TenantDivisionId;
            model.IncPurchaseOrder.TenantHouseId = model.IncRequestPurchase.TenantHouseId;
            model.IncPurchaseOrder.SupplierId = model.IncPurchaseOrder.SupplierId;
            model.IncPurchaseOrder.Discount = model.IncPurchaseOrder.Discount;
            model.IncPurchaseOrder.OrderTax = model.IncPurchaseOrder.OrderTax;
            model.IncPurchaseOrder.Note = model.IncRequestPurchase.SpecialInstruction;
            model.IncPurchaseOrder.Status = "Pending";
            model.IncPurchaseOrder.DateCreated = DateTime.Now;
            model.IncPurchaseOrder.CreatedBy = User.FindFirst("UserName")?.Value;

            await _unitOfWork.PurchaseOrder.AddAsync(model.IncPurchaseOrder);

            for (int i = 0; i < model.IncPurchaseOrder?.IncPurchaseOrderProducts?.Count; i++)
            {
                model.IncPurchaseOrder.IncPurchaseOrderProducts[i].PONumber = PONumber;
                model.IncPurchaseOrder.IncPurchaseOrderProducts[i].SubTotal = model.IncPurchaseOrder.IncPurchaseOrderProducts[i].Quantity * model.IncPurchaseOrder.IncPurchaseOrderProducts[i].UnitPrice;
                model.IncPurchaseOrder.IncPurchaseOrderProducts[i].Status = "Pending";
                await _unitOfWork.PurchaseOrderProduct.AddAsync(model.IncPurchaseOrder.IncPurchaseOrderProducts[i]);

                var incRequestPurchaseProduct = await _unitOfWork.PurchaseRequestProduct.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter :
                        m => m.RequestProductId == model.IncRequestPurchase.IncRequestPurchaseProducts[i].RequestProductId);
                if(incRequestPurchaseProduct != null)
                {
                    incRequestPurchaseProduct.Status = "Issued";
                    _unitOfWork.PurchaseRequestProduct.Update(incRequestPurchaseProduct);
                }
            }

            await _unitOfWork.SaveAsync();

            var checker = await _unitOfWork.PurchaseRequest.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                includeProperties:
                    m => m.Include(m => m.IncRequestPurchaseProducts),
                filter:
                    m => m.RequestId == model.IncRequestPurchase.RequestId);

            if (!checker.IncRequestPurchaseProducts.Any(m => m.Status == "Approved"))
            {
                checker.RequestStatus = "Issued";
                _unitOfWork.PurchaseRequest.Update(checker);
                await _unitOfWork.SaveAsync();
            }

            TempData["success"] = "Purchase Order Added Successfully!";
            return RedirectToAction("Detail", "PurchaseOrderList", new { PONumber = PONumber });
        }

        [HttpGet]
        public async Task<JsonResult> GetPurchaseOrderProductByPOProductId(int POProductId)
        {
            var incPurchaseOrderProduct = await _unitOfWork.PurchaseOrderProduct.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductData),
                filter:
                    m => m.POProductId == POProductId);
            return Json(incPurchaseOrderProduct);
        }

    }
}
