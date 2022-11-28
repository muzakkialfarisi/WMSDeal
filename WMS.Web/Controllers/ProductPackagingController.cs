#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class ProductPackagingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductPackagingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.ProductPackaging.GetAllAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int PackagingId, MasProductPackaging model)
        {
            var result = await _unitOfWork.ProductPackaging.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.PackagingId == PackagingId);

            var update = true;

            if (result == null)
            {
                result = new MasProductPackaging();
                update = false;
            }

            result.PackagingCode = model.PackagingCode.Trim().ToUpper();
            result.PackagingName = model.PackagingName.ToUpper();
            result.Flag = model.Flag;

            var checkname = await _unitOfWork.ProductPackaging.GetAllAsync(
                filter:
                    m => (m.PackagingName == result.PackagingName ||
                    m.PackagingCode == result.PackagingCode) &&
                    m.PackagingId != PackagingId);

            if (checkname.Count > 0)
            {
                TempData["error"] = "Code or Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _unitOfWork.ProductPackaging.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _unitOfWork.ProductPackaging.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
