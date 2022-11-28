#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class StorageSizesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageSizesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.StorageSize.GetAllAsync(
                includeProperties:
                    m => m.Include(i => i.InvStorageBesaran)
                    .Include(i => i.InvStorageTebal));
                
            return View(model);

        }

        public async Task<IActionResult> Details(string id)
        {
            var model = await _unitOfWork.StorageSize.GetSingleOrDefaultAsync(
                filter:
                    m => m.SizeCode == id,
                includeProperties:
                    m => m.Include(i => i.InvStorageBesaran)
                    .Include(i => i.InvStorageTebal));

            if (model == null)
            {
                TempData["error"] = "Storage Size Notfound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["BesaranCode"] = new SelectList(await _unitOfWork.StorageBesaran.GetAllAsync(), "Code", "Name");
            ViewData["TebalCode"] = new SelectList(await _unitOfWork.StorageTebal.GetAllAsync(), "Code", "Name" );

            var model = new StorageSizeViewModel();

            model.Tebals = await _unitOfWork.StorageTebal.GetAllAsync(
                orderBy:
                    m => m.OrderBy(m => m.MaxTinggi));

            model.Besarans = await _unitOfWork.StorageBesaran.GetAllAsync(
                orderBy:
                    m => m.OrderBy(m => m.MaxPanjang).OrderBy(m => m.MaxLebar));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StorageSizeViewModel model)
        {
            ViewData["BesaranCode"] = new SelectList(await _unitOfWork.StorageBesaran.GetAllAsync(), "Code", "Name", model.InvStorageSize.BesaranCode);
            ViewData["TebalCode"] = new SelectList(await _unitOfWork.StorageTebal.GetAllAsync(), "Code", "Name", model.InvStorageSize.TebalCode);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid Modelstate!";
                return View(model);
            }

            var wideName = await _unitOfWork.StorageBesaran.GetSingleOrDefaultAsync(
                filter:
                    m => m.Code == model.InvStorageSize.BesaranCode);


            var heigthName = await _unitOfWork.StorageTebal.GetSingleOrDefaultAsync(
                filter:
                    m => m.Code == model.InvStorageSize.TebalCode);

            if (wideName == null || heigthName == null)
            {
                TempData["error"] = "Wide Code or Height Code not found";
                return View(model);
            }

            var first = "C";
            var last = 1.ToString("D3");

            var sizeCode = first + last;

            if (await _unitOfWork.StorageSize.AnyAsync(filter: m => m.SizeCode == sizeCode))
            {
                var checker = await _unitOfWork.StorageSize.GetAllAsync(m => m.SizeCode.Contains(first));
                int LastCount = int.Parse(checker.Max(m => m.SizeCode.Substring(first.Length)));
                sizeCode = first + (LastCount + 1).ToString("00#");
            }

            model.InvStorageSize.SizeCode = sizeCode;
            model.InvStorageSize.SizeName = wideName.Name + " - " + heigthName.Name;

            await _unitOfWork.StorageSize.AddAsync(model.InvStorageSize);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Data Saved Successfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTebal(InvStorageTebal invStorageTebal)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.StorageTebal.AnyAsync(e => e.Code == invStorageTebal.Code))
                {
                    TempData["error"] = "Storage Code Already Exists";
                    return RedirectToAction(nameof(Create));

                }
                else
                {
                    await _unitOfWork.StorageTebal.AddAsync(invStorageTebal);
                    await _unitOfWork.SaveAsync();

                    TempData["success"] = "Data Saved Successfully";
                    return RedirectToAction(nameof(Create));
                }

            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBesar(InvStorageBesaran invStorageBesaran)
        {
            if (ModelState.IsValid)
            {
                if (await _unitOfWork.StorageBesaran.AnyAsync(e => e.Code == invStorageBesaran.Code))
                {
                    TempData["error"] = "Storage Code Already Exists";
                    return RedirectToAction(nameof(Create));

                }
                else
                {
                    await _unitOfWork.StorageBesaran.AddAsync(invStorageBesaran);
                    await _unitOfWork.SaveAsync();

                    TempData["success"] = "Data Saved Successfully";
                    return RedirectToAction(nameof(Create));
                }

            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {

            var invStorageSize = await _unitOfWork.StorageSize.GetSingleOrDefaultAsync(m => m.SizeCode == id);
            if (invStorageSize == null)
            {
                TempData["error"] = "Stprage Size Notfound!";
                return RedirectToAction("Index");
            }
            ViewData["BesaranCode"] = new SelectList(await _unitOfWork.StorageBesaran.GetAllAsync(), "Code", "Code", invStorageSize.BesaranCode);
            ViewData["TebalCode"] = new SelectList(await _unitOfWork.StorageTebal.GetAllAsync(), "Code", "Code", invStorageSize.TebalCode);
            return View(invStorageSize);
        }

        [HttpGet]
        public async Task<JsonResult> GetStorageSize()
        {
            var storageSize = await _unitOfWork.StorageSize.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.InvStorageTebal)
                    .Include(m => m.InvStorageBesaran));

            return Json(storageSize);

        }
    }
}
