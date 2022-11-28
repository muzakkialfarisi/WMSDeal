#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class StorageCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.StorageCategory.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int StorageCategoryId, InvStorageCategory model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid Model State!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _unitOfWork.StorageCategory.GetSingleOrDefaultAsync(
                filter:
                    m => m.StorageCategoryId == StorageCategoryId);

            var update = true;

            if (result == null)
            {
                result = new InvStorageCategory();
                update = false;
            }

            result.StorageCategoryCode = model.StorageCategoryCode.Trim().ToUpper();
            result.StorageCategoryName = model.StorageCategoryName.ToUpper();
            result.Flag = model.Flag;

            var codechecker = await _unitOfWork.StorageCategory.GetAllAsync(
                filter:
                    m => (m.StorageCategoryCode == result.StorageCategoryCode ||
                    m.StorageCategoryName == result.StorageCategoryName) && 
                    m.StorageCategoryId != result.StorageCategoryId);

            if (codechecker.Count > 0)
            {
                TempData["error"] = "Code or Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _unitOfWork.StorageCategory.Update(result);
                TempData["success"] = "Updated Successfully";
            }
            else
            {
                await _unitOfWork.StorageCategory.AddAsync(result);
                TempData["success"] = "Added Successfully";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
