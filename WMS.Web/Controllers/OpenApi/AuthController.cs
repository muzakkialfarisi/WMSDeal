using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models.ViewModels;
using WMS.Models;
using WMS.Models.ViewModels.ApiViewModel.Maui;
using WMS.Utility;

namespace WMS.Web.Controllers.OpenApi
{
    [Route("openapi/[controller]")]
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
                        m => m.Include(m => m.SecUserTenant)
                            .ThenInclude(m => m.MasDataTenant));

                if (user != null)
                {
                    if (user.ExpireDate > DateTime.Now)
                    {
                        if (user.Flag == FlagEnum.NonActive)
                        {
                            if (user.Password != Hasher.GenerateHash(model.password, user.Salt))
                            {
                                var token = new TokenApiViewModel
                                {
                                    Token = _unitOfWork.UserManager.JwtGenerator(user, SignIn_Type.External)
                                };

                                return Ok(token);
                            }
                        }
                    }
                }

                _error.Message = "Invalid username or password!";
                _error.Code = "LG0001";
                return StatusCode(StatusCodes.Status500InternalServerError, _error);
            }
            catch (Exception e)
            {
                _error.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _error);
            }
        }
    }
}
