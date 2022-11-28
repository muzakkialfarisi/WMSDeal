using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Utility;

namespace WMS.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    x => x.UserName == model.username,
                includeProperties:
                    m => m.Include(m => m.MasHouseCode)
                    .Include(m => m.MasJabatan)
                    .Include(m => m.SecProfile));

            if (user == null)
            {
                return Unauthorized("Account not found");
            }

            if (user.ProfileId.ToString() != SD.Role_WarehouseAdmin)
            {
                return Unauthorized("Account not found");
            }

            if (user.ExpireDate < DateTime.Now)
            {
                return Unauthorized("Your Account has been expired");
            }

            if (user.Flag == FlagEnum.NonActive)
            {
                return Unauthorized("Your Account has been disabled");
            }

            if(user.Password != Hasher.GenerateHash(model.password, user.Salt))
            {
                return Unauthorized("Password was wrong");
            }

            
            var token = _unitOfWork.UserManager.JwtGenerator(user, SignIn_Type.Internal);

            return Ok(token);
        }

        [HttpGet("Versions")]
        public async Task<ActionResult> GetMobileVersion(string? Device)
        {
            var model = await _unitOfWork.MobileAppVersion.GetAllAsync();

            if(Device != null)
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
