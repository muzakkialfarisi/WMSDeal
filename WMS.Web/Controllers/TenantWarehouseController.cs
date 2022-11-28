#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class TenantWarehouseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TenantWarehouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<JsonResult> GetTenantWarehousesByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.TenantWarehouse.GetAllAsync(
                filter:
                    m => m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));
            return Json(model);
        }
    }
}
