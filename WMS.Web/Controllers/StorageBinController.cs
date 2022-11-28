using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class StorageBinController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageBinController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<JsonResult> GetBinBySectionCodeByColumnCode(string SectionCode, string ColumnCode)
        {
            var model = await _unitOfWork.StorageBin.GetAllAsync(
                filter:
                    m => m.InvStorageLevel.SectionCode == SectionCode &&
                    m.InvStorageLevel.ColumnCode == ColumnCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageLevel));

            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBinByBinCode(string BinCode)
        {
            var model = await _unitOfWork.StorageBin.GetSingleOrDefaultAsync(
                filter:
                    m => m.BinCode == BinCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageCode));
            return Json(model);
        }
    }
}
