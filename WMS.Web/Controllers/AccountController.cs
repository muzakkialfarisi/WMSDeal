using WMS.Models;
using WMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Utility;
using System.Text.RegularExpressions;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult UpdatePassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var UserId = User.FindFirst("UserId")?.Value;

            var model = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId.ToString() == UserId,
                includeProperties:
                    m => m.Include(m => m.SecProfile)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasJabatan)
                    .Include(m => m.SecUserWarehouses).ThenInclude(m => m.MasHouseCode));

            if (model == null)
            {
                TempData["error"] = "User Notfound!";
                return RedirectToAction("Login");
            }

            model.Salt = null;
            model.Password = null;

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdatePassword(UserChangePasswordViewModel model)
        {
            var UserId = User.FindFirst("UserId")?.Value;

            var result = await _unitOfWork.User.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.UserId.ToString() == UserId);

            if (result == null)
            {
                return Json(BadRequest("User Notfound!"));
            }

            if (result.Password != Hasher.GenerateHash(model.OldPassword, result.Salt))
            {
                return Json(BadRequest("Password waswrong!"));
            }

            if (model.NewPassword == model.OldPassword || model.ConfirmPassword == model.OldPassword)
            {
                return Json(BadRequest("Password cannot be same!"));
            }

            if (model.ConfirmPassword != model.NewPassword)
            {
                return Json(BadRequest("Password does not match!"));
            }

            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$");
            if (!validateGuidRegex.IsMatch(model.NewPassword))
            {
                return Json(BadRequest("Invalid Modelstate!"));
            }

            var Salt = Hasher.GenerateSalt();

            result.Salt = Salt;
            result.Password = Hasher.GenerateHash(model.NewPassword, Salt);
            result.ModifiedBy = model.UserName;
            result.ModifiedDate = DateTime.Now;

            _unitOfWork.User.Update(result);
            await _unitOfWork.SaveAsync();

            return Json(Ok("Success!"));
        }
    }
}
