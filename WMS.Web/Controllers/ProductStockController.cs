using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class ProductStockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductStockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<JsonResult> GetStockByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.ProductStock.GetAllAsync(
                filter:
                    m => m.MasProductData.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasProductData));

            return Json(model);
        }

        public async Task<JsonResult> GetStockByProductId(int ProductId)
        {
            var model = await _unitOfWork.ProductStock.GetAllAsync(
                filter:
                    m => m.ProductId == ProductId);
                
            return Json(model);
        }
    }
}