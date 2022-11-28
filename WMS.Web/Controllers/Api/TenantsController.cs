using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class TenantsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TenantsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string HouseCode)
        {
            var model = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));

            if (HouseCode != null)
            {
                model = model.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
            }

            return Ok(model);
        }

        [HttpGet("Pages")]
        public async Task<IActionResult> GetPages(int indexStart, int indexSize, string HouseCode)
        {
            var model = await _unitOfWork.Tenant.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenantWarehouses));

            if (HouseCode != null)
            {
                model = model.Where(m => m.MasDataTenantWarehouses.Any(m => m.HouseCode == HouseCode)).ToList();
            }

            model = model.Skip(indexStart - 1).Take(indexSize).ToList();

            return Ok(model);
        }

        [HttpGet("{TenantId}")]
        public async Task<IActionResult> GetByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.Tenant.GetSingleOrDefaultAsync(
                filter:
                    x => x.TenantId == TenantId);

            return Ok(model);
        }
    }
}