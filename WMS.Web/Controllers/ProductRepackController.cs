#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class ProductRepackController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductRepackController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.ProductTypeOfRepack.GetAllAsync();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string RepackCode, MasProductTypeOfRepack model)
        {
            var result = await _unitOfWork.ProductTypeOfRepack.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RepackCode == RepackCode);

            var update = true;

            if (result == null)
            {
                result = new MasProductTypeOfRepack();
                update = false;
            }

            result.RepackCode = model.RepackCode.Trim().ToUpper();
            result.RepackName = model.RepackName.ToUpper();
            result.RepackDescription = model.RepackDescription;
            result.Flag = model.Flag;

            var checkname = await _unitOfWork.ProductTypeOfRepack.GetAllAsync(
                filter:
                    m => m.RepackName == result.RepackName &&
                    m.RepackCode != result.RepackCode);

            if (checkname.Count > 0)
            {
                TempData["error"] = "Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _unitOfWork.ProductTypeOfRepack.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _unitOfWork.ProductTypeOfRepack.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}