using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var model = await _unitOfWork.User.GetAllAsync();
            return Ok(model);
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetByUserId(Guid UserId)
        {
            var model = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    x => x.UserId == UserId);
            return Ok(model);
        }

        [HttpGet("Current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            Guid UserId = new Guid(User.FindFirst("UserId")?.Value);

            var model = await _unitOfWork.User.GetSingleOrDefaultAsync(
                filter:
                    m => m.UserId == UserId);
            return Ok(model);
        }
    }
}
