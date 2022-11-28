using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class ItemProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetItemProduct(int? DOProductId, int? Status)
        {
            var model = await _unitOfWork.ItemProduct.GetAllAsync();

            if (DOProductId != null)
            {
                model = model.Where(m => m.DOProductId == DOProductId).ToList();
            }
            if (Status != null)
            {
                model = model.Where(m => m.Status == Status).ToList();
            }

            return Ok(model);
        }

        [HttpGet("{IKU}")]
        public async Task<IActionResult> GetItemProductByIKU(string IKU)
        {
            var model = await _unitOfWork.ItemProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.IKU == IKU);

            return Ok(model);
        }
    }
}
