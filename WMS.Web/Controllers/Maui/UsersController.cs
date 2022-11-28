using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models.ViewModels.ApiViewModel.Maui;

namespace WMS.Web.Controllers.Maui
{
    [Route("maui/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ErrorResponseViewModel _error = new ErrorResponseViewModel();

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                Guid userid = new Guid(User.FindFirst("UserId").Value);

                var user = await _unitOfWork.User.GetSingleOrDefaultAsync(
                    filter:
                        m => m.UserId == userid,
                    includeProperties:
                        m => m.Include(m => m.SecProfile)
                        .Include(m => m.MasJabatan)
                        .Include(m => m.MasHouseCode)
                        );

                UserInfo userInfo = new UserInfo();
                userInfo.UserId =user.UserId;
                userInfo.UserName = user.UserName;
                userInfo.FirstName = user.FirstName;
                userInfo.LastName = user.LastName;
                userInfo.Email = user.Email;
                userInfo.PhoneNumber =  user.PhoneNumber;
                userInfo.JobPosName = user.MasJabatan.JobPosName;
                userInfo.HouseCode = user.HouseCode;
                userInfo.Profile = user.SecProfile.ProfileName;
                userInfo.ProfileId = user.ProfileId;
                userInfo.Warehouse = user.MasHouseCode.HouseName;
                userInfo.ExpireDate = user.ExpireDate;
              
                return Ok(userInfo);

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
    }
}
