using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Utility;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class DeliveryOrderListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryOrderListController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> Index(DateTime? FilterDateFrom, DateTime? FilterDateTo, string? FilterHouseCode, Guid? FilterTenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                filter:
                    m => m.Status != SD.FlagDO_OPN,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.IncDeliveryOrderProducts),
                orderBy:
                    m => m.OrderByDescending(m => m.DateDelivered));

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                filter:
                    m => m.UserId == UserId);

                warehouses = warehouses.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
                model = model.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                tenants = tenants.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name");
            ViewData["HouseCode"] = new SelectList(warehouses, "HouseCode", "HouseName");

            if (FilterDateFrom != null && FilterDateTo != null)
            {
                FilterDateTo = FilterDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(59);
                model = model.Where(m => m.DateDelivered >= FilterDateFrom && m.DateDelivered <= FilterDateTo).ToList();
            }
            else
            {
                model = model.Where(m => m.DateDelivered.Value.Year == DateTime.Now.Year &&
                m.DateDelivered.Value.Month == DateTime.Now.Month).ToList();
            }

            if (FilterHouseCode != null)
            {
                model = model.Where(m => m.HouseCode == FilterHouseCode).ToList();
            }

            if (FilterTenantId != null)
            {
                model = model.Where(m => m.TenantId == FilterTenantId).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string DONumber, Guid TenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.TenantId == TenantId &&
                    m.Status != SD.FlagDO_OPN,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.MasDeliveryOrderCourier)
                    .Include(m => m.IncPurchaseOrder)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncDeliveryOrderArrivals)
                            .ThenInclude(m => m.InvProductPutaways)
                    .Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncSerialNumbers));

            if (model == null)
            {
                TempData["error"] = "Delivery Order Notfound!";
                return RedirectToAction("Index");
            }

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                filter:
                    m => m.UserId == UserId);

                if (!userWarehouses.Any(m => m.HouseCode == model.HouseCode))
                {
                    TempData["error"] = "Delivery Order Notfound!";
                    return RedirectToAction("Index");
                }
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
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
        [Authorize(Policy = "Tenant")]
        public async Task<IActionResult> DeleteDeliveryOrder(string DONumber, Guid TenantId)
        {
            var model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DONumber == DONumber &&
                    m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts)
                        .ThenInclude(m => m.IncDeliveryOrderArrivals)
                        .Include(m => m.IncDeliveryOrderProducts).ThenInclude(m => m.IncSerialNumbers));

            if (model == null)
            {
                TempData["error"] = "Delivery Order Notfound!";
                return RedirectToAction(nameof(Index));
            }

            if (model.Status == SD.FlagDO_AR || model.Status == SD.FlagDO_PUT)
            {
                TempData["error"] = "Tidak dapat merubah status!";
                return RedirectToAction("Detail", new { DONumber = DONumber, TenantId = TenantId });
            }

            foreach (var item in model.IncDeliveryOrderProducts)
            {
                if (item.IncDeliveryOrderArrivals != null)
                {
                    item.MasProductData = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                        filter:
                            m => m.ProductId == item.ProductId);

                    TempData["error"] = item.MasProductData.ProductName + "sudah ter arrival!";
                    return RedirectToAction("Detail", new { DONumber = DONumber, TenantId = TenantId });
                }

                if (item.IncSerialNumbers.Count > 0)
                {
                    _unitOfWork.SerialNumber.RemoveRange(item.IncSerialNumbers);
                }

                item.Status = SD.FlagDOProduct_Canceled;
                _unitOfWork.DeliveryOrderProduct.Update(item);
            }

            model.Status = SD.FlagDO_CNC;
            _unitOfWork.DeliveryOrder.Update(model);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Delivery Order canceled sucessfully!!"; 
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DelveryOrdersToExcel(DateTime? FilterDateFrom, DateTime? FilterDateTo, string? FilterHouseCode, Guid? FilterTenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var models = await _unitOfWork.DeliveryOrder.GetAllAsync(
                filter:
                    m => m.Status != "Open",
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierData)
                    .Include(m => m.IncDeliveryOrderProducts),
                orderBy:
                    m => m.OrderByDescending(m => m.DateDelivered));

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                filter:
                    m => m.UserId == UserId);

                models = models.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                models = models.Where(m => m.HouseCode == HouseCode).ToList();
            }

            if (FilterDateFrom != null && FilterDateTo != null)
            {
                FilterDateTo = FilterDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(59);
                models = models.Where(m => m.DateDelivered >= FilterDateFrom && m.DateDelivered <= FilterDateTo).ToList();
            }
            else
            {
                models = models.Where(m => m.DateDelivered.Value.Year == DateTime.Now.Year &&
                m.DateDelivered.Value.Month == DateTime.Now.Month).ToList();
            }

            if (FilterHouseCode != null)
            {
                models = models.Where(m => m.HouseCode == FilterHouseCode).ToList();
            }

            if (FilterTenantId != null)
            {
                models = models.Where(m => m.TenantId == FilterTenantId).ToList();
            }

            DataTable dt = new DataTable("Delivery Orders");
            dt.Columns.AddRange(new DataColumn[9] {
                                new DataColumn("DO Number"),
                                new DataColumn("PO Number"),
                                new DataColumn("Date"),
                                new DataColumn("Tenant"),
                                new DataColumn("Warehouse"),
                                new DataColumn("Supplier"),
                                new DataColumn("Product"),
                                new DataColumn("Quantity"),
                                new DataColumn("Status")
            });

            foreach (var model in models)
            {
                dt.Rows.Add(model.DOSupplier,
                            model.PONumber,
                            model.DateDelivered,
                            model.MasDataTenant.Name,
                            model.MasHouseCode.HouseCode,
                            model.MasSupplierData.Name,
                            model.IncDeliveryOrderProducts.Count,
                            model.IncDeliveryOrderProducts.Sum(m => m.Quantity),
                            model.Status);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Delivery Orders.xlsx");
                }
            }
        }
    }
}