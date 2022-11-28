#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class SupplierController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.Supplier.GetAllAsync(
                filter:
                    m => m.TenantId == TenantId);

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierBySupplierId(int SupplierId)
        {
            var model = await _unitOfWork.Supplier.GetSingleOrDefaultAsync(
                filter:
                    m => m.SupplierId == SupplierId);

            return Json(model);
        }
    }
}
