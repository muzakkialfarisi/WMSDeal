using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Models.ViewModels;
using WMS.Utility;
using ClosedXML.Excel;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class DashboardMonitoringController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardMonitoringController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? FilterDateFrom, DateTime? FilterDateTo, Guid? FilterTenantId, string? FilterHouseCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            var models = await _unitOfWork.ProductStock.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductData)
                    .ThenInclude(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            var histories = await _unitOfWork.ProductHistory.GetAllAsync(
                orderBy:
                    m => m.OrderByDescending(m => m.DatedTime));

            var histories2 = histories;

            var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
            filter:
                m => m.UserId.ToString() == UserId);

            if (ProfileId == SD.Role_Tenant)
            {
                warehouses = warehouses.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                tenants = tenants.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
            }

            if (FilterDateFrom != null && FilterDateTo != null)
            {
                FilterDateTo = FilterDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(59);
                histories = histories.Where(m => m.DatedTime >= FilterDateFrom && m.DatedTime <= FilterDateTo).ToList();
            }
            else
            {
                FilterDateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                histories = histories.Where(m => m.DatedTime.ToShortDateString() == FilterDateFrom.Value.ToShortDateString()).ToList();
            }

            if (FilterHouseCode != null)
            {
                if (FilterHouseCode == "*")
                {
                    if (ProfileId == SD.Role_Tenant)
                    {
                        models = models.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
                        histories = histories.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
                    }
                    else if (ProfileId == SD.Role_WarehouseAdmin)
                    {
                        models = models.Where(m => m.HouseCode == HouseCode).ToList();
                        histories = histories.Where(m => m.HouseCode == HouseCode).ToList();
                    }
                }
                else
                {
                    models = models.Where(m => m.HouseCode == FilterHouseCode).ToList();
                    histories = histories.Where(m => m.HouseCode == FilterHouseCode).ToList();
                }
            }
            else
            {
                models = models.Where(m => m.HouseCode == HouseCode).ToList();
                histories = histories.Where(m => m.HouseCode == HouseCode).ToList();
            }

            if (FilterTenantId != null)
            {
                models = models.Where(m => m.MasProductData.TenantId == FilterTenantId).ToList();
            }


            var results = new List<ProductMonitoringToExcel>();

            for (int i = 0; i < models.Count; i++)
            {
                var result = new ProductMonitoringToExcel();
                result.Id = models[i].Id;
                result.BeautyPicture = models[i].MasProductData.BeautyPicture;
                result.ProductName = models[i].MasProductData.ProductName;
                result.SKU = models[i].MasProductData.SKU;
                result.SerialNumber = models[i].MasProductData.SerialNumber;
                result.ProductCondition = models[i].MasProductData.ProductCondition;
                result.Tenant = models[i].MasProductData.MasDataTenant.Name;
                result.HouseName = models[i].MasHouseCode.HouseName;

                var Start_Stock = histories2.FirstOrDefault(m => m.ProductId == models[i].ProductId &&
                                            m.HouseCode == models[i].HouseCode &&
                                            m.DatedTime < FilterDateFrom);

                if (Start_Stock != null)
                {
                    result.Start_Stock = Start_Stock.Stock;
                }
                else
                {
                    result.Start_Stock = 0;
                }

                result.Incoming = histories.Where(m => m.HistoryType == ProductHistoryType.In && m.ProductId == models[i].ProductId && m.HouseCode == models[i].HouseCode).Sum(m => m.Quantity);
                result.Outgoing = histories.Where(m => m.HistoryType == ProductHistoryType.Out && m.ProductId == models[i].ProductId && m.HouseCode == models[i].HouseCode).Sum(m => m.Quantity);
                result.End_Stock = models[i].Stock + models[i].QtyOrder;
                result.UOM = models[i].MasProductData.Unit;

                results.Add(result);
            }

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name");
            ViewData["HouseCode"] = new SelectList(warehouses, "HouseCode", "HouseName");

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> DetailHistory(string Id)
        {
            var productstock = await _unitOfWork.ProductStock.GetSingleOrDefaultAsync(
                filter:
                    m => m.Id == Id);

            if (productstock == null)
            {
                TempData["error"] = "Product Nofound!";
                return RedirectToAction("Index");
            }

            var model = new ProductMonitoringViewModel();

            model.masProductData = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductId == productstock.ProductId,
                includeProperties:
                    m => m
                    .Include(m => m.InvProductStocks.Where(m => m.HouseCode == productstock.HouseCode))
                    .Include(m => m.IncDeliveryOrderProducts
                        .Where(m => m.IncDeliveryOrder.HouseCode == productstock.HouseCode &&
                        (m.Status == SD.FlagDOProduct_Arrived || m.Status == SD.FlagDOProduct_Puted)))
                            .ThenInclude(m => m.IncDeliveryOrderArrivals.InvProductPutaways.Where(m => m.QtyStock > 0))
                                .ThenInclude(m => m.InvStorageCode.InvStorageBin)
                    .Include(m => m.OutSalesOrderProducts
                        .Where(m => m.OutSalesOrder.HouseCode == productstock.HouseCode &&
                        (m.Flag >= SD.FlagSOProduct_Booked)))
                );
                
            model.invProductHistories = new List<InvProductHistory>(
                await _unitOfWork.ProductHistory.GetAllAsync(
                    filter:
                        m => m.ProductId == productstock.ProductId &&
                        m.HouseCode == productstock.HouseCode,
                    orderBy:
                        m => m.OrderByDescending(m => m.Id)));

            if (model.masProductData == null)
            {
                TempData["error"] = "Product Nofound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DetailInventory(string Id)
        {
            var productstock = await _unitOfWork.ProductStock.GetSingleOrDefaultAsync(
                filter:
                    m => m.Id == Id);

            if (productstock == null)
            {
                TempData["error"] = "Product Nofound!";
                return RedirectToAction("Index");
            }

            var model = new ProductMonitoringViewModel();

            model.masProductData = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductId == productstock.ProductId,
                includeProperties:
                    m => m
                    .Include(m => m.InvProductStocks.Where(m => m.HouseCode == productstock.HouseCode))
                    .Include(m => m.IncDeliveryOrderProducts
                        .Where(m => m.IncDeliveryOrder.HouseCode == productstock.HouseCode &&
                        (m.Status == SD.FlagDOProduct_Arrived || m.Status == SD.FlagDOProduct_Puted)))
                            .ThenInclude(m => m.IncDeliveryOrderArrivals.InvProductPutaways.Where(m => m.QtyStock > 0))
                                .ThenInclude(m => m.InvStorageCode.InvStorageBin)
                );

            model.invProductHistories = new List<InvProductHistory>(
                await _unitOfWork.ProductHistory.GetAllAsync(
                    filter:
                        m => m.ProductId == productstock.ProductId &&
                        m.HouseCode == productstock.HouseCode,
                    orderBy:
                        m => m.OrderByDescending(m => m.Id)));

            if (model.masProductData == null)
            {
                TempData["error"] = "Product Nofound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult<InvProductStock>> DetailSerialNumber(string Id)
        {
            var productstock = await _unitOfWork.ProductStock.GetSingleOrDefaultAsync(
                filter:
                    m => m.Id == Id);

            if (productstock == null)
            {
                TempData["error"] = "Product Nofound!";
                return RedirectToAction("Index");
            }

            var model = new ProductMonitoringViewModel();

            model.masProductData = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductId == productstock.ProductId,
                includeProperties:
                    m => m
                    .Include(m => m.IncSerialNumbers.Where(m => m.Status != SD.FlagSerialNumber_Open &&
                    m.IncDeliveryOrderProduct.IncDeliveryOrder.HouseCode == productstock.HouseCode))
                    .Include(m => m.InvProductStocks.Where(m => m.HouseCode == productstock.HouseCode))
                    .Include(m => m.IncDeliveryOrderProducts
                        .Where(m => m.IncDeliveryOrder.HouseCode == productstock.HouseCode &&
                        (m.Status == SD.FlagDOProduct_Arrived || m.Status == SD.FlagDOProduct_Puted)))
                            .ThenInclude(m => m.IncDeliveryOrderArrivals.InvProductPutaways.Where(m => m.QtyStock > 0))
                                .ThenInclude(m => m.InvStorageCode.InvStorageBin)
                    .Include(m => m.OutSalesOrderProducts
                        .Where(m => m.OutSalesOrder.HouseCode == productstock.HouseCode &&
                        (m.Flag >= SD.FlagSOProduct_Booked)))
                );

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> EditSerialNumber(string Id, IncSerialNumber model)
        {
            var models = await _unitOfWork.SerialNumber.GetAllAsync(
                disableTracking:
                    false,
                filter:
                    m => m.ProductId == model.ProductId);

            var result = models.SingleOrDefault(m => m.SerialId == model.SerialId);

            var DOProduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == model.DOProductId);

            if (result == null || DOProduct == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index", "Dashboards");
            }

            if (models.Any(m => m.SerialNumber == model.SerialNumber &&
            m.ProductId == model.ProductId &&
            m.SerialId != model.SerialId))
            {
                TempData["error"] = "SN already exsist!";
                return RedirectToAction("DetailSerialNumber", new { Id = Id });
            }

            if (result.Status == SD.FlagSerialNumber_OUT)
            {
                TempData["error"] = "SN tidak dapat diupdate!";
                return RedirectToAction("DetailSerialNumber", new { Id = Id });
            }

            result.SerialNumber = model.SerialNumber;

            _unitOfWork.SerialNumber.Update(result);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "SN updated successfully!";
            return RedirectToAction("DetailSerialNumber", new { Id = Id });
        }

        [HttpPost]
        public async Task<IActionResult> RekapProductToExcel(DateTime? FilterDateFrom, DateTime? FilterDateTo, Guid? FilterTenantId, string? FilterHouseCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            var models = await _unitOfWork.ProductStock.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductData)
                    .ThenInclude(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            var histories = await _unitOfWork.ProductHistory.GetAllAsync(
                orderBy:
                    m => m.OrderByDescending(m => m.DatedTime));

            var histories2 = histories;

            var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
            filter:
                m => m.UserId.ToString() == UserId);

            if (ProfileId == SD.Role_Tenant)
            {
                warehouses = warehouses.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                tenants = tenants.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
            }

            if (FilterDateFrom != null && FilterDateTo != null)
            {
                FilterDateTo = FilterDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(59);
                histories = histories.Where(m => m.DatedTime >= FilterDateFrom && m.DatedTime <= FilterDateTo).ToList();
            }
            else
            {
                FilterDateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                histories = histories.Where(m => m.DatedTime.ToShortDateString() == FilterDateFrom.Value.ToShortDateString()).ToList();
            }

            if (FilterHouseCode != null)
            {
                if (FilterHouseCode == "*")
                {
                    if (ProfileId == SD.Role_Tenant)
                    {
                        models = models.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
                        histories = histories.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
                    }
                    else if (ProfileId == SD.Role_WarehouseAdmin)
                    {
                        models = models.Where(m => m.HouseCode == HouseCode).ToList();
                        histories = histories.Where(m => m.HouseCode == HouseCode).ToList();
                    }
                }
                else
                {
                    models = models.Where(m => m.HouseCode == FilterHouseCode).ToList();
                    histories = histories.Where(m => m.HouseCode == FilterHouseCode).ToList();
                }
            }
            else
            {
                models = models.Where(m => m.HouseCode == HouseCode).ToList();
                histories = histories.Where(m => m.HouseCode == HouseCode).ToList();
            }

            if (FilterTenantId != null)
            {
                models = models.Where(m => m.MasProductData.TenantId == FilterTenantId).ToList();
            }


            var results = new List<ProductMonitoringToExcel>();

            for (int i = 0; i < models.Count; i++)
            {
                var result = new ProductMonitoringToExcel();
                result.No = i + 1;
                result.BeautyPicture = models[i].MasProductData.BeautyPicture;
                result.ProductName = models[i].MasProductData.ProductName;
                result.SKU = models[i].MasProductData.SKU;
                result.ProductCondition = models[i].MasProductData.ProductCondition;
                result.Tenant = models[i].MasProductData.MasDataTenant.Name;
                result.HouseName = models[i].MasHouseCode.HouseName;

                var Start_Stock = histories2.FirstOrDefault(m => m.ProductId == models[i].ProductId &&
                                            m.HouseCode == models[i].HouseCode &&
                                            m.DatedTime < FilterDateFrom);

                if (Start_Stock != null)
                {
                    result.Start_Stock = Start_Stock.Stock;
                }
                else
                {
                    result.Start_Stock = 0;
                }

                result.Incoming = histories.Where(m => m.HistoryType == Models.ProductHistoryType.In && m.ProductId == models[i].ProductId && m.HouseCode == models[i].HouseCode).Sum(m => m.Quantity);
                result.Outgoing = histories.Where(m => m.HistoryType == Models.ProductHistoryType.Out && m.ProductId == models[i].ProductId && m.HouseCode == models[i].HouseCode).Sum(m => m.Quantity);
                result.End_Stock = models[i].Stock + models[i].QtyOrder;
                result.UOM = models[i].MasProductData.Unit;

                var history = histories.Where(m => m.ProductId == models[i].ProductId && m.HouseCode == models[i].HouseCode).ToList();
                result.invProductHistories.AddRange(history);

                results.Add(result);
            }

            DataTable product = new DataTable("Product Stock");
            product.Columns.AddRange(new DataColumn[11] {
                                new DataColumn("No"),
                                new DataColumn("Product"),
                                new DataColumn("SKU"),
                                new DataColumn("Status"),
                                new DataColumn("Tenant"),
                                new DataColumn("Warehouse"),
                                new DataColumn("Stok Awal\n" + FilterDateFrom.ToString()),
                                new DataColumn("Incoming"),
                                new DataColumn("Outgoing"),
                                new DataColumn("Stok Akhir\n" + FilterDateTo.ToString()),
                                new DataColumn("UOM")
            });

            DataTable productdetail = new DataTable("Product Histories");
            productdetail.Columns.AddRange(new DataColumn[11] {
                                new DataColumn("Product"),
                                new DataColumn("SKU"),
                                new DataColumn("Status"),
                                new DataColumn("Tenant"),
                                new DataColumn("Warehouse"),
                                new DataColumn("Type"),
                                new DataColumn("Trx. Number"),
                                new DataColumn("Date Time"),
                                new DataColumn("Quantity"),
                                new DataColumn("Stock Akhir"),
                                new DataColumn("UOM")
            });

            foreach (var model in results)
            {
                product.Rows.Add(model.No,
                            model.ProductName,
                            model.SKU,
                            model.ProductCondition,
                            model.Tenant,
                            model.HouseName,
                            model.Start_Stock,
                            model.Incoming,
                            model.Outgoing,
                            model.End_Stock,
                            model.UOM);

                foreach (var item in model.invProductHistories)
                {
                    productdetail.Rows.Add(
                                model.ProductName,
                                model.SKU,
                                model.ProductCondition,
                                model.Tenant,
                                model.HouseName,
                                item.HistoryType,
                                item.TrxNo,
                                item.DatedTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                item.Quantity,
                                item.Stock,
                                model.UOM);
                }
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(product);
                wb.Worksheets.Add(productdetail);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Product Stock "+ FilterHouseCode +".xlsx");
                }
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSerialNumberByTrxNo(string TrxNo, int ProductId)
        {
            var models = await _unitOfWork.SerialNumber.GetAllAsync(
                filter:
                    m => m.ProductId == ProductId &&
                    (m.IncDeliveryOrderProduct.DONumber == TrxNo ||
                    m.OutSalesOrderProduct.OrderId == TrxNo),
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProduct)
                    .Include(m => m.OutSalesOrderProduct)
                );

            if (models.Count > 0)
            {
                return Json(Ok(models));
            }

            return Json(NoContent());
        }
    }
}
