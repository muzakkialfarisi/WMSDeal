using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class HouseCodesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HouseCodesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var model = await _unitOfWork.HouseCode.GetAllAsync();
            return Ok(model);
        }

        [HttpGet("{HouseCode}")]
        public async Task<IActionResult> GetByHouseCode(string HouseCode)
        {
            var model = await _unitOfWork.HouseCode.GetSingleOrDefaultAsync(
                filter:
                    x => x.HouseCode == HouseCode);
            return Ok(model);
        }
    }
}
