#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using WMS.Utility;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _unitOfWork.User.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasJabatan.MasDivision)
                    .Include(x => x.SecProfile)
                    .Include(x => x.MasHouseCode));

            List<UserViewModel> model = users.Select(x => new UserViewModel
            {
                UserId = x.UserId,
                ProfileImageUrl = x.ProfileImageUrl,
                UserName = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                EmailConfirmed = x.EmailConfirmed,
                PhoneNumberConfirmed = x.PhoneNumberConfirmed,
                ProfileId = x.ProfileId,
                HouseCode = x.HouseCode,
                JobPosId = x.JobPosId,
                ExpireDate = x.ExpireDate,
                Flag = x.Flag,
                SecProfile = x.SecProfile,
                MasJabatan = x.MasJabatan,
                MasHouseCode = x.MasHouseCode,

            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid UserId)
        {
            var model = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId,
                includeProperties:
                    m => m.Include(m => m.MasHouseCode)
                    .Include(m => m.MasJabatan.MasDivision)
                    .Include(m => m.SecProfile)
                    .Include(m => m.SecUserTenant).ThenInclude(m => m.MasDataTenant)
                    .Include(m => m.SecUserWarehouses).ThenInclude(m => m.MasHouseCode));

            if(model == null)
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction("Index");
            }

            ViewData["TenantId"] = new SelectList(await _unitOfWork.Tenant.GetAllAsync(), "TenantId", "Name");

            var usertenants = await _unitOfWork.UserTenant.GetAllAsync(
                filter:
                    m => m.UserId == UserId);

            var tenantwarehouses = await _unitOfWork.TenantWarehouse.GetAllAsync(
                filter:
                    m => usertenants.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString()),
                includeProperties:
                    m => m.Include(m => m.MasHouseCode));

            ViewData["HouseCode"] = new SelectList(tenantwarehouses, "HouseCode", "MasHouseCode.HouseName");

            var result = new UserViewModel
            {
                UserId = model.UserId,
                ProfileImageUrl = model.ProfileImageUrl,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailConfirmed,
                PhoneNumberConfirmed = model.PhoneNumberConfirmed,
                ProfileId = model.ProfileId,
                HouseCode = model.HouseCode,
                JobPosId = model.JobPosId,
                ExpireDate = model.ExpireDate,
                Flag = model.Flag,
                SecProfile = model.SecProfile,
                MasJabatan = model.MasJabatan,
                MasHouseCode = model.MasHouseCode,
                SecUserTenant = model.SecUserTenant,
                SecUserWarehouses = model.SecUserWarehouses
            };

            return View(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateUserWarehouse(Guid UserId, string HouseCode)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId,
                includeProperties:
                    m => m.Include(m => m.SecUserWarehouses));

            var warehouse = await _unitOfWork.HouseCode.GetSingleOrDefaultAsync(
                filter:
                    m => m.HouseCode == HouseCode);

            if (user != null && warehouse != null)
            {
                if(user.SecUserWarehouses.Any(m => m.HouseCode == warehouse.HouseCode))
                {
                    TempData["error"] = "Warehouse Already Exist!";
                    return RedirectToAction("Detail", new { UserId = UserId });
                }

                var model = new SecUserWarehouse
                {
                    UserId = user.UserId,
                    HouseCode = warehouse.HouseCode,
                    Flag = FlagEnum.Active
                };

                await _unitOfWork.UserWarehouse.AddAsync(model);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Detail", new { UserId = UserId });
            }

            TempData["error"] = "Invalid Modelstate!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserWarehouse(string Id, Guid UserId)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId,
                includeProperties:
                    m => m.Include(m => m.SecUserWarehouses));

            if (user == null)
            {
                TempData["error"] = "User notfound!";
                return RedirectToAction("Detail", new { UserId = UserId });
            }

            if (!user.SecUserWarehouses.Any(m => m.Id == Id))
            {
                TempData["error"] = "Warehouse notfound!";
                return RedirectToAction("Detail", new { UserId = UserId });
            }

            _unitOfWork.UserWarehouse.Remove(user.SecUserWarehouses.SingleOrDefault(m => m.Id == Id));
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Warehosue deleted successfully!";
            return RedirectToAction("Detail", new { UserId = UserId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserTenant(Guid UserId, Guid TenantId)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId,
                includeProperties:
                    m => m.Include(m => m.SecUserTenant));


            var tenant = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                filter:
                    m => m.TenantId == TenantId);

            if(user != null && tenant != null)
            {
                if (user.ProfileId.ToString() != SD.Role_Tenant)
                {
                    TempData["error"] = "Invalid ProvileId!";
                    return RedirectToAction("Detail", new { UserId = UserId });
                }

                if (user.SecUserTenant != null)
                {
                    TempData["error"] = "Only 1 Tenant Allowed!";
                    return RedirectToAction("Detail", new { UserId = UserId });
                }

                var model = new SecUserTenant
                {
                    UserId = user.UserId,
                    TenantId = tenant.TenantId,
                    Flag = FlagEnum.Active
                };

                await _unitOfWork.UserTenant.AddAsync(model);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Detail", new { UserId = UserId});
            }

            TempData["error"] = "Invalid Modelstate!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserTenant(string Id, Guid UserId)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId,
                includeProperties:
                    m => m.Include(m => m.SecUserTenant));

            if (user == null)
            {
                TempData["error"] = "User notfound!";
                return RedirectToAction("Detail", new { UserId = UserId });
            }

            if (user.SecUserTenant != null)
            {
                TempData["error"] = "Tenant  notfound!";
                return RedirectToAction("Detail", new { UserId = UserId });
            }

            _unitOfWork.UserTenant.Remove(user.SecUserTenant);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Tenant deleted successfully!";
            return RedirectToAction("Detail", new { UserId = UserId });
        }

        private string GetUploadFileName(string fileName, UserViewModel userViewModel)
        {
            string uniqueFileName = null;
            if (userViewModel.ProfileImage != null)
            {
                var fName = userViewModel.ProfileImage.FileName;
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/avatars");
                uniqueFileName = fileName.ToString() + fName.Substring(fName.Length - 4);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    userViewModel.ProfileImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(), "HouseCode", "HouseName");
            ViewData["JobPosId"] = new SelectList(await _unitOfWork.Jabatan.GetAllAsync(), "JobPosId", "JobPosName");
            ViewData["ProfileId"] = new SelectList(await _unitOfWork.Profile.GetAllAsync(), "ProfileId", "ProfileName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            ViewData["JobPosId"] = new SelectList(await _unitOfWork.Jabatan.GetAllAsync(), "JobPosId", "JobPosName", model.JobPosId);
            ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(), "HouseCode", "HouseName", model.HouseCode);
            ViewData["ProfileId"] = new SelectList(await _unitOfWork.Profile.GetAllAsync(), "ProfileId", "ProfileName", model.ProfileId);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid Modelstate!";
                return View(model);
            }

            if(await _unitOfWork.User.AnyAsync(filter: m => m.UserName == model.UserName))
            {
                TempData["error"] = "User name already exists, please type another username";
                return View(model);
            }


            var UserId = Guid.NewGuid();

            var salt = Hasher.GenerateSalt();

            var ProfileImageUrl = GetUploadFileName(UserId.ToString(), model);
            if (ProfileImageUrl == null)
            {
                ProfileImageUrl = "noimage.png";
            }

            SecUser secUser = new SecUser
            {
                UserId = UserId,
                UserName = model.UserName,
                Salt = salt,
                Password = Hasher.GenerateHash(model.UserName, salt),
                ProfileImageUrl = ProfileImageUrl,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.Email,
                PhoneNumberConfirmed = model.PhoneNumber,
                ProfileId = model.ProfileId,
                HouseCode = model.HouseCode,
                JobPosId = model.JobPosId,
                ExpireDate = DateTime.Now.AddYears(1),
                ExpireFlag = 1,
                CreateDate = DateTime.Now,
                ModifiedBy = User.FindFirst("UserName")?.Value,
                CreateBy = User.FindFirst("UserName")?.Value,
                Flag = FlagEnum.Active,
            };

            await _unitOfWork.User.AddAsync(secUser);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Data saved successfully";
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(filter: m => m.UserId == id);

            if(user == null)
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobPosId"] = new SelectList(await _unitOfWork.Jabatan.GetAllAsync(), "JobPosId", "JobPosName", user.JobPosId);
            ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(), "HouseCode", "HouseName", user.HouseCode);
            ViewData["ProfileId"] = new SelectList(await _unitOfWork.Profile.GetAllAsync(), "ProfileId", "ProfileName", user.ProfileId);
            ViewData["Picprofile"] = user.ProfileImageUrl;

            UserViewModel userVm = new UserViewModel{
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ProfileId = user.ProfileId,
                HouseCode = user.HouseCode,
                JobPosId = user.JobPosId,
                SecProfile = user.SecProfile,
                MasHouseCode = user.MasHouseCode,
                MasJabatan = user.MasJabatan,
                Flag = user.Flag
            };

            return View(userVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? UserId, UserViewModel model)
        {
            ViewData["JobPosId"] = new SelectList(await _unitOfWork.Jabatan.GetAllAsync(), "JobPosId", "JobPosName", model.JobPosId);
            ViewData["HouseCode"] = new SelectList(await _unitOfWork.HouseCode.GetAllAsync(), "HouseCode", "HouseCode", model.HouseCode);
            ViewData["ProfileId"] = new SelectList(await _unitOfWork.Profile.GetAllAsync(), "ProfileId", "Description", model.ProfileId);
            ViewData["Picprofile"] = "noimage.png";

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid Modelstate!";
                return View(model);
            }

            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                disableTracking:
                    false, 
                filter: 
                    m => m.UserId == UserId);

            if (user == null)
            {
                TempData["error"] = "User Notfound!";
                return View(model);
            }

            user.UserId = model.UserId;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.EmailConfirmed = model.Email;
            user.PhoneNumberConfirmed = model.PhoneNumber;
            user.ProfileId = model.ProfileId;
            user.HouseCode = model.HouseCode;
            user.JobPosId = model.JobPosId;
            user.Flag = model.Flag;
            user.ModifiedBy = User.FindFirst("UserName")?.Value;
            user.ModifiedDate = DateTime.Now;

            if (model.ProfileImage != null)
            {
                user.ProfileImageUrl = GetUploadFileName(model.UserId.ToString(), model);
            }

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "User updated successfully!";
            return RedirectToAction(nameof(Index));

        }

        [HttpPost ]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtendExpiration(string id)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                disableTracking:
                    false, 
                filter:
                    u => u.UserId.ToString() == id.Substring(1));

            if(user == null)
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction("Index");
            }

            if (id.Substring(0, 1) == "1")
            {
                user.ExpireDate = DateTime.Now.AddYears(1);
                user.ExpireFlag = 1;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = User.FindFirst("UserName")?.Value;
                _unitOfWork.User.Update(user);

                TempData["success"] = "User expiration has been successfully extended";
            }
            if (id.Substring(0, 1) == "2")
            {
                var salt = Hasher.GenerateSalt();
                user.Password = Hasher.GenerateHash(user.UserName, salt);
                user.Salt = salt;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = User.FindFirst("UserName")?.Value;
                _unitOfWork.User.Update(user);

                TempData["success"] = "Password successfully reset";
            }
            if (id.Substring(0, 1) == "3")
            {
                user.Flag = FlagEnum.Active;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = User.FindFirst("UserName")?.Value;
                _unitOfWork.User.Update(user);

                var profile = await _unitOfWork.Profile.GetSingleOrDefaultAsync(
                disableTracking:
                    false, 
                filter:
                    x => x.ProfileId == user.ProfileId);

                if (profile == null)
                {
                    TempData["error"] = "Profile Notfound!";
                    return RedirectToAction("Index");
                }

                if (profile.Flag == FlagEnum.NonActive)
                {
                    profile.Flag = FlagEnum.Active;
                    profile.DateModified = DateTime.Now;
                    profile.ModifiedBy = User.FindFirst("UserName")?.Value;
                    _unitOfWork.Profile.Update(profile);
                }

                TempData["success"] = "User has been activated successfully";
            }

            if (id.Substring(0, 1) == "4")
            {
                user.Flag = FlagEnum.NonActive;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = User.FindFirst("UserName")?.Value;
                _unitOfWork.User.Update(user);

                TempData["success"] = "User disabled successfully";
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction("Detail", new { UserId = user.UserId});
        }
    }
}