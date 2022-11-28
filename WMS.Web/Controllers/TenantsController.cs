using WMS.DataAccess;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.Utility;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "Tenant")]
    public class TenantsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment1;

        public TenantsController(IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork, AppDbContext context)
        {
            _webHostEnvironment1 = webHostEnvironment;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            ViewData["ProId"] = new SelectList(await _unitOfWork.Provinsi.GetAllAsync(), "ProId", "ProName");

            var model = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                m => m.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                .Include(m => m.MasDataTenantWarehouses));

            if (ProfileId == SD.Role_Tenant)
            {
                var userTenants = await _unitOfWork.UserTenant.GetAllAsync(
                filter:
                    m => m.UserId.ToString() == UserId);

                model = model.Where(m => userTenants.Select(m => m.TenantId).Contains(m.TenantId)).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> CreateTenant(MasDataTenant model)
        {
            var TenantCode = 1.ToString("D5");
            if (TenantCode == await _unitOfWork.Tenant.MaxAsync(selector: m => m.TenantCode))
            {
                TenantCode = (int.Parse(await _unitOfWork.Tenant.MaxAsync(selector: m => m.TenantCode)) + 1).ToString("0000#");
            }

            var TenantId = Guid.NewGuid();
            var avatar = GetUploadPicture(TenantId.ToString(), model);
            if (avatar == null)
            {
                avatar = "picture128.jpg";
            }

            model.Name = model.Name.ToUpper();
            model.TenantId = TenantId;
            model.TenantCode = TenantCode;
            model.ProfileImageUrl = avatar;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = User.FindFirst("UserName")?.Value;
            model.Flag = FlagEnum.Active;

            await _unitOfWork.Tenant.AddAsync(model);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Data Created Successfully";
            return RedirectToAction("Detail", new { TenantId = model.TenantId});
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid TenantId)
        {
            var model = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                filter:
                    m => m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses).ThenInclude(m => m.MasHouseCode)
                    .Include(m => m.MasSupplierDatas).ThenInclude(m => m.MasSupplierType)
                    .Include(m => m.MasSupplierDatas).ThenInclude(m => m.MasSupplierService)
                    .Include(m => m.MasSupplierDatas).ThenInclude(m => m.MasIndustry)
                    .Include(m => m.MasStores).ThenInclude(m => m.MasPlatform)
                    .Include(m => m.MasStores).ThenInclude(m => m.MasKelurahan)
                    .Include(m => m.MasDataTenantDivisions)
                    .Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                );

            if (model == null)
            {
                TempData["error"] = "Tenant Notfound!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(), "HouseCode", "HouseName");
            ViewData["SupplierServiceId"] = new SelectList(_context.MasSupplierServices.Where(i => i.Flag == FlagEnum.Active), "ServiceId", "SupplierServiceName");
            ViewData["SupplierTypeId"] = new SelectList(_context.MasSupplierTypes.Where(i => i.Flag == FlagEnum.Active), "TypeId", "SupplierTypeName");
            ViewData["IndustryId"] = new SelectList(_context.MasIndustries.Where(i => i.Flag == FlagEnum.Active), "ID", "IndustryName");
            ViewData["PlatformId"] = new SelectList(await _context.MasPlatforms.ToListAsync(), "PlatformId", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> UpsertTenantWarehouse(int? Id, MasDataTenantWarehouse model)
        {
            var tenant = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                    filter:
                        m => m.TenantId == model.TenantId,
                    includeProperties:
                        m => m.Include(m => m.MasDataTenantWarehouses));

            if (tenant == null)
            {
                TempData["error"] = "Tenant Notfound!";
                return RedirectToAction(nameof(Index));
            }

            var housecode = await _unitOfWork.HouseCode.GetSingleOrDefaultAsync(
                filter:
                    m => m.HouseCode == model.HouseCode);

            if (housecode == null)
            {
                TempData["error"] = "IHouseCode Notfound!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId});
            }

            if (Id == null)
            {
                if (tenant.MasDataTenantWarehouses.Any(m => m.HouseCode == model.HouseCode))
                {
                    TempData["error"] = "Warehouse Already Exists!";
                    return RedirectToAction("Detail", new { TenantId = model.TenantId });
                }

                TempData["success"] = "Tenant Warehouse Created Successfully!";
                await _unitOfWork.TenantWarehouse.AddAsync(model);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("Detail", new { TenantId = model.TenantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> DeleteTenantWarehouse(int Id)
        {
            var model = await _unitOfWork.TenantWarehouse.GetSingleOrDefaultAsync(
                filter:
                    m => m.Id == Id);

            if (model == null)
            {
                TempData["error"] = "Tenant Notfound!";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Manual gais!";
            return RedirectToAction("Detail", new { TenantId = model.TenantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertTenantSupplier(int? SupplierId, MasSupplierData model)
        {
            var tenant = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.TenantId == model.TenantId,
                includeProperties:
                    m => m.Include(m => m.MasSupplierDatas));

            if (tenant == null)
            {
                TempData["error"] = "Tenant Notfound!";
                return RedirectToAction(nameof(Index));
            }

            if (SupplierId == null)
            {
                if (tenant.MasSupplierDatas.Any(m => m.Name.ToUpper() == model.Name.ToUpper()))
                {
                    TempData["error"] = "Supplier already exists!";
                    return RedirectToAction("Detail", new { TenantId = model.TenantId });
                }

                var newmodel = new MasSupplierData
                {
                    TenantId = tenant.TenantId,
                    Name = model.Name.ToUpper(),
                    SupplierTypeId = model.SupplierTypeId,
                    SupplierServiceId = model.SupplierServiceId,
                    IndustryId = model.IndustryId,
                    Email = model.Email,
                    OfficePhone = model.OfficePhone,
                    HandPhone = model.HandPhone,

                    Address = tenant.Address,
                    KelId = tenant.KelId,
                    KodePos = tenant.KodePos,
                    RekeningNo = string.Empty,
                    BankName = string.Empty,
                    TermOfPayment = string.Empty,
                    CreditLimit = 0,

                    CreateDate = DateTime.Now,
                    ModifiedBy = string.Empty,
                    Flag = model.Flag
                };

                await _unitOfWork.Supplier.AddAsync(newmodel);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Supplier created successfully!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            var supplier = tenant.MasSupplierDatas.SingleOrDefault(m => m.SupplierId == SupplierId);

            if (supplier == null)
            {
                TempData["error"] = "Supplier notfound!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            if (tenant.MasSupplierDatas.Any(m => m.Name.ToUpper() == model.Name.ToUpper() && m.SupplierId != SupplierId))
            {
                TempData["error"] = "Supplier already exists!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            supplier.Name = model.Name;
            supplier.SupplierTypeId = model.SupplierTypeId;
            supplier.SupplierServiceId = model.SupplierServiceId;
            supplier.HandPhone = model.HandPhone;
            supplier.OfficePhone = model.OfficePhone;
            supplier.IndustryId = model.IndustryId;
            supplier.Email = model.Email;
            supplier.Flag = model.Flag;

            _unitOfWork.Supplier.Update(supplier);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Supplier Updated successfully!";
            return RedirectToAction("Detail", new { TenantId = model.TenantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertTenantStore(int? StoreId, MasStore model)
        {
            var tenant = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.TenantId == model.TenantId,
                    includeProperties:
                        m => m.Include(m => m.MasStores));

            if (tenant == null)
            {
                TempData["error"] = "Tenant Notfound!";
                return RedirectToAction(nameof(Index));
            }

            if (StoreId == null)
            {
                if (tenant.MasStores.Any(m => m.Name.ToUpper() == model.Name.ToUpper() && m.PlatformId == model.PlatformId))
                {
                    TempData["error"] = "Store already exists!";
                    return RedirectToAction("Detail", new { TenantId = model.TenantId });
                }

                var newmodel = new MasStore
                {
                    TenantId = tenant.TenantId,
                    Name = model.Name.ToUpper(),
                    PlatformId = model.PlatformId,
                    PhoneNumber = model.PhoneNumber,

                    Address = tenant.Address,
                    KelId = tenant.KelId,
                    KodePos = tenant.KodePos,

                    Flag = model.Flag
                };

                await _unitOfWork.Store.AddAsync(newmodel);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Store created successfully!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            var store = tenant.MasStores.SingleOrDefault(m => m.StoreId == StoreId);

            if (store == null)
            {
                TempData["error"] = "Store notfound!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            if (tenant.MasStores.Any(
                m => m.Name.ToUpper() == model.Name.ToUpper() && 
                m.StoreId != StoreId && 
                m.PlatformId == model.PlatformId))
            {
                TempData["error"] = "Store already exists!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            store.Name = model.Name;
            store.PlatformId = model.PlatformId;
            store.PhoneNumber = model.PhoneNumber;
            store.Flag = model.Flag;

            _unitOfWork.Store.Update(store);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Store Updated successfully!";
            return RedirectToAction("Detail", new { TenantId = model.TenantId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertTenantDivision(int? Id, MasDataTenantDivision model)
        {
            var tenant = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.TenantId == model.TenantId,
                    includeProperties:
                        m => m.Include(m => m.MasDataTenantDivisions));

            if (tenant == null)
            {
                TempData["error"] = "Tenant Notfound!";
                return RedirectToAction(nameof(Index));
            }

            if (Id == null)
            {
                if (tenant.MasDataTenantDivisions.Any(m => m.Name.ToUpper() == model.Name.ToUpper()))
                {
                    TempData["error"] = "Division already exists!";
                    return RedirectToAction("Detail", new { TenantId = model.TenantId });
                }

                var newmodel = new MasDataTenantDivision
                {
                    TenantId = tenant.TenantId,
                    Name = model.Name.ToUpper(),
                };

                await _unitOfWork.TenantDivision.AddAsync(newmodel);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Store created successfully!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            var division = tenant.MasDataTenantDivisions.SingleOrDefault(m => m.Id == Id);

            if (division == null)
            {
                TempData["error"] = "Store notfound!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            if (tenant.MasDataTenantDivisions.Any(
                m => m.Name.ToUpper() == model.Name.ToUpper() &&
                m.Id != Id))
            {
                TempData["error"] = "Store already exists!";
                return RedirectToAction("Detail", new { TenantId = model.TenantId });
            }

            division.Name = model.Name;

            _unitOfWork.TenantDivision.Update(division);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Store Updated successfully!";
            return RedirectToAction("Detail", new { TenantId = model.TenantId });
        }

        private string GetUploadPicture(string guid, MasDataTenant dataTenant)
        {
            string uniqueFileName = "picture128.jpg";

            if (dataTenant.FormProfileImage != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment1.WebRootPath, "img/tenant");
                uniqueFileName = guid + "_" + dataTenant.FormProfileImage.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    dataTenant.FormProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public async Task<JsonResult> GetAllTenants()
        {
            var model = await _unitOfWork.Tenant.GetAllAsync();
            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetTenantByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                filter:
                    m => m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));
            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetTenantByHouseCode(string HouseCode)
        {
            var model = await _unitOfWork.TenantWarehouse.GetAllAsync(
                filter:
                    m => m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant));
            return Json(model);
        }
    }
}