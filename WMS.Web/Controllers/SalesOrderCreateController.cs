using WMS.DataAccess;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WMS.Utility;
using Microsoft.CodeAnalysis;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "Tenant")]
    public class SalesOrderCreateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public SalesOrderCreateController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.SalesOrder.GetAllAsync(
                filter:
                    m => m.Status == 1,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts));

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                    filter:
                        m => m.UserId == UserId);

                model = model.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? OrderId, Guid? TenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var tenants = await _unitOfWork.Tenant.GetAllAsync();
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();
            var salestypes = await _context.MasSalesTypes.AsNoTracking().ToListAsync();
            var platforms = await _context.MasPlatforms.Where(
                m => m.Flag == FlagEnum.Active).AsNoTracking().ToListAsync();
            var stores = await _unitOfWork.Store.GetAllAsync(
                filter:
                    m => m.Flag == FlagEnum.Active);
            var provinsis = await _context.MasProvinsis.AsNoTracking().ToListAsync();
            var kabupatens = await _context.MasKabupatens.AsNoTracking().ToListAsync();
            var kecamatans = await _context.MasKecamatans.AsNoTracking().ToListAsync();
            var kelurahans = await _context.MasKelurahans.AsNoTracking().ToListAsync();

            var model = new SalesOrderCreateViewModel();

            model.outSalesOrder = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten)
                    .Include(m => m.OutSalesOrderProducts).ThenInclude(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutsalesOrderDelivery));

            if (ProfileId == SD.Role_Tenant)
            {
                var usertenant = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                var userwarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                tenants = tenants.Where(m => usertenant.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString())).ToList();
                warehouses = warehouses.Where(m => userwarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }

            model.masProductBundlings = await _unitOfWork.ProductBundling.GetAllAsync(
                filter:
                    m => m.Flag == FlagEnum.Active,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas));

            if (model.outSalesOrder != null)
            {
                model.masProductDatas = await _unitOfWork.Product.GetAllAsync(
                    filter:
                        m => m.TenantId == model.outSalesOrder.TenantId &&
                        m.Flag == FlagEnum.Active,
                    includeProperties:
                        m => m.Include(m => m.InvProductStocks
                        .Where(m => m.HouseCode == model.outSalesOrder.HouseCode)));

                model.masProductBundlings = model.masProductBundlings.Where(m => m.TenantId == model.outSalesOrder.TenantId);

                tenants = tenants.Where(m => m.TenantId == model.outSalesOrder.TenantId).ToList();
                warehouses = warehouses.Where(m => m.HouseCode == model.outSalesOrder.HouseCode).ToList();
                stores = stores.Where(m => m.PlatformId == model.outSalesOrder.PlatformId &&
                        m.TenantId == model.outSalesOrder.TenantId).ToList();

                if (model.outSalesOrder.OutSalesOrderCustomer != null && model.outSalesOrder.OutSalesOrderConsignee != null)
                {
                    ViewData["ProIdCust"] = new SelectList(provinsis, "ProId", "ProName", model.outSalesOrder.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.ProId);
                    ViewData["CustCity"] = new SelectList(kabupatens.Where(m => m.ProId == model.outSalesOrder.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.ProId), "KabId", "KabName", model.outSalesOrder.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.KabId);
                    ViewData["KecCust"] = new SelectList(kecamatans.Where(m => m.KabId == model.outSalesOrder.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.KabId), "KecId", "KecName", model.outSalesOrder.OutSalesOrderCustomer.MasKelurahan.KecId);
                    ViewData["KelCust"] = new SelectList(kelurahans.Where(m => m.KecId == model.outSalesOrder.OutSalesOrderCustomer.MasKelurahan.KecId), "KelId", "KelName", model.outSalesOrder.OutSalesOrderCustomer.KelId);

                    ViewData["ProIdCnee"] = new SelectList(provinsis, "ProId", "ProName", model.outSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.ProId);
                    ViewData["CneeCity"] = new SelectList(kabupatens.Where(m => m.ProId == model.outSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.ProId), "KabId", "KabName", model.outSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.KabId);
                    ViewData["KecCnee"] = new SelectList(kecamatans.Where(m => m.KabId == model.outSalesOrder.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.KabId), "KecId", "KecName", model.outSalesOrder.OutSalesOrderConsignee.MasKelurahan.KecId);
                    ViewData["KelCnee"] = new SelectList(kelurahans.Where(m => m.KecId == model.outSalesOrder.OutSalesOrderConsignee.MasKelurahan.KecId), "KelId", "KelName", model.outSalesOrder.OutSalesOrderConsignee.KelId);
                }
                else
                {
                    if (model.outSalesOrder.OrdSalesType == SD.SOType_Retrieval)
                    {
                        ViewData["ProIdCust"] = new SelectList(provinsis, "ProId", "ProName", model.outSalesOrder.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.ProId);
                        var kabcust = kabupatens.Where(m => m.ProId == model.outSalesOrder.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.ProId).ToList();
                        ViewData["CustCity"] = new SelectList(kabcust, "KabId", "KabName", model.outSalesOrder.MasHouseCode.MasKelurahan.MasKecamatan.KabId);
                        var keccust = kecamatans.Where(m => m.KabId == model.outSalesOrder.MasHouseCode.MasKelurahan.MasKecamatan.KabId).ToList();
                        ViewData["KecCust"] = new SelectList(keccust, "KecId", "KecName", model.outSalesOrder.MasHouseCode.MasKelurahan.KecId);
                        var kelcust = kelurahans.Where(m => m.KecId == model.outSalesOrder.MasHouseCode.MasKelurahan.KecId).ToList();
                        ViewData["KelCust"] = new SelectList(kelcust, "KelId", "KelName", model.outSalesOrder.MasHouseCode.KelId);

                        ViewData["ProIdCnee"] = new SelectList(provinsis, "ProId", "ProName", model.outSalesOrder.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.ProId);
                        ViewData["CneeCity"] = new SelectList(kabcust, "KabId", "KabName", model.outSalesOrder.MasHouseCode.MasKelurahan.MasKecamatan.KabId);
                        ViewData["KecCnee"] = new SelectList(keccust, "KecId", "KecName", model.outSalesOrder.MasHouseCode.MasKelurahan.KecId);
                        ViewData["KelCnee"] = new SelectList(kelcust, "KelId", "KelName", model.outSalesOrder.MasHouseCode.KelId);
                    }
                    else
                    {
                        ViewData["ProIdCust"] = new SelectList(provinsis, "ProId", "ProName");
                        var kabcust = kabupatens.Where(m => m.ProId == provinsis.Select(m => m.ProId).FirstOrDefault()).ToList();
                        ViewData["CustCity"] = new SelectList(kabcust, "KabId", "KabName");
                        var keccust = kecamatans.Where(m => m.KabId == kabupatens.Select(m => m.KabId).FirstOrDefault()).ToList();
                        ViewData["KecCust"] = new SelectList(keccust, "KecId", "KecName");
                        var kelcust = kelurahans.Where(m => m.KecId == kecamatans.Select(m => m.KecId).FirstOrDefault()).ToList();
                        ViewData["KelCust"] = new SelectList(kelcust, "KelId", "KelName");

                        ViewData["ProIdCnee"] = new SelectList(provinsis, "ProId", "ProName");
                        var kabcnee = kabupatens.Where(m => m.ProId == provinsis.Select(m => m.ProId).FirstOrDefault()).ToList();
                        ViewData["CneeCity"] = new SelectList(kabcnee, "KabId", "KabName");
                        var keccnee = kecamatans.Where(m => m.KabId == kabupatens.Select(m => m.KabId).FirstOrDefault()).ToList();
                        ViewData["KecCnee"] = new SelectList(keccnee, "KecId", "KecName");
                        var kelcnee = kelurahans.Where(m => m.KecId == kecamatans.Select(m => m.KecId).FirstOrDefault()).ToList();
                        ViewData["KelCnee"] = new SelectList(kelcnee, "KelId", "KelName");
                    }
                }
            }

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name", model.outSalesOrder?.TenantId);
            ViewData["HouseCode"] = new SelectList(warehouses, "HouseCode", "HouseName", model.outSalesOrder?.HouseCode);
            ViewData["OrdSalesType"] = new SelectList(salestypes, "StyId", "StyName", model.outSalesOrder?.OrdSalesType);
            ViewData["OrdPlatform"] = new SelectList(platforms, "PlatformId", "Name", model.outSalesOrder?.PlatformId);
            ViewData["StoreId"] = new SelectList(stores, "StoreId", "Name", model.outSalesOrder?.StoreId);

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(SalesOrderCreateViewModel model)
        {
            if (model.outSalesOrder == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var result = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == model.outSalesOrder.OrderId &&
                    m.TenantId == model.outSalesOrder.TenantId &&
                    m.Status == SD.FlagSO_Open);

            var update = true;

            if (result == null)
            {
                result = new OutSalesOrder();

                var Code = "SO";
                var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
                var Last = 1.ToString("D4");

                model.outSalesOrder.OrderId = Code + Tanggal + Last;
                var check = await _unitOfWork.SalesOrder.GetAllAsync(
                   filter:
                    m => m.OrderId == model.outSalesOrder.OrderId);

                if (check.Count > 0)
                {
                    int LastCount = int.Parse(check.Max(m => m.OrderId.Substring(Code.Length + Tanggal.Length)));
                    model.outSalesOrder.OrderId = Code + Tanggal + (LastCount + 1).ToString("000#");
                }

                result.OrderId = model.outSalesOrder.OrderId;
                result.TenantId = model.outSalesOrder.TenantId;

                update = false;
            }

            result.OrdSalesType = model.outSalesOrder.OrdSalesType;
            result.HouseCode = model.outSalesOrder.HouseCode;
            result.PlatformId = model.outSalesOrder.PlatformId;
            result.StoreId = model.outSalesOrder.StoreId;

            var StoreName = await _unitOfWork.Store.GetSingleOrDefaultAsync(
                filter:
                    m => m.StoreId == result.StoreId);

            if (StoreName == null)
            {
                TempData["error"] = "Store Notfound!";
                return RedirectToAction("Upsert", new { OrderId = model.outSalesOrder.OrderId, TenantId = model.outSalesOrder.TenantId });
            }

            result.StoreName = StoreName.Name;

            if (update == true)
            {
                _unitOfWork.SalesOrder.Update(result);
                await _unitOfWork.SaveAsync();
                return Json(Ok(new { data = "", message = "Sales Order Updated!" }));
            }
            else
            {
                await _unitOfWork.SalesOrder.AddAsync(result);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Sales Order Created!";
            }

            return RedirectToAction("Upsert", new { OrderId = model.outSalesOrder.OrderId, TenantId = model.outSalesOrder.TenantId });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpsertAddress(SalesOrderCreateViewModel model)
        {
            if (model.outSalesOrder == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var result = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == model.outSalesOrder.OrderId &&
                    m.TenantId == model.outSalesOrder.TenantId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderCustomer)
                    .Include(m => m.OutSalesOrderConsignee));

            var updateCust = true;
            var updateCnee = true;

            if (result.OutSalesOrderCustomer == null)
            {
                result.OutSalesOrderCustomer = new OutSalesOrderCustomer();
                result.OutSalesOrderCustomer.OrderId = model.outSalesOrder.OrderId;

                updateCust = false;
            }

            if (result.OutSalesOrderConsignee == null)
            {
                result.OutSalesOrderConsignee = new OutSalesOrderConsignee();
                result.OutSalesOrderConsignee.OrderId = model.outSalesOrder.OrderId;

                updateCnee = false;
            }

            result.OutSalesOrderCustomer.KTP = model.outSalesOrder.OutSalesOrderCustomer.KTP;
            result.OutSalesOrderCustomer.CustName = model.outSalesOrder.OutSalesOrderCustomer.CustName;
            result.OutSalesOrderCustomer.CustPhone = model.outSalesOrder.OutSalesOrderCustomer.CustPhone;
            result.OutSalesOrderCustomer.CustCity = model.outSalesOrder.OutSalesOrderCustomer.CustCity;
            result.OutSalesOrderCustomer.KelId = model.outSalesOrder.OutSalesOrderCustomer.KelId;
            result.OutSalesOrderCustomer.CustAddress = model.outSalesOrder.OutSalesOrderCustomer.CustAddress;
            result.OutSalesOrderCustomer.CustZipCode = model.outSalesOrder.OutSalesOrderCustomer.CustZipCode;

            result.OutSalesOrderConsignee.CneeName = model.outSalesOrder.OutSalesOrderConsignee.CneeName;
            result.OutSalesOrderConsignee.CneePhone = model.outSalesOrder.OutSalesOrderConsignee.CneePhone;
            if (result.OrdSalesType == SD.SOType_Retrieval)
            {
                result.OutSalesOrderConsignee.CneeName = result.OutSalesOrderCustomer.CustName;
                result.OutSalesOrderConsignee.CneePhone = result.OutSalesOrderCustomer.CustPhone;
            }
            result.OutSalesOrderConsignee.CneeCity = model.outSalesOrder.OutSalesOrderConsignee.CneeCity;
            result.OutSalesOrderConsignee.KelId = model.outSalesOrder.OutSalesOrderConsignee.KelId;
            result.OutSalesOrderConsignee.CneeAddress = model.outSalesOrder.OutSalesOrderConsignee.CneeAddress;
            result.OutSalesOrderConsignee.OrdZipCode = model.outSalesOrder.OutSalesOrderConsignee.OrdZipCode;

            if (updateCust == true)
            {
                _unitOfWork.SalesOrderCustomer.Update(result.OutSalesOrderCustomer);
            }
            else
            {
                await _unitOfWork.SalesOrderCustomer.AddAsync(result.OutSalesOrderCustomer);
            }

            if (updateCnee == true)
            {
                _unitOfWork.SalesOrderConsignee.Update(result.OutSalesOrderConsignee);
            }
            else
            {
                await _unitOfWork.SalesOrderConsignee.AddAsync(result.OutSalesOrderConsignee);
            }

            await _unitOfWork.SaveAsync();

            if (updateCnee == true)
            {
                return Json(Ok(new { data = "", message = "Sales Order Updated!" }));
            }

            TempData["success"] = "Sales Order Updated!";
            return RedirectToAction("Upsert", new { OrderId = model.outSalesOrder.OrderId, TenantId = model.outSalesOrder.TenantId });
        }

        [HttpPost]
        public async Task<JsonResult> AddProductByProductId(bool Type, OutSalesOrderProduct model)
        {
            if (model == null)
            {
                return Json(BadRequest(new { data = "", message = "Invalid Modelstate!" }));
            }

            var result = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == model.OrderId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts).ThenInclude(m => m.MasProductData));

            if (result == null)
            {
                return Json(BadRequest(new { data = "", message = "Invalid Modelstate!" }));
            }

            var resultproduct = result.OutSalesOrderProducts.SingleOrDefault(
                m => m.ProductId == model.ProductId);

            var product = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                    filter:
                        m => m.ProductId == model.ProductId &&
                        m.TenantId == result.TenantId,
                    includeProperties:
                        m => m.Include(m => m.InvProductStocks.Where(m => m.HouseCode == result.HouseCode)));

            if (product == null)
            {
                return Json(BadRequest(new { data = result, message = "Produk gagal ditambahkan!" }));
            }

            if (product.InvProductStocks.Count < 1)
            {
                return Json(BadRequest(new { data = result, message = "Produk gagal ditambahkan!" }));
            }

            var update = true;

            if (resultproduct == null)
            {
                if (product.InvProductStocks.Sum(m => m.Stock) < 1)
                {
                    return Json(BadRequest(new { data = result, message = "Produk gagal ditambahkan!" }));
                }
                resultproduct = new OutSalesOrderProduct();

                resultproduct.OrderId = result.OrderId;
                resultproduct.ProductId = product.ProductId;
                resultproduct.UnitPrice = product.SellingPrice;
                resultproduct.Quantity = 1;

                update = false;
            }
            else
            {
                if (Type)
                {
                    if (resultproduct.Quantity + 1 > product.InvProductStocks.Sum(m => m.Stock))
                    {
                        return Json(BadRequest(new { data = result, message = "Over Quantity!" }));
                    }
                    resultproduct.Quantity += 1;
                }
                else
                {
                    if (model.Quantity > product.InvProductStocks.Sum(m => m.Stock))
                    {
                        return Json(BadRequest(new { data = result, message = "Over Quantity!" }));
                    }
                    resultproduct.Quantity = model.Quantity;
                }
            }

            resultproduct.SubTotal = resultproduct.Quantity * resultproduct.UnitPrice;
            resultproduct.TotalWeight = resultproduct.Quantity * product.ActualWeight;
            if (product.VolWight > product.ActualWeight)
            {
                resultproduct.TotalWeight = resultproduct.Quantity * product.VolWight;
            }

            if (update == true)
            {
                if (resultproduct.Quantity < 1)
                {
                    foreach (var serial in resultproduct.IncSerialNumbers)
                    {
                        serial.OrdProductId = null;
                        serial.Status = SD.FlagSerialNumber_IN;

                        _unitOfWork.SerialNumber.Update(serial);
                    }

                    _unitOfWork.SalesOrderProduct.Remove(resultproduct);
                }
                else
                {
                    _unitOfWork.SalesOrderProduct.Update(resultproduct);
                }
            }
            else
            {
                await _unitOfWork.SalesOrderProduct.AddAsync(resultproduct);
            }

            await _unitOfWork.SaveAsync();

            result = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts.OrderByDescending(m => m.OrdProductId)).ThenInclude(m => m.MasProductData),
                filter:
                    m => m.OrderId == model.OrderId &&
                    m.Status == SD.FlagSO_Open);

            return Json(Ok(new { data = result, message = "Product berhasil diupdate!" }));
        }

        [HttpPost]
        public async Task<JsonResult> AddProductByBundlingId(string OrderId, string BundlingId, Guid TenantId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == OrderId &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts).ThenInclude(m => m.MasProductData));

            if (model == null)
            {
                return Json(BadRequest(new { data = "", message = "Invalid Modelstate!" }));
            }

            var bundling = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                filter:
                    m => m.BundlingId == BundlingId &&
                    m.TenantId == TenantId &&
                    m.Flag == FlagEnum.Active,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas)
                    .ThenInclude(m => m.MasProductData)
                    .ThenInclude(m => m.InvProductStocks.Where(m => m.HouseCode == model.HouseCode)));

            if (bundling == null)
            {
                return Json(BadRequest(new { data = "", message = "Invalid Modelstate!" }));
            }

            foreach (var item in bundling.MasProductBundlingDatas)
            {
                var modelproduct = model.OutSalesOrderProducts.SingleOrDefault(m => m.ProductId == item.ProductId);
                
                if (item.MasProductData.InvProductStocks.Count < 1)
                {
                    return Json(BadRequest(new { data = model, message = "Stok habis!" }));
                }

                if (modelproduct != null)
                {
                    if (modelproduct.Quantity + item.Quantity > item.MasProductData.InvProductStocks.Sum(m => m.Stock))
                    {
                        return Json(BadRequest(new { data = model, message = item.MasProductData.ProductName + " Over Quantity!" }));
                    }
                    modelproduct.Quantity += item.Quantity;
                    modelproduct.SubTotal = modelproduct.Quantity * modelproduct.UnitPrice;
                    modelproduct.TotalWeight = modelproduct.Quantity * item.MasProductData.ActualWeight;
                    if (item.MasProductData.VolWight > item.MasProductData.ActualWeight)
                    {
                        modelproduct.TotalWeight = modelproduct.Quantity * item.MasProductData.VolWight;
                    }

                    _unitOfWork.SalesOrderProduct.Update(modelproduct);
                }
                else
                {
                    if (item.Quantity > item.MasProductData.InvProductStocks.Sum(m => m.Stock))
                    {
                        return Json(BadRequest(new { data = model, message = item.MasProductData.ProductName + " Over Quantity!" }));
                    }

                    modelproduct = new OutSalesOrderProduct();

                    modelproduct.OrderId = model.OrderId;
                    modelproduct.ProductId = (int)item.ProductId;
                    modelproduct.Quantity = item.Quantity;
                    modelproduct.UnitPrice = item.MasProductData.SellingPrice;
                    modelproduct.SubTotal = modelproduct.Quantity * modelproduct.UnitPrice;
                    modelproduct.TotalWeight = modelproduct.Quantity * item.MasProductData.ActualWeight;
                    if (item.MasProductData.VolWight > item.MasProductData.ActualWeight)
                    {
                        modelproduct.TotalWeight = modelproduct.Quantity * item.MasProductData.VolWight;
                    }

                    await _unitOfWork.SalesOrderProduct.AddAsync(modelproduct);
                }
            }

            await _unitOfWork.SaveAsync();

            model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts.OrderByDescending(m => m.OrdProductId)).ThenInclude(m => m.MasProductData),
                filter:
                    m => m.OrderId == model.OrderId &&
                    m.Status == SD.FlagSO_Open);

            return Json(Ok(new { data = model, message = "Produk berhasil diupdate!" }));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string OrderId, Guid TenantId)
        {
            var model = new SalesOrderCreateViewModel();
            model.outSalesOrder = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                            .ThenInclude(m => m.InvProductStocks)
                    .Include(m => m.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutsalesOrderDelivery));

            if (model.outSalesOrder == null)
            {
                return RedirectToAction("Upsert");
            }

            if (model.outSalesOrder.OutSalesOrderCustomer == null || model.outSalesOrder.OutSalesOrderConsignee == null)
            {
                TempData["warning"] = "Alamat belum lengkap!";
                return RedirectToAction("Upsert", new { OrderId = OrderId, TenantId = TenantId});
            }

            if (model.outSalesOrder.OutSalesOrderProducts.Count < 1)
            {
                TempData["warning"] = "Please add some product!";
                return RedirectToAction("Upsert", new { OrderId = OrderId, TenantId = TenantId });
            }

            foreach (var item in model.outSalesOrder.OutSalesOrderProducts)
            {
                var productstock = item.MasProductData.InvProductStocks.Where(m => m.HouseCode == model.outSalesOrder.HouseCode);

                if (productstock.Count() < 1)
                {
                    TempData["error"] = item.MasProductData.ProductName + " Over Quantity!";
                    return RedirectToAction("Upsert", new { OrderId = OrderId, TenantId = TenantId });
                }

                if (item.Quantity > productstock.Sum(m => m.Stock))
                {
                    TempData["error"] = item.MasProductData.ProductName + " Over Quantity!";
                    return RedirectToAction("Upsert", new { OrderId = OrderId, TenantId = TenantId });
                }
            }

            var couriers = await _context.MasSalesCouriers.AsNoTracking().ToListAsync();
            ViewData["OrdCourier"] = new SelectList(couriers, "Id", "Name");

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(SalesOrderCreateViewModel model)
        {
            if (model.outSalesOrder == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var result = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == model.outSalesOrder.OrderId &&
                    m.TenantId == model.outSalesOrder.TenantId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.OutsalesOrderDelivery)
                    .Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                            .ThenInclude(m => m.InvProductStocks));

            if (model.outSalesOrder == null)
            {
                return RedirectToAction("Upsert");
            }

            var update = true;

            if (result.OutsalesOrderDelivery == null)
            {
                result.OutsalesOrderDelivery = new OutsalesOrderDelivery();
                result.OutsalesOrderDelivery.OrderId = model.outSalesOrder.OrderId;

                update = false;
            }

            result.OutsalesOrderDelivery.GrandWeight = result.OutSalesOrderProducts.Sum(m => m.TotalWeight);
            result.OutsalesOrderDelivery.OrdCourier = model.outSalesOrder.OutsalesOrderDelivery.OrdCourier;
            result.OutsalesOrderDelivery.OrdCourierService = model.outSalesOrder.OutsalesOrderDelivery.OrdCourierService;
            result.OutsalesOrderDelivery.ShippingCost = model.outSalesOrder.OutsalesOrderDelivery.ShippingCost;
            result.OutsalesOrderDelivery.OrdTax = model.outSalesOrder.OutsalesOrderDelivery.OrdTax;

            if (update == true)
            {
                _unitOfWork.SalesOrderDelivery.Update(result.OutsalesOrderDelivery);
            }
            else
            {
                await _unitOfWork.SalesOrderDelivery.AddAsync(result.OutsalesOrderDelivery);
            }

            //save delivery
            await _unitOfWork.SaveAsync();

            foreach (var resultproduct in result.OutSalesOrderProducts)
            {
                var productstock = resultproduct.MasProductData.InvProductStocks.SingleOrDefault(m => m.HouseCode == result.HouseCode);

                if (productstock == null)
                {
                    TempData["error"] = resultproduct.MasProductData.ProductName + " tidak terdaftar dalam gudang!";
                    return RedirectToAction("Upsert", new { OrderId = model.outSalesOrder.OrderId, TenantId = result.TenantId });
                }

                if (resultproduct.Quantity > productstock.Stock)
                {
                    TempData["error"] = resultproduct.MasProductData.ProductName + " Over Quantity!";
                    return RedirectToAction("Upsert", new { OrderId = result.OrderId, TenantId = result.TenantId });
                }

                productstock.Stock = productstock.Stock - resultproduct.Quantity;
                productstock.QtyOrder = productstock.QtyOrder + resultproduct.Quantity;
                _unitOfWork.ProductStock.Update(productstock);

                resultproduct.Flag = SD.FlagSOProduct_Booked;
                _unitOfWork.SalesOrderProduct.Update(resultproduct);
            }

            result.Status = SD.FlagSO_Ordered;
            result.OrderBy = User.FindFirst("UserName")?.Value;
            result.DateOrdered = DateTime.Now;

            _unitOfWork.SalesOrder.Update(result);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Sales Order created successfully!";
            return RedirectToAction("Detail", "SalesOrderList", new { OrderId = result.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSalesOrder(string OrderId, Guid TenantId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagSO_Open,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts)
                    .Include(m => m.OutSalesOrderCustomer)
                    .Include(m => m.OutSalesOrderConsignee)
                    .Include(m => m.OutsalesOrderDelivery));

            if (model == null)
            {
                TempData["error"] = "Sales Order Notfund!";
                return RedirectToAction("Index");
            }

            if (model.OutSalesOrderProducts.Count > 0)
            {
                _unitOfWork.SalesOrderProduct.RemoveRange(model.OutSalesOrderProducts);
            }
            if (model.OutSalesOrderCustomer != null)
            {
                _unitOfWork.SalesOrderCustomer.Remove(model.OutSalesOrderCustomer);
            }
            if (model.OutSalesOrderConsignee != null)
            {
                _unitOfWork.SalesOrderConsignee.Remove(model.OutSalesOrderConsignee);
            }
            if (model.OutsalesOrderDelivery != null)
            {
                _unitOfWork.SalesOrderDelivery.Remove(model.OutsalesOrderDelivery);
            }

            _unitOfWork.SalesOrder.Remove(model);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Sales Order Deleted Successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesOrderProductByOrderIdByProductId(string OrderId, int ProductId)
        {
            var model = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductData),
                filter:
                    m => m.OrderId == OrderId &&
                    m.ProductId == ProductId);

            if (model == null)
            {
                return Json(BadRequest(new { data = string.Empty, message = "Produk tidak ditemukan!" }));
            }

            return Json(Ok(new { data = model, message = "Sukses!" }));
        }

        [HttpGet]
        public JsonResult GetCourierService(int IdCourier, int Origin, int Destination, float Weight)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var url = "";

            if (IdCourier == 2)
            {
                url = "https://api2.ptncs.com/bot/generalpublishrate/demo/demo?origin=" + Origin + "&destination=" + Destination + "&weight=" + Weight;
            }

            var result = new WebClient().DownloadString(url);

            return Json(Ok(new { data = result, message = "Success!" }));
        }
        
    }
}
