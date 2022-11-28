using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Utility;

namespace WMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                    filter:
                        x => x.UserName == model.username,
                    includeProperties:
                        m => m.Include(m => m.SecProfile)
                        .Include(m => m.MasJabatan)
                        .Include(m => m.MasHouseCode));

            if (user == null)
            {
                TempData["error"] = "Account Not Found";
                return View();
            }
            if (user.Flag == FlagEnum.NonActive)
            {
                TempData["error"] = "Your Account has been disabled";
                return View();
            }
            if (user.Password != Hasher.GenerateHash(model.password, user.Salt))
            {
                TempData["error"] = "Password was wrong";
                return View();
            }
            if (user.ExpireDate < DateTime.Now)
            {
                TempData["error"] = "Your Account has been expired";
                return View();
            }

            if (user.UserName == model.password)
            {
                return RedirectToAction("UpdatePassword", new {UserId = user.UserId, Salt = user.Salt});
            }


            await _unitOfWork.UserManager.SignIn(HttpContext, user, false);
            return LocalRedirect("~/Dashboards");
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await _unitOfWork.UserManager.SignOut(HttpContext);
            return RedirectPermanent("~/Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePassword(Guid UserId, string Salt)
        {
            var model = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId &&
                    m.Salt == Salt);

            if (model == null)
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction("Login");
            }

            if (model.Password != Hasher.GenerateHash(model.UserName, model.Salt))
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction("Login");
            }

            var result = new UserChangePasswordViewModel
            {
                UserId = UserId,
                UserName = model.UserName,
                Salt = Salt,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ProfileImageUrl = model.ProfileImageUrl
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UserChangePasswordViewModel model)
        {
            var secUser = await _unitOfWork.User.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.UserId == model.UserId &&
                    m.UserName == model.UserName &&
                    m.Salt == model.Salt);

            if (model == null)
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction("Login");
            }

            if (secUser.Password != Hasher.GenerateHash(model.OldPassword, secUser.Salt))
            {
                TempData["error"] = "Oldpassword was wrong!";
                return View(model);
            }

            if (model.NewPassword == model.OldPassword || model.ConfirmPassword == model.OldPassword)
            {
                TempData["error"] = "Password cannot be same!";
                return View(model);
            }

            if(model.ConfirmPassword != model.NewPassword)
            {
                TempData["error"] = "Password does not match!";
                return View(model);
            }

            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$");
            if (!validateGuidRegex.IsMatch(model.NewPassword))
            {
                TempData["error"] = "Invalid Modelstate.";
                return View(model);
            }

            var Salt = Hasher.GenerateSalt();

            secUser.Salt = Salt;
            secUser.Password = Hasher.GenerateHash(model.NewPassword, Salt);
            secUser.ModifiedBy = model.UserName;
            secUser.ModifiedDate = DateTime.Now;

            _unitOfWork.User.Update(secUser);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Password Updated Successfully!";
            return RedirectToAction("Login");
        }
    }
}