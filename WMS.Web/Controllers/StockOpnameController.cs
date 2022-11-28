using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Utility;
using ProductHistoryType = WMS.Models.ProductHistoryType;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class StockOpnameController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockOpnameController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var models = await _unitOfWork.StockOpname.GetAllAsync(
                filter:
                    m => m.Status != SD.FlagOpname_Done,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.InvStockOpnameProducts));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                models = models.Where(m => m.HouseCode == HouseCode).ToList();
            }

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? OpnameId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var masHouseCodes = await _unitOfWork.HouseCode.GetAllAsync();
            var masDataTenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                masHouseCodes = masHouseCodes.Where(m => m.HouseCode == HouseCode).ToList();
                masDataTenants = masDataTenants.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
            }

            ViewData["HouseCode"] = new SelectList(masHouseCodes, "HouseCode", "HouseName");
            ViewData["TenantId"] = new SelectList(masDataTenants, "TenantId", "Name");

            if (OpnameId != null)
            {
                var model = await _unitOfWork.StockOpname.GetSingleOrDefaultAsync(
                filter:
                    m => m.OpnameId == OpnameId,
                includeProperties:
                    m => m.Include(m => m.InvStockOpnameProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));

                if (model != null)
                {
                    return View(model);
                }
            }

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(string? OpnameId, InvStockOpname model)
        {
            var ProductId = model.InvStockOpnameProducts[0].ProductId;

            if (ProductId == null)
            {
                TempData["error"] = "Produk tidak ditemukan!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var DOProducts = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                filter:
                    m => m.ProductId == ProductId &&
                    m.IncDeliveryOrder.HouseCode == model.HouseCode,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderArrivals));

            var DOProductsDO = DOProducts.Where(m => m.Status == SD.FlagDOProduct_Booked).ToList();
            var DOProductsAR = DOProducts.Where(m => m.Status == SD.FlagDOProduct_Arrived).ToList();

            if (DOProductsDO.Any(m => m.IncDeliveryOrderArrivals != null))
            {
                TempData["error"] = "Masih terdapat produk belum ter PUT";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (DOProductsAR.Count > 0)
            {
                TempData["error"] = "Masih terdapat produk belum ter PUT";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var SOProducts = await _unitOfWork.SalesOrderProduct.GetAllAsync(
                filter:
                    m => m.ProductId == ProductId &&
                    m.OutSalesOrder.HouseCode == model.HouseCode,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder.OutSalesOrderAssign));

            if (SOProducts.Any(m => m.OutSalesOrder.FlagPick == 1 && m.OutSalesOrder.OutSalesOrderAssign.Flag == 1))
            {
                TempData["error"] = "Masih terdapat produk SO dengan status PICKED";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (OpnameId == null)
            {
                var Code = "OP" + model.HouseCode;
                var Tanggal = DateTime.Now.ToString("yyMMdd");
                var Last = 1.ToString("D2");

                OpnameId = Code + Tanggal + Last;
                if (await _unitOfWork.StockOpname.AnyAsync(m => m.OpnameId == OpnameId))
                {
                    var checker = await _unitOfWork.StockOpname.GetAllAsync(m => m.OpnameId.Contains(Code + Tanggal));
                    int LastCount = int.Parse(checker.Max(m => m.OpnameId.Substring(Code.Length + Tanggal.Length)));
                    OpnameId = Code + Tanggal + (LastCount + 1).ToString("0#");
                }

                model.OpnameId = OpnameId;
                model.DateCreated = DateTime.Now;
                model.CreatedBy = User.FindFirst("FirstName")?.Value + " " + User.FindFirst("LastName")?.Value;
                model.Status = SD.FlagOpname_Open;
                model.InvStockOpnameProducts = null;

                await _unitOfWork.StockOpname.AddAsync(model);
                await _unitOfWork.SaveAsync();
            }

            model = await _unitOfWork.StockOpname.GetSingleOrDefaultAsync(
               filter:
                   m => m.OpnameId == OpnameId,
               includeProperties:
                   m => m.Include(m => m.InvStockOpnameProducts));

            if (model.Status != SD.FlagOpname_Open)
            {
                TempData["error"] = "Manifest Stock Opname sudah selesai!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (model.InvStockOpnameProducts.Any(m => m.Status == SD.FlagOpnamePro_Open))
            {
                TempData["error"] = "Masih terdapat produk belum selesai di-Opname!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (model.InvStockOpnameProducts.Any(m => m.ProductId == ProductId))
            {
                TempData["error"] = "Produk sudah ter-Opname!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var stock = await _unitOfWork.ProductStock.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductId == ProductId &&
                    m.HouseCode == model.HouseCode);

            if (stock == null)
            {
                TempData["error"] = "Stok tidak terdaftar pada warehouse ini!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var product = new InvStockOpnameProduct()
            {
                ProductId = ProductId,
                OpnameId = OpnameId,
                DateOpname = DateTime.Now,
                SystemQty = stock.Stock + stock.QtyOrder,
                Status = SD.FlagOpnamePro_Open
            };

            await _unitOfWork.StockOpnameProduct.AddAsync(product);
            await _unitOfWork.SaveAsync();

            return RedirectToAction("Upsert", new { OpnameId = OpnameId });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> OpnameProductDone(string OpnameId, InvStockOpnameProduct model)
        {
            var result = await _unitOfWork.StockOpname.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OpnameId == OpnameId,
                includeProperties:
                    m => m.Include(m => m.InvStockOpnameProducts));

            if (result == null)
            {
                TempData["error"] = "Stock Opname tidak ditemukan!";
                return RedirectToAction("Index");
            }

            var resultproduct = result.InvStockOpnameProducts.SingleOrDefault(
                m => m.OpnameProductId == model.OpnameProductId);

            if (resultproduct == null)
            {
                TempData["error"] = "Produk tidak ditemukan!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (resultproduct.Status != SD.FlagOpnamePro_Open)
            {
                TempData["error"] = "Produk sudah ter-Opname!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var DOProducts = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                filter:
                    m => m.ProductId == resultproduct.ProductId &&
                    m.IncDeliveryOrder.HouseCode == result.HouseCode,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderArrivals));

            var DOProductsDO = DOProducts.Where(m => m.Status == SD.FlagDOProduct_Booked).ToList();
            var DOProductsAR = DOProducts.Where(m => m.Status == SD.FlagDOProduct_Arrived).ToList();

            if (DOProductsDO.Any(m => m.IncDeliveryOrderArrivals != null))
            {
                TempData["error"] = "Masih terdapat produk belum ter PUT";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (DOProductsAR.Count > 0)
            {
                TempData["error"] = "Masih terdapat produk belum ter PUT";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var SOProducts = await _unitOfWork.SalesOrderProduct.GetAllAsync(
                filter:
                    m => m.ProductId == resultproduct.ProductId &&
                    m.OutSalesOrder.HouseCode == result.HouseCode,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder.OutSalesOrderAssign));

            if (SOProducts.Any(m => m.OutSalesOrder.FlagPick == 1 && m.OutSalesOrder.OutSalesOrderAssign.Flag == 1))
            {
                TempData["error"] = "Masih terdapat produk SO dengan status PICKED";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            var stock = await _unitOfWork.ProductStock.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.ProductId == resultproduct.ProductId &&
                    m.HouseCode == result.HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasProductData));

            if (resultproduct.SystemQty != stock.Stock + stock.QtyOrder)
            {
                TempData["error"] = "Stock tidak sesuai, harus diupdate!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            resultproduct.DateOpname = DateTime.Now;
            resultproduct.ExpiredQty = model.ExpiredQty;
            resultproduct.BrokenQty = model.BrokenQty;
            resultproduct.StockQty = model.StockQty;
            resultproduct.DiscrepancyQty = (model.StockQty + model.ExpiredQty + model.BrokenQty) - resultproduct.SystemQty;
            //resultproduct.BrokenDescription = model.BrokenDescription;
            resultproduct.Status = SD.FlagOpnamePro_Done;
            _unitOfWork.StockOpnameProduct.Update(resultproduct);

            if (resultproduct.SystemQty != resultproduct.StockQty)
            {
                var productHistory = new InvProductHistory();
                productHistory.ProductId = resultproduct.ProductId;
                productHistory.HouseCode = result.HouseCode;
                productHistory.TrxNo = result.OpnameId;
                productHistory.Interest = "Stock Opname";
                productHistory.DatedTime = DateTime.Now;
                productHistory.UserBy = User.FindFirst("UserName")?.Value;

                var putaways = await _unitOfWork.PutAway.GetAllAsync(
                        disableTracking:
                            false,
                        filter:
                            m => m.IncDeliveryOrderArrival.IncDeliveryOrderProduct.ProductId == resultproduct.ProductId &&
                            m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode == result.HouseCode,
                        includeProperties:
                            m => m.Include(m => m.IncDeliveryOrderArrival.IncDeliveryOrderProduct)
                            .Include(m => m.InvStorageCode.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow),
                        orderBy:
                            m => m.OrderByDescending(m => m.DatePutaway));

                if (putaways.Count < 1)
                {
                    TempData["error"] = "Stock kosong!";
                    return RedirectToAction("Upsert", new { OpnameId = OpnameId });
                }

                if (resultproduct.StockQty < resultproduct.SystemQty)
                {
                    putaways = putaways.Where(m => m.QtyStock > 0).ToList();

                    var counterQty = resultproduct.SystemQty - resultproduct.StockQty;

                    if (putaways.Sum(m => m.QtyStock) < counterQty)
                    {
                        TempData["error"] = "Masih terdapat produk belum ter PUT!";
                        return RedirectToAction("Upsert", new { OpnameId = OpnameId });
                    }

                    if (putaways.Sum(m => m.InvStorageCode.Qty) < counterQty)
                    {
                        TempData["error"] = "Masih terdapat produk belum ter PUT Storage!";
                        return RedirectToAction("Upsert", new { OpnameId = OpnameId });
                    }

                    for (int i = 0; i < putaways.Count; i++)
                    {
                        var tempsku1 = 0;

                        if (putaways[i].QtyStock - counterQty <= 0)
                        {
                            tempsku1 = putaways[i].QtyStock;
                            putaways[i].QtyStock = 0;
                            putaways[i].InvStorageCode.Qty = putaways[i].InvStorageCode.Qty - tempsku1;
                        }
                        else
                        {
                            tempsku1 = counterQty;
                            putaways[i].QtyStock = putaways[i].QtyStock - counterQty;
                            putaways[i].InvStorageCode.Qty = putaways[i].InvStorageCode.Qty - counterQty;
                        }

                        counterQty = counterQty - tempsku1;

                        _unitOfWork.PutAway.Update(putaways[i]);
                        _unitOfWork.StorageCode.Update(putaways[i].InvStorageCode);

                        resultproduct.Log = resultproduct.Log + putaways[i].Id.ToString() + "=-" + tempsku1.ToString() + ";";
                        _unitOfWork.StockOpnameProduct.Update(resultproduct);

                        productHistory.Note = resultproduct.Log;

                        stock.Stock = stock.Stock - tempsku1;

                        if (stock.Stock < 0)
                        {
                            TempData["error"] = "Product Stock minus!";
                            return RedirectToAction("Upsert", new { OpnameId = OpnameId });
                        }
                        _unitOfWork.ProductStock.Update(stock);

                        if (counterQty <= 0)
                        {
                            break;
                        }
                    }

                    productHistory.HistoryType = ProductHistoryType.Op;
                    productHistory.Quantity = resultproduct.StockQty - resultproduct.SystemQty;
                }
                else
                {
                    var putaway = putaways.FirstOrDefault();
                    var tempsku2 = resultproduct.StockQty - resultproduct.SystemQty;

                    putaway.QtyStock = putaway.QtyStock + tempsku2;
                    _unitOfWork.PutAway.Update(putaway);

                    putaway.InvStorageCode.Qty = putaway.InvStorageCode.Qty + tempsku2;
                    _unitOfWork.StorageCode.Update(putaway.InvStorageCode);

                    stock.Stock = stock.Stock + tempsku2;
                    _unitOfWork.ProductStock.Update(stock);

                    resultproduct.Log = resultproduct.Log + putaway.Id.ToString() + "=+" + tempsku2.ToString() + ";";
                    _unitOfWork.StockOpnameProduct.Update(resultproduct);


                    productHistory.Note = resultproduct.Log;
                    productHistory.HistoryType = ProductHistoryType.Op;
                    productHistory.Quantity = resultproduct.StockQty - resultproduct.SystemQty;
                }

                productHistory.Stock = stock.Stock + stock.QtyOrder;
                await _unitOfWork.ProductHistory.AddAsync(productHistory);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "StockOpname Updated Successfully!";
            return RedirectToAction("Upsert", new { OpnameId = OpnameId });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> OpnameDone(string OpnameId, string HouseCode)
        {
            var model = await _unitOfWork.StockOpname.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OpnameId == OpnameId &&
                    m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.InvStockOpnameProducts));

            if (model == null)
            {
                TempData["error"] = "Manifest Stock Opname tidak ditemukan!";
                return RedirectToAction("Index");
            }

            if (model.Status == SD.FlagOpname_Done)
            {
                TempData["error"] = "Manifest Stock Opname sudah terbit!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (model.InvStockOpnameProducts.Count < 1)
            {
                TempData["error"] = "Produk tidak boleh kosong!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            if (!model.InvStockOpnameProducts.All(m => m.Status == SD.FlagOpnamePro_Done))
            {
                TempData["error"] = "Masih terdapat produk belum ter-Opname!";
                return RedirectToAction("Upsert", new { OpnameId = OpnameId });
            }

            model.Status = SD.FlagOpname_Done;
            model.DateOpname = DateTime.Now;
            _unitOfWork.StockOpname.Update(model);
            
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Stock Opname telah diterbitkan!";
            return RedirectToAction("Detail", "StockOpnameList", new { OpnameId = OpnameId });
        } 
    }
}