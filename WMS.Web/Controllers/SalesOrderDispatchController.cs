using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Utility;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class SalesOrderDispatchController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SalesOrderDispatchController(AppDbContext context, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.SalesOrderDispatch.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder)
                    .Include(m => m.MasSalesCourier));
            

            if (ProfileId == SD.Role_SuperAdmin)
            {
                ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(), "TenantId", "Name");
                ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(), "HouseCode", "HouseName", HouseCode);
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                model = model.Where(m => m.OutSalesOrder.HouseCode == HouseCode).ToList();

                ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(m => m.Flag == FlagEnum.Active && m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)), "TenantId", "Name");
                ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(m => m.HouseCode == HouseCode), "HouseCode", "HouseName", HouseCode);
            }

            ViewData["OrdCourier"] = new SelectList(await _unitOfWork.SalesOrderCourier.GetAllAsync(), "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? OrderId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            ViewData["OrdCourier"] = new SelectList(_context.MasSalesCouriers, "Id", "Name");

            if (OrderId != null)
            {
                var model = await _unitOfWork.SalesOrder.GetFirstOrDefaultAsync(
                    filter:
                        m => m.OrderId == OrderId &&
                        m.Status >= SD.FlagSO_Packed,
                    includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.OutSalesDispatchtoCouriers.Where(m => m.Flag == 1))
                    .Include(m => m.MasPlatform)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasStore)
                    .Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData));

                if (ProfileId == SD.Role_WarehouseAdmin)
                {
                    if (model.HouseCode != HouseCode)
                    {
                        TempData["error"] = "Sales Order notfound!";
                        return RedirectToAction("Index");
                    }
                }

                var newmodel = model.OutSalesDispatchtoCouriers.FirstOrDefault();

                return View(newmodel);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string? OrderId, OutSalesDispatchtoCourier model)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            ViewData["OrdCourier"] = new SelectList(_context.MasSalesCouriers, "Id", "Name");

            var result = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == OrderId &&
                    m.Status >= SD.FlagSO_Packed,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.OutSalesDispatchtoCouriers)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasStore)
                    .Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData));

            if (result == null)
            {
                TempData["error"] = "Sales Order Notfound!";
                return View();
            }

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                if (result.HouseCode != HouseCode)
                {
                    TempData["error"] = "Sales Order Notfound!";
                    return View();
                }
            }

            model.OutSalesOrder = result;

            if (result.Status < SD.FlagSO_Packed)
            {
                TempData["error"] = "Sales Order belum packed!";
                return View(model);
            }

            if (result.Status > SD.FlagSO_Packed)
            {
                TempData["error"] = "Sales Order Has Been Handed Over To Courier!";
                return View(model);
            }

            foreach(var item in result.OutSalesOrderProducts)
            {
                item.Flag = SD.FlagSOProduct_Dispatch;
                _unitOfWork.SalesOrderProduct.Update(item); 
            }

            result.Status = SD.FlagSO_Dispatch;
            _unitOfWork.SalesOrder.Update(result);

            OutSalesDispatchtoCourier dispatchtoCourier = new OutSalesDispatchtoCourier
            {
                OrdCourier = model.OrdCourier,
                CourierName = model.CourierName,
                OrderId = OrderId,
                DatedHandOvered = DateTime.Now,
                HandoveredBy = User.FindFirst("UserName")?.Value,
                Flag = 1
            };

            await _unitOfWork.SalesOrderDispatch.AddAsync(dispatchtoCourier);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Sales Order Handed Over to Courier Successfully!";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var model = await _unitOfWork.SalesOrderDispatch.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.Id == Id,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder));

            if (model == null)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction(nameof(Index));
            }

            model.OutSalesOrder.Status = SD.FlagSO_Packed;
            _unitOfWork.SalesOrder.Update(model.OutSalesOrder);

            model.Flag = 0;
            _unitOfWork.SalesOrderDispatch.Update(model);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "SalesOrder Dispatch Updated Successfullly!";
            return RedirectToAction(nameof(Index));
        }
    }
}
