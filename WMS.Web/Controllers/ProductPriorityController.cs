#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class ProductPriorityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductPriorityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.ProductPriority.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string CargoPriorityCode, MasProductPriority model)
        {
            var result = await _unitOfWork.ProductPriority.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.CargoPriorityCode == CargoPriorityCode);

            var update = true;

            if (result == null)
            {
                result = new MasProductPriority();
                update = false;
            }

            result.CargoPriorityCode = model.CargoPriorityCode.Trim().ToUpper();
            result.CargoPriorityName = model.CargoPriorityName.ToUpper();
            result.CargoPriorityDescription = model.CargoPriorityDescription;
            result.Flag = model.Flag;

            var checkname = await _unitOfWork.ProductPriority.GetAllAsync(
                filter:
                    m => m.CargoPriorityName == result.CargoPriorityName &&
                    m.CargoPriorityCode != result.CargoPriorityCode);

            if (checkname.Count > 0)
            {
                TempData["error"] = "Code or Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _unitOfWork.ProductPriority.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _unitOfWork.ProductPriority.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
