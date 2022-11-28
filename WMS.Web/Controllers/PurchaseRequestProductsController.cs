#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PurchaseRequestProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseRequestProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<JsonResult> GetPurchaseRequestProductById(int RequestProductId)
        {
            var model = await _unitOfWork.PurchaseRequestProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.RequestProductId == RequestProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductData));
                
            return Json(model);
        }
    }
}
