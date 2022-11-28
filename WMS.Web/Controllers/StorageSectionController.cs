using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class StorageSectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageSectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<JsonResult> GetSectionByRowCode(string RowCode)
        {
            var temp = await _unitOfWork.StorageSection.GetAllAsync(
                filter:
                    a => a.RowCode == RowCode);

            return Json(temp);
        }
        [HttpGet]
        public async Task<JsonResult> GetSectionBySectionCode(string SectionCode)
        {
            var temp = await _unitOfWork.StorageSection.GetSingleOrDefaultAsync(
                filter:
                    a => a.SectionCode == SectionCode);

            return Json(temp);
        }
    }
}
