using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models.ViewModels;
using WMS.Models;
using WMS.Utility;
using WMS.Models.ViewModels.ApiViewModel.Maui;

namespace WMS.Web.Controllers.Maui
{
    [Route("maui/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ErrorResponseViewModel _error = new ErrorResponseViewModel();

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            try
            {
                var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    x => x.UserName == model.username,
                includeProperties:
                    m => m.Include(m => m.MasHouseCode)
                    .Include(m => m.SecProfile)
                    .Include(x => x.MasJabatan));

                if (user == null)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Account Notfound";
                    _error.Message = "Account Notfound!";
                    _error.Code = "LG1000";
                    return BadRequest(_error);
                }

                if (user.ExpireDate < DateTime.Now)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Account Expired";
                    _error.Message = "Your Account has been expired!";
                    _error.Code = "LG1001";
                    return BadRequest(_error);
                }

                if (user.Flag == FlagEnum.NonActive)
                {
                    _error.StatusCode = "400";
                    _error.Error = "Account Disabled";
                    _error.Message = "Your Account has been disabled!";
                    _error.Code = "LG1002";
                    return BadRequest(_error);
                }

                if (user.Password != Hasher.GenerateHash(model.password, user.Salt))
                {
                    _error.StatusCode = "400";
                    _error.Error = "Password wrong";
                    _error.Message = "Your Password was wrong!";
                    _error.Code = "LG1003";
                    return BadRequest(_error);
                }

                var token = new TokenApiViewModel
                {
                    Token = _unitOfWork.UserManager.JwtGenerator(user, SignIn_Type.Internal)
                };

                return Ok(token);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "LG5001";
                return BadRequest(_error);
            }
        }

       

        [HttpGet("Versions")]
        public async Task<ActionResult> GetMobileVersion(string? Device)
        {
            var model = await _unitOfWork.MobileAppVersion.GetAllAsync();

            if (Device != null)
            {
                Device = Device.ToLower();
                model = model.Where(m => m.Device.ToLower() == Device).ToList();
            }

            return Ok(model);
        }

        [HttpGet("Versions/{Device}/LastVersion")]
        public async Task<ActionResult> GetMobileLastVersion(string Device)
        {
            Device = Device.ToLower();
            var model = await _unitOfWork.MobileAppVersion.GetAllAsync(filter: m => m.Device.ToLower() == Device);
            var lastVersion = model.Max(m => m.Version);

            var result = await _unitOfWork.MobileAppVersion.GetSingleOrDefaultAsync(
                filter:
                    m => m.Version == lastVersion &&
                    m.Device.ToLower() == Device);
            return Ok(result);
        }
    }
}
