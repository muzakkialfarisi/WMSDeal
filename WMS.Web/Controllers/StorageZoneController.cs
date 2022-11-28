#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class StorageZoneController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageZoneController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.StorageZone.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string ZoneCode, InvStorageZone model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid Model State!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _unitOfWork.StorageZone.GetSingleOrDefaultAsync(
                filter:
                    m => m.ZoneCode == ZoneCode);

            var update = true;

            if (result == null)
            {
                result = new InvStorageZone();
                update = false;
            }

            result.ZoneCode = model.ZoneCode.Trim().ToUpper();
            result.ZoneName = model.ZoneName.ToUpper();
            result.Flag = model.Flag;

            var codechecker = await _unitOfWork.StorageZone.GetAllAsync(
                filter:
                    m => m.ZoneCode != result.ZoneCode &&
                    m.ZoneName == result.ZoneName);

            if (codechecker.Count > 0)
            {
                TempData["error"] = "Code or Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _unitOfWork.StorageZone.Update(result);
                TempData["success"] = "Updated Successfully";
            }
            else
            {
                await _unitOfWork.StorageZone.AddAsync(result);
                TempData["success"] = "Added Successfully";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
