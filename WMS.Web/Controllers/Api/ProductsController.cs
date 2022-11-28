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
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? TenantId)
        {
            var model = await _unitOfWork.Product.GetAllAsync();

            if (TenantId != null)
            {
                model = model.Where(m => m.TenantId == TenantId).ToList();
            }

            return Ok(model);
        }

        [HttpGet("Pages")]
        public async Task<IActionResult> GetPageList(int indexStart, int indexSize, Guid? TenantId)
        {
            var model = await _unitOfWork.Product.GetAllAsync();

            if (TenantId != null)
            {
                model = model.Where(m => m.TenantId == TenantId).ToList();
            }

            model = model.Skip(indexStart - 1).Take(indexSize).ToList();

            return Ok(model);
        }

        [HttpGet("{ProductCode}")]
        public async Task<IActionResult> GetByProductId(Guid ProductCode)
        {
            var model = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                filter:
                    x => x.ProductCode == ProductCode);

            return Ok(model);
        }
    }
}