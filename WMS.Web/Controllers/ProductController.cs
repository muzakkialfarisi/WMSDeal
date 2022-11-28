#nullable disable
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.Utility;
using Microsoft.CodeAnalysis;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "Tenant")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork)
        {
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.Product.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.InvStorageSize)
                    .Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageZone)
                    .Include(m => m.MasProductPackaging)
                    .Include(m => m.MasDataTenant.SecUserTenants),
                orderBy:
                    m => m.OrderBy(m => m.Flag));

            if (ProfileId == SD.Role_Tenant)
            {
                var UserTenants = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId == UserId);

                model = model.Where(m => m.MasDataTenant.SecUserTenants.Any(m => m.UserId == UserId)).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid ProductCode, Guid TenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductCode == ProductCode &&
                    m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasProductPackaging)
                    .Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageSize)
                    .Include(m => m.InvStorageZone)
                    .Include(m => m.MasDataTenant.MasDataTenantWarehouses));

            if (model == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Index");
            }

            if (ProfileId == SD.Role_Tenant)
            {
                var UserTenants = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId == UserId);

                if (!UserTenants.Any(m => m.TenantId == model.TenantId))
                {
                    TempData["error"] = "Product Notfound!";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? ProductId, Guid? TenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductId == ProductId &&
                    m.TenantId == TenantId);

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.SecUserTenants));

            if (ProfileId == SD.Role_Tenant)
            {
                tenants = tenants.Where(m => m.SecUserTenants.Any(m => m.UserId == UserId)).ToList();
            }

            var suppliers = await _unitOfWork.Supplier.GetAllAsync(
                filter:
                    m => m.TenantId == tenants.Select(m => m.TenantId).FirstOrDefault());

            if (model != null)
            {
                tenants = tenants.Where(m => m.TenantId == model.TenantId).ToList();
                suppliers = suppliers.Where(m => m.TenantId == model.TenantId).ToList();
            }

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name", model?.TenantId);
            ViewData["SupplierId"] = new SelectList(suppliers, "Name", "Name", model?.Supplier);

            ViewData["ZoneCode"] = new SelectList(await _unitOfWork.StorageZone.GetAllAsync(), "ZoneCode", "ZoneName", model?.ZoneCode);
            ViewData["CategoryId"] = new SelectList(await _unitOfWork.StorageCategory.GetAllAsync(), "StorageCategoryId", "StorageCategoryName", model?.CategoryId);
            ViewData["PackagingId"] = new SelectList(await _unitOfWork.ProductPackaging.GetAllAsync(), "PackagingId", "PackagingName", model?.PackagingId);
            ViewData["Unit"] = new SelectList(await _unitOfWork.ProductUnit.GetAllAsync(), "UnitName", "UnitName", model?.Unit);
            ViewData["CargoPriorityCode"] = new SelectList(await _unitOfWork.ProductPriority.GetAllAsync(), "CargoPriorityCode", "CargoPriorityName", model?.CargoPriorityCode);
            ViewData["TypeOfRepackCode"] = new SelectList(await _unitOfWork.ProductTypeOfRepack.GetAllAsync(), "RepackCode", "RepackName", model?.TypeOfRepackCode);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int? ProductId, Guid? TenantId, MasProductData model)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m=> m.SecUserTenants));

            if (ProfileId == SD.Role_Tenant)
            {
                tenants = tenants.Where(m => m.SecUserTenants.Any(m => m.UserId == UserId)).ToList();
            }

            var suppliers = await _unitOfWork.Supplier.GetAllAsync(
                filter:
                    m => m.TenantId == tenants.Select(m => m.TenantId).FirstOrDefault());

            var result = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.ProductId == ProductId &&
                    m.TenantId == TenantId);

            var update = true;

            if (result == null)
            {
                result = new MasProductData();
                result.TenantId = model.TenantId;

                update = false;
            }
            else
            {
                tenants = tenants.Where(m => m.TenantId == model.TenantId).ToList();
                suppliers = suppliers.Where(m => m.TenantId == model.TenantId).ToList();
            }

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name", model?.TenantId);
            ViewData["SupplierId"] = new SelectList(suppliers, "Name", "Name", model?.Supplier);

            ViewData["ZoneCode"] = new SelectList(await _unitOfWork.StorageZone.GetAllAsync(), "ZoneCode", "ZoneName", model?.ZoneCode);
            ViewData["CategoryId"] = new SelectList(await _unitOfWork.StorageCategory.GetAllAsync(), "StorageCategoryId", "StorageCategoryName", model?.CategoryId);
            ViewData["PackagingId"] = new SelectList(await _unitOfWork.ProductPackaging.GetAllAsync(), "PackagingId", "PackagingName", model?.PackagingId);
            ViewData["Unit"] = new SelectList(await _unitOfWork.ProductUnit.GetAllAsync(), "UnitName", "UnitName", model?.Unit);
            ViewData["CargoPriorityCode"] = new SelectList(await _unitOfWork.ProductPriority.GetAllAsync(), "CargoPriorityCode", "CargoPriorityName", model?.CargoPriorityCode);
            ViewData["TypeOfRepackCode"] = new SelectList(await _unitOfWork.ProductTypeOfRepack.GetAllAsync(), "RepackCode", "RepackName", model?.TypeOfRepackCode);


            var SKU = model.SKU.Trim().ToUpper().ToString();

            var skuchecker = await _unitOfWork.Product.GetAllAsync(
                filter: m => m.SKU == SKU &&
                    m.TenantId == model.TenantId &&
                    m.ProductId != result.ProductId);
            if (skuchecker.Count > 0)
            {
                TempData["error"] = "SKU Already Used!";
                return View(model);
            }

            if (!await _unitOfWork.StorageSize.AnyAsync(e => e.SizeCode == model.SizeCode))
            {
                TempData["error"] = "Product Size Not Found!";
                return View(model);
            }

            if (SKU == SD.AutoGenerated.Trim().ToUpper().ToString())
            {
                var First = "WMS-";
                var Last = 1.ToString("D7");

                SKU = First + Last;
                if (await _unitOfWork.Product.AnyAsync(filter: m => m.SKU == SKU && m.TenantId == model.TenantId))
                {
                    var product = await _unitOfWork.Product.GetAllAsync(filter: m => m.TenantId == model.TenantId && m.SKU.Contains(First));
                    int LastCount = int.Parse(product.Max(m => m.SKU.Substring(First.Length)));
                    SKU = First + (LastCount + 1).ToString("000000#");
                }
            }

            var BeautyPict = UploadPhoto(model.FormBeautyPicture, result.ProductCode, "BeautyPicture");
            var CloseUpPict = UploadPhoto(model.FormCloseUpPicture, result.ProductCode, "CloseUpPicture");

            var PurchasePrice = model.PurchasePrice.ToString().Replace(",", "");
            var SellingPrice = model.SellingPrice.ToString().Replace(",", "");

            result.ProductName = model.ProductName;
            result.FriendlyName = model.FriendlyName;
            result.BrandName = model.BrandName;
            result.SKU = SKU;
            result.ProductCondition = model.ProductCondition;
            result.CategoryId = model.CategoryId;
            result.PackagingId = model.PackagingId;
            result.Description = model.Description;
            result.Supplier = model.Supplier;
            result.Unit = model.Unit;
            result.Ipb = model.Ipb;
            result.Storageperiod = model.Storageperiod;
            result.SafetyStock = model.SafetyStock;
            result.PurchasePrice = float.Parse(PurchasePrice);
            result.SellingPrice = float.Parse(SellingPrice);
            result.Resellable = model.Resellable;
            result.ResellablePrice = model.ResellablePrice;
            result.RePackaging = model.RePackaging;
            result.TypeOfRepackCode = model.TypeOfRepackCode;
            result.ProductLevel = model.ProductLevel;
            result.SerialNumber = model.SerialNumber;
            result.ZoneCode = model.ZoneCode;
            result.StorageMethod = model.StorageMethod;
            result.CargoPriorityCode = model.CargoPriorityCode;
            result.Panjang = model.Panjang;
            result.Lebar = model.Lebar;
            result.Tinggi = model.Tinggi;
            result.SizeCode = model.SizeCode;
            result.VolWight = model.VolWight;
            result.ActualWeight = model.ActualWeight;
            result.Flag = model.Flag;

            if (update == true)
            {
                if (BeautyPict != "default-product.jpg")
                {
                    result.BeautyPicture = BeautyPict;
                }
                if (CloseUpPict != "default-product.jpg")
                {
                    result.CloseUpPicture = CloseUpPict;
                }

                _unitOfWork.Product.Update(result);
                TempData["success"] = "Product Updated Successfully";
            }
            else
            {
                result.BeautyPicture = BeautyPict;
                result.CloseUpPicture = CloseUpPict;

                await _unitOfWork.Product.AddAsync(result);
                TempData["success"] = "Product Created Successfully";
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction("Upsert", new { ProductId = result.ProductId, TenantId = result.TenantId });
        }

        private string UploadPhoto(IFormFile formFile, Guid ProductCode, string Type)
        {
            string uniqueFileName = "default-product.jpg";

            if (formFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/product");
                uniqueFileName = Type + "_" + ProductCode.ToString() + "_" + formFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }

            }
            return uniqueFileName;
        }

        [HttpGet]
        public async Task<JsonResult> GetProductSize(int Tinggi, int Panjang, int Lebar)
        {
            var tebals = await _unitOfWork.StorageTebal.GetAllAsync(
                filter:
                    x => x.MaxTinggi >= Tinggi,
                orderBy:
                    m => m.OrderBy(m => m.MaxTinggi));

            var tebal = tebals.FirstOrDefault();

            var besarans = await _unitOfWork.StorageBesaran.GetAllAsync(
                filter:
                    m => m.MaxPanjang >= Panjang &&
                    m.MaxLebar >= Lebar,
                orderBy:
                    m => m.OrderBy(m => m.MaxPanjang).OrderBy(m => m.MaxLebar));

            var besaran = besarans.FirstOrDefault();

            if (tebal == null || besaran == null)
            {
                return Json(null);
            }

            var size = await _unitOfWork.StorageSize.GetSingleOrDefaultAsync(
                filter:
                    m => m.TebalCode == tebal.Code &&
                    m.BesaranCode == besaran.Code);
            return Json(size);
        }

        public async Task<IActionResult> GetProductsByTenant(Guid TenantId)
        {
            var model = await _unitOfWork.Product.GetAllAsync(
                filter:
                    m => m.TenantId == TenantId &&
                    m.Flag == FlagEnum.Active,
                includeProperties:
                    m => m.Include(m => m.MasProductPackaging)
                    .Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageSize)
                    .Include(m => m.InvStorageZone));

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductByProductId(int ProductId)
        {
            var model = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    m => m.ProductId == ProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductPackaging)
                    .Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageSize)
                    .Include(m => m.InvStorageZone));

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetUnitById(int Id)
        {
            var model = await _unitOfWork.ProductUnit.GetSingleOrDefaultAsync(
                filter:
                    m => m.Id == Id);
            return Json(Ok(model));
        }
    }
}