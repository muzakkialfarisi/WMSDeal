using WMS.DataAccess;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WMS.Utility;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class StorageLevelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageLevelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string RowCode, string LevelCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = new StorageViewModel();

            model.invStorageLevel = await _unitOfWork.StorageLevel.GetSingleOrDefaultAsync(
                filter:
                    m => m.LevelCode == LevelCode &&
                    m.InvStorageColumn.RowCode == RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageBins).ThenInclude(m => m.InvStorageCode).ThenInclude(m => m.InvStorageSize.InvStorageBesaran)
                    .Include(m => m.InvStorageBins).ThenInclude(m => m.InvStorageCode).ThenInclude(m => m.InvStorageSize.InvStorageTebal)
                    .Include(m => m.InvStorageBins).ThenInclude(m => m.InvStorageCode).ThenInclude(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageColumn.InvStorageRow));

            if (model.invStorageLevel == null)
            {
                TempData["error"] = "Invalid model state!";
                return RedirectToAction("Index", "Dashboards");
            }

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                if (model.invStorageLevel.InvStorageColumn.InvStorageRow.HouseCode != HouseCode)
                {
                    TempData["error"] = "Invalid model state!";
                    return RedirectToAction("Index", "Dashboards");
                }
            }

            ViewData["StorageCategoryId"] = new SelectList(await _unitOfWork.StorageCategory.GetAllAsync(), "StorageCategoryId", "StorageCategoryName");

            var sizecode = await _unitOfWork.StorageSize.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.InvStorageBesaran)
                    .Include(m => m.InvStorageTebal));

            ViewData["SizeCode"] = sizecode
                                    .Select(n => new SelectListItem
                                    {
                                        Value = n.SizeCode,
                                        Text = string.Concat(n.InvStorageBesaran.MaxPanjang, " (P) x ", n.InvStorageBesaran.MaxLebar, " (L) x ", n.InvStorageTebal.MaxTinggi, " (T) | ", n.SizeName)
                                    }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertLevel(StorageViewModel model)
        {
            var storagelevel = await _unitOfWork.StorageLevel.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.LevelCode == model.invStorageLevel.LevelCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageColumn.InvStorageRow)
                    .Include(m => m.InvStorageSection));

            if (storagelevel == null)
            {
                TempData["error"] = "Invalid model state!";
                return RedirectToAction("Index", "Dashboards");
            }

            storagelevel.LevelName = storagelevel.InvStorageColumn.InvStorageRow.HouseCode + " - " +
                    storagelevel.InvStorageColumn.InvStorageRow.ZoneCode + " - " +
                    storagelevel.InvStorageColumn.RowCode.Substring(storagelevel.InvStorageColumn.InvStorageRow.HouseCode.Length + storagelevel.InvStorageColumn.InvStorageRow.ZoneCode.Length) + " - " +
                    model.invStorageLevel.LevelCode.Substring(storagelevel.InvStorageColumn.RowCode.Length);
            storagelevel.Flag = model.invStorageLevel.Flag;
            storagelevel.ModifiedBy = User.FindFirst("UserName")?.Value;
            storagelevel.DateModified = DateTime.Now;

            TempData["success"] = "Level updated successfully";
            _unitOfWork.StorageLevel.Update(storagelevel);

            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index", new { RowCode = storagelevel.InvStorageColumn.RowCode, LevelCode = storagelevel.LevelCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertBin(string? BinCode, StorageViewModel model)
        {
            var storagelevel = await _unitOfWork.StorageLevel.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.LevelCode == model.invStorageLevel.LevelCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageBins).ThenInclude(m => m.InvStorageCode)
                    .Include(m => m.InvStorageColumn));

            if (storagelevel == null)
            {
                TempData["error"] = "Invalid model state!";
                return RedirectToAction("Index", "Dashboards");
            }

            var storagebin = storagelevel.InvStorageBins.SingleOrDefault(m => m.BinCode == BinCode);

            if (storagebin == null)
            {
                var Last = 1.ToString("D2");

                BinCode = model.invStorageLevel.LevelCode + Last;

                var check = await _unitOfWork.StorageBin.GetAllAsync(
                    filter:
                        m => m.BinCode == BinCode);

                if (check.Count > 0)
                {
                    int LastCount = int.Parse(check.Max(m => m.BinCode.Substring(model.invStorageLevel.LevelCode.Length)));
                    BinCode = model.invStorageLevel.LevelCode + (LastCount + 1).ToString("0#");
                }

                model.invStorageCode.InvStorageBin.LevelCode = model.invStorageLevel.LevelCode;
                model.invStorageCode.InvStorageBin.BinCode = BinCode;
                model.invStorageCode.InvStorageBin.BinName = storagelevel.LevelName + " / " + BinCode.Substring(storagelevel.LevelCode.Length);
                model.invStorageCode.InvStorageBin.DateCreated = DateTime.Now;
                model.invStorageCode.InvStorageBin.CreatedBy = User.FindFirst("UserName")?.Value;

                await _unitOfWork.StorageBin.AddAsync(model.invStorageCode.InvStorageBin);

                var storagecode = new InvStorageCode
                {
                    StorageCode = Guid.NewGuid(),
                    BinCode = BinCode,
                    StorageCategoryId = model.invStorageCode.StorageCategoryId,
                    SizeCode = model.invStorageCode.SizeCode,
                    CreatedBy = User.FindFirst("UserName")?.Value,
                    DateCreated = DateTime.Now
                };

                await _unitOfWork.StorageCode.AddAsync(storagecode);

                TempData["success"] = "Bin created successfully";
            }
            else
            {
                storagebin.BinName = storagelevel.LevelName + " / " + BinCode.Substring(storagelevel.LevelCode.Length);
                storagebin.Flag = model.invStorageCode.InvStorageBin.Flag;

                _unitOfWork.StorageBin.Update(storagebin);

                storagebin.InvStorageCode.SizeCode = model.invStorageCode.SizeCode;
                storagebin.InvStorageCode.StorageCategoryId = model.invStorageCode.StorageCategoryId;

                _unitOfWork.StorageCode.Update(storagebin.InvStorageCode);
                TempData["success"] = "Bin updated successfully";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index", new { RowCode = storagelevel.InvStorageColumn.RowCode, LevelCode = storagelevel.LevelCode });
        }


        [HttpGet]
        public async Task<JsonResult> GetLevels(string rowCode)
        {
            var temp = await _unitOfWork.StorageLevel.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.InvStorageSection),
                filter:
                    x => x.InvStorageSection.RowCode == rowCode);
            return Json(temp);
        }

        [HttpGet]
        public async Task<JsonResult> GetLevelByRowCode(string RowCode)
        {
            var model = await _unitOfWork.StorageLevel.GetAllAsync(
                filter:
                    m => m.InvStorageColumn.RowCode == RowCode ||
                    m.InvStorageSection.RowCode == RowCode);

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetLevelByLevelCode(string LevelCode)
        {
            var model = await _unitOfWork.StorageLevel.GetSingleOrDefaultAsync(
                filter:
                    m => m.LevelCode == LevelCode);

            return Json(model);
        }
    }
}