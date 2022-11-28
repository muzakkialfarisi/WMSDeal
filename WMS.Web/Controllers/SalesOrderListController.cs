using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WMS.DataAccess;
using WMS.DataAccess.Repository.IRepository;
using WMS.Utility;

namespace App.WMSDeal.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class SalesOrderListController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public SalesOrderListController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? FilterDateFrom, DateTime? FilterDateTo, string? FilterHouseCode, Guid? FilterTenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            var model = await _unitOfWork.SalesOrder.GetAllAsync(
                filter:
                    m => m.Status != SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts),
                orderBy:
                    m => m.OrderByDescending(m => m.DateOrdered));

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                filter:
                    m => m.UserId.ToString() == UserId);

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
                model = model.Where(m => m.DateOrdered >= FilterDateFrom && m.DateOrdered <= FilterDateTo).ToList();
            }
            else
            {
                model = model.Where(m => m.DateOrdered.Year == DateTime.Now.Year &&
                m.DateOrdered.Month == DateTime.Now.Month).ToList();
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

        [HttpPost]
        public async Task<IActionResult> SalesOrdersToExcel(DateTime? FilterDateFrom, DateTime? FilterDateTo, string? FilterHouseCode, Guid? FilterTenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            var models = await _unitOfWork.SalesOrder.GetAllAsync(
                filter:
                    m => m.Status != SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts),
                orderBy:
                    m => m.OrderByDescending(m => m.DateOrdered));

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                filter:
                    m => m.UserId.ToString() == UserId);

                warehouses = warehouses.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
                models = models.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                tenants = tenants.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
                models = models.Where(m => m.HouseCode == HouseCode).ToList();
            }

            if (FilterDateFrom != null && FilterDateTo != null)
            {
                FilterDateTo = FilterDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(59);
                models = models.Where(m => m.DateOrdered >= FilterDateFrom && m.DateOrdered <= FilterDateTo).ToList();
            }
            else
            {
                models = models.Where(m => m.DateOrdered.Year == DateTime.Now.Year &&
                m.DateOrdered.Month == DateTime.Now.Month).ToList();
            }

            if (FilterHouseCode != null)
            {
                models = models.Where(m => m.HouseCode == FilterHouseCode).ToList();
            }

            if (FilterTenantId != null)
            {
                models = models.Where(m => m.TenantId == FilterTenantId).ToList();
            }

            DataTable dt = new DataTable("Sales Orders");
            dt.Columns.AddRange(new DataColumn[8] {
                                new DataColumn("SO Number"),
                                new DataColumn("Type"),
                                new DataColumn("Date"),
                                new DataColumn("Tenant"),
                                new DataColumn("Warehouse"),
                                new DataColumn("Product"),
                                new DataColumn("Quantity"),
                                new DataColumn("Status")
            });

            foreach (var model in models)
            {
                string status = "";
                if (model.Status == 0)
                {
                    status = "Cancelled";
                }
                if (model.Status == 1)
                {
                    status = "Open";
                }
                if (model.Status == 2)
                {
                    status = "Ordered";
                }
                if (model.Status == 3 || model.Status == 4)
                {
                    status = "Picked";
                }
                if (model.Status == 5)
                {
                    status = "Packed";
                }
                if (model.Status == 6)
                {
                    status = "Dispatch";
                }

                dt.Rows.Add(model.OrderId,
                            model.MasSalesType.StyName,
                            model.DateOrdered,
                            model.MasDataTenant.Name,
                            model.MasHouseCode.HouseCode,
                            model.OutSalesOrderProducts.Count,
                            model.OutSalesOrderProducts.Sum(m => m.Quantity),
                            status);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sales Orders.xlsx");
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> Detail(string OrderId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts).ThenInclude(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderProducts).ThenInclude(m => m.IncSerialNumbers)
                    .Include(m => m.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutsalesOrderDelivery.MasSalesCourier));

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                filter:
                    m => m.UserId == UserId);

                if (!userWarehouses.Any(m => m.HouseCode == model.HouseCode))
                {
                    TempData["error"] = "SONumber Notfound!";
                    return RedirectToAction("Index");
                }
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                if (model.HouseCode != HouseCode)
                {
                    TempData["error"] = "SONumber Notfound!";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSalesOrder(string OrderId, Guid TenantId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == OrderId &&
                    m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts)
                    .ThenInclude(m => m.MasProductData)
                    .ThenInclude(m => m.InvProductStocks)
                    .Include(m => m.OutSalesOrderCustomer)
                    .Include(m => m.OutSalesOrderConsignee)
                    .Include(m => m.OutsalesOrderDelivery));

            if (model == null)
            {
                TempData["error"] = "Sales Order Notfund!";
                return RedirectToAction("Index");
            }

            if (model.FlagPick != 0 || model.Status != SD.FlagSO_Ordered)
            {
                TempData["error"] = "Sales Order has been processed!";
                return RedirectToAction("Index");
            }

            foreach (var item in model.OutSalesOrderProducts)
            {
                var produkstok = item.MasProductData.InvProductStocks.SingleOrDefault(m => m.HouseCode == model.HouseCode);

                if (produkstok == null)
                {
                    TempData["error"] = "Invalid Modelstate!";
                    return RedirectToAction("Index");
                }

                produkstok.QtyOrder = produkstok.QtyOrder - item.Quantity;
                produkstok.Stock = produkstok.Stock + item.Quantity;

                if (produkstok.QtyOrder < 0)
                {
                    TempData["error"] = "Minus Quantity Order!";
                    return RedirectToAction("Index");
                }

                _unitOfWork.ProductStock.Update(produkstok);

                item.Flag = SD.FlagSOProduct_Canceled;

                _unitOfWork.SalesOrderProduct.Update(item);
            }

            model.Status = SD.FlagSO_Canceled;
            _unitOfWork.SalesOrder.Update(model);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Sales Order Canceled Successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetMasPackingTypes()
        {
            var model = await _context.MasPackingType.ToListAsync();
            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetSerialNumberByOrdProductId(string OrderId, int OrdProductId)
        {
            var model = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.OrdProductId == OrdProductId,
                includeProperties:
                    m => m.Include(m => m.IncSerialNumbers)
                    .Include(m => m.MasProductData));

            return Json(Ok(model));
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesOrderProductByOrdProductId(int OrdProductId)
        {
            var model = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrdProductId == OrdProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderPack));

            return Json(model);
        }
    }
}