#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class TenantDivisionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TenantDivisionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<JsonResult> GetTenantDivisionsByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.TenantDivision.GetAllAsync(
                filter:
                    m => m.TenantId == TenantId); 

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetDivisionById(int Id)
        {
            var model = await _unitOfWork.TenantDivision.GetSingleOrDefaultAsync(
                filter:
                    m => m.Id == Id
                );

            return Json(model);
        }
    }
}
