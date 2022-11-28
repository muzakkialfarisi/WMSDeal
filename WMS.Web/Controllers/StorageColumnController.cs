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
    [Authorize(Policy = "AdminWarehouse")]
    public class StorageColumnController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageColumnController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<JsonResult> GetColumnByColumnCode(string ColumnCode)
        {
            var model = await _unitOfWork.StorageColumn.GetSingleOrDefaultAsync(
                filter:
                    m => m.ColumnCode == ColumnCode);
            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetColumnByRowCode(string RowCode)
        {
            var model = await _unitOfWork.StorageColumn.GetAllAsync(
                filter:
                    a => a.RowCode == RowCode);
            return Json(model);
        }
    }
}
