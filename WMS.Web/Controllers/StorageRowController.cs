using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.Utility;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class StorageRowController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorageRowController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            StorageViewModel model = new StorageViewModel();
            model.invStorageRows = await _unitOfWork.StorageRow.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasHouseCode)
                    .Include(m => m.InvStorageZone));

            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                warehouses = warehouses.Where(m => m.HouseCode == HouseCode).ToList();
                model.invStorageRows = model.invStorageRows.Where(m => m.HouseCode == HouseCode).ToList();
            }

            ViewData["HouseCode"] = new SelectList(warehouses, "HouseCode", "HouseName");
            ViewData["ZoneCode"] = new SelectList(await _unitOfWork.StorageZone.GetAllAsync(), "ZoneCode", "ZoneName");

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertRow(string? RowCode)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            StorageViewModel model = new StorageViewModel();
            model.invStorageRow = await _unitOfWork.StorageRow.GetSingleOrDefaultAsync(
                filter:
                    m => m.RowCode == RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageZone)
                    .Include(m => m.InvStorageColumns)
                    .Include(m => m.InvStorageSections));

            if (model.invStorageRow == null)
            {
                TempData["error"] = "Storage Row notfound!";
                return RedirectToAction("Index");
            }

            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                warehouses = warehouses.Where(m => m.HouseCode == HouseCode).ToList();
                if (model.invStorageRow.HouseCode != HouseCode)
                {
                    TempData["error"] = "Storage Row notfound!";
                    return RedirectToAction("Index");
                }
            }

            model.invStorageLevels = await _unitOfWork.StorageLevel.GetAllAsync(
                filter:
                    m => m.InvStorageColumn.RowCode == RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageBins));

            ViewData["HouseCode"] = new SelectList(warehouses, "HouseCode", "HouseName", model.invStorageRow.HouseCode);
            ViewData["ZoneCode"] = new SelectList(await _unitOfWork.StorageZone.GetAllAsync(), "ZoneCode", "ZoneName", model.invStorageRow.ZoneCode);

            var sectioncode = await _unitOfWork.StorageSection.GetAllAsync(
                filter:
                    m => m.RowCode == model.invStorageRow.RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageRow));
            ViewData["SectionCode"] = sectioncode
                                    .Select(m => new SelectListItem
                                    {
                                        Value = m.SectionCode.ToString().Trim(),
                                        Text = string.Concat(m.InvStorageRow.HouseCode + " - " +
                                        m.InvStorageRow.ZoneCode + " - " +
                                        m.RowCode.Substring(m.InvStorageRow.HouseCode.Length + m.InvStorageRow.ZoneCode.Length) + " - " +
                                        m.SectionCode.Substring(m.RowCode.Length))
                                    }).ToList();

            var columncode = await _unitOfWork.StorageColumn.GetAllAsync(
                filter:
                    m => m.RowCode == model.invStorageRow.RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageRow));

            ViewData["ColumnCode"] = columncode
                                    .Select(m => new SelectListItem
                                    {
                                        Value = m.ColumnCode.ToString().Trim(),
                                        Text = string.Concat(m.InvStorageRow.HouseCode + " - " +
                                        m.InvStorageRow.ZoneCode + " - " +
                                        m.RowCode.Substring(m.InvStorageRow.HouseCode.Length + m.InvStorageRow.ZoneCode.Length) + " - " +
                                        m.ColumnCode.Substring(m.RowCode.Length))
                                    }).ToList();

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
        public async Task<IActionResult> UpsertRow(StorageViewModel model)
        {
            if (model.invStorageRow == null)
            {
                TempData["error"] = "Invalid Model State!";
                return RedirectToAction("Index");
            }

            var RowCode = model.invStorageRow.HouseCode + 
                model.invStorageRow.ZoneCode + 
                model.invStorageRow.RowCode.Trim().ToUpper();

            var storagerow = await _unitOfWork.StorageRow.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RowCode == RowCode);

            if (storagerow == null)
            {
                model.invStorageRow.RowCode = RowCode;
                model.invStorageRow.DateCreated = DateTime.Now;
                model.invStorageRow.CreatedBy = User.FindFirst("UserName")?.Value;
                model.invStorageRow.RowPlanPhoto = "default.jpg";

                await _unitOfWork.StorageRow.AddAsync(model.invStorageRow);
                TempData["success"] = "Row created successfully";
            }
            else
            {
                storagerow.RowName = model.invStorageRow.RowName;
                storagerow.Flag = model.invStorageRow.Flag;
                storagerow.ZoneCode = model.invStorageRow.ZoneCode;

                _unitOfWork.StorageRow.Update(storagerow);
                TempData["success"] = "Row updated successfully";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("UpsertRow", new { RowCode = RowCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertSection(StorageViewModel model)
        {
            var storagerow = await _unitOfWork.StorageRow.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RowCode == model.invStorageSection.RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageSections));

            if (storagerow == null)
            {
                TempData["error"] = "Storage Row notfound!";
                return RedirectToAction("Index");
            }

            var SectionCode  = model.invStorageSection.SectionCode;

            if (model.invStorageSection.SectionCode == "Auto Generated")
            {
                var Last = 1.ToString("D1");

                SectionCode = model.invStorageSection.RowCode + "S" + Last;

                var check = await _unitOfWork.StorageSection.GetAllAsync(
                    filter:
                        m => m.SectionCode.Contains(model.invStorageSection.RowCode + "S"));

                if (check.Count > 0)
                {
                    int LastCount = int.Parse(check.Max(m => m.SectionCode.Substring(model.invStorageSection.RowCode.Length + 1)));
                    if (LastCount >= 9)
                    {
                        TempData["error"] = "Maksimum 9 Section!";
                        return RedirectToAction("UpsertRow", new { RowCode = model.invStorageSection.RowCode });
                    }
                    SectionCode = model.invStorageSection.RowCode + "S" + (LastCount + 1).ToString();
                }

                model.invStorageSection.SectionCode = SectionCode;
                model.invStorageSection.SectionName = storagerow.HouseCode + " - " +
                    storagerow.ZoneCode + " - " +
                    storagerow.RowCode.Substring(storagerow.HouseCode.Length + storagerow.ZoneCode.Length) + " - " +
                    SectionCode.Substring(storagerow.RowCode.Length);
                model.invStorageSection.DateCreated = DateTime.Now;
                model.invStorageSection.CreatedBy = User.FindFirst("UserName")?.Value;

                TempData["success"] = "Section created successfully";
                await _unitOfWork.StorageSection.AddAsync(model.invStorageSection);
            }
            else
            {
                var storagesection = storagerow.InvStorageSections.SingleOrDefault(m => m.SectionCode == SectionCode);

                storagesection.SectionName = storagerow.HouseCode + " - " +
                    storagerow.ZoneCode + " - " +
                    storagerow.RowCode.Substring(storagerow.HouseCode.Length + storagerow.ZoneCode.Length) + " - " +
                    SectionCode.Substring(storagerow.RowCode.Length);
                storagesection.Flag = model.invStorageSection.Flag;

                TempData["success"] = "Section updated successfully";
                _unitOfWork.StorageSection.Update(storagesection);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("UpsertRow", new { RowCode = model.invStorageSection.RowCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertColumn(StorageViewModel model)
        {
            var storagerow = await _unitOfWork.StorageRow.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.RowCode == model.invStorageColumn.RowCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageColumns));

            if (storagerow == null)
            {
                TempData["error"] = "Storage Row notfound!";
                return RedirectToAction("Index");
            }

            var ColumnCode = model.invStorageColumn.ColumnCode;


            if (ColumnCode == "Auto Generated")
            {
                var Last = 1.ToString("D1");

                ColumnCode = model.invStorageColumn.RowCode + "C" + Last;

                var check = await _unitOfWork.StorageColumn.GetAllAsync(
                    filter:
                        m => m.ColumnCode.Contains(model.invStorageColumn.RowCode + "C"));

                if (check.Count > 0)
                {
                    int LastCount = int.Parse(check.Max(m => m.ColumnCode.Substring(model.invStorageColumn.RowCode.Length + 1)));
                    if (LastCount >= 99)
                    {
                        TempData["error"] = "Maksimum 99 Column!";
                        return RedirectToAction("UpsertRow", new { RowCode = model.invStorageColumn.RowCode });
                    }
                    ColumnCode = model.invStorageColumn.RowCode + "C" + (LastCount + 1).ToString();
                }

                model.invStorageColumn.ColumnCode = ColumnCode;
                model.invStorageColumn.ColumnName = storagerow.HouseCode + " - " +
                    storagerow.ZoneCode + " - " +
                    storagerow.RowCode.Substring(storagerow.HouseCode.Length + storagerow.ZoneCode.Length) + " - " +
                    ColumnCode.Substring(storagerow.RowCode.Length);
                model.invStorageColumn.DateCreated = DateTime.Now;
                model.invStorageColumn.CreatedBy = User.FindFirst("UserName")?.Value;

                TempData["success"] = "Section created successfully";
                await _unitOfWork.StorageColumn.AddAsync(model.invStorageColumn);
            }
            else
            {
                var storagecolumn = storagerow.InvStorageColumns.SingleOrDefault(m => m.ColumnCode == ColumnCode);

                storagecolumn.ColumnName = storagerow.HouseCode + " - " +
                    storagerow.ZoneCode + " - " +
                    storagerow.RowCode.Substring(storagerow.HouseCode.Length + storagerow.ZoneCode.Length) + " - " +
                    ColumnCode.Substring(storagerow.RowCode.Length);
                storagecolumn.Flag = model.invStorageColumn.Flag;

                TempData["success"] = "Section updated successfully";
                _unitOfWork.StorageColumn.Update(storagecolumn);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("UpsertRow", new { RowCode = model.invStorageColumn.RowCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertLevel(StorageViewModel model)
        {
            var storagesection = await _unitOfWork.StorageSection.GetSingleOrDefaultAsync(
                filter:
                    m => m.SectionCode == model.invStorageLevel.SectionCode);

            var storagecolumn = await _unitOfWork.StorageColumn.GetSingleOrDefaultAsync(
                filter:
                    m => m.ColumnCode == model.invStorageLevel.ColumnCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageRow));

            if (storagesection == null || storagecolumn == null)
            {
                TempData["error"] = "Invalid model state!";
                return RedirectToAction("Index");
            }

            var LevelCode = model.invStorageLevel.SectionCode + 
                model.invStorageLevel.ColumnCode.Substring(storagecolumn.RowCode.Length);

            var storagelevel = await _unitOfWork.StorageLevel.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.LevelCode == LevelCode);

            if (storagelevel == null)
            {
                model.invStorageLevel.LevelName = storagecolumn.InvStorageRow.HouseCode + " - " +
                    storagecolumn.InvStorageRow.ZoneCode + " - " +
                    storagecolumn.RowCode.Substring(storagecolumn.InvStorageRow.HouseCode.Length + storagecolumn.InvStorageRow.ZoneCode.Length) + " - " +
                    LevelCode.Substring(storagecolumn.RowCode.Length);
                model.invStorageLevel.CreatedBy = User.FindFirst("UserName")?.Value;
                model.invStorageLevel.DateCreated = DateTime.Now;

                TempData["success"] = "Level created successfully";
                await _unitOfWork.StorageLevel.AddAsync(model.invStorageLevel);
            }
            else 
            {
                storagelevel.LevelName = storagecolumn.InvStorageRow.HouseCode + " - " +
                    storagecolumn.InvStorageRow.ZoneCode + " - " +
                    storagecolumn.RowCode.Substring(storagecolumn.InvStorageRow.HouseCode.Length + storagecolumn.InvStorageRow.ZoneCode.Length) + " - " +
                    LevelCode.Substring(storagecolumn.RowCode.Length);
                storagelevel.Flag = model.invStorageLevel.Flag;
                storagelevel.ModifiedBy = User.FindFirst("UserName")?.Value;
                storagelevel.DateModified = DateTime.Now;

                TempData["success"] = "Level updated successfully";
                _unitOfWork.StorageLevel.Update(storagelevel);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("UpsertRow", new { RowCode = storagecolumn.RowCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertBin(string? BinCode, StorageViewModel model)
        {
            var storagelevel = await _unitOfWork.StorageLevel.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.LevelCode == model.invStorageCode.InvStorageBin.LevelCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageBins).ThenInclude(m => m.InvStorageCode)
                    .Include(m => m.InvStorageColumn));

            if (storagelevel == null)
            {
                TempData["error"] = "Invalid model state!";
                return RedirectToAction("Index");
            }

            if (BinCode == null)
            {
                var Last = 1.ToString("D2");

                BinCode = model.invStorageCode.InvStorageBin.LevelCode + Last;

                var check = await _unitOfWork.StorageBin.GetAllAsync(
                    filter:
                        m => m.BinCode == BinCode);

                if (check.Count > 0)
                {
                    int LastCount = int.Parse(check.Max(m => m.BinCode.Substring(model.invStorageCode.InvStorageBin.LevelCode.Length)));
                    BinCode = model.invStorageCode.InvStorageBin.LevelCode + (LastCount + 1).ToString("0#");
                }

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

            await _unitOfWork.SaveAsync();
            return RedirectToAction("UpsertRow", new { RowCode = storagelevel.InvStorageColumn.RowCode });
        }

        [HttpGet]
        public async Task<JsonResult> GetRowByHouseCodeByZoneCode(string HouseCode, string ZoneCode)
        {
            var model = await _unitOfWork.StorageRow.GetAllAsync(
                filter:
                    m => m.HouseCode == HouseCode &&
                    m.ZoneCode == ZoneCode);

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetRowByRowCode(string RowCode)
        {
            var model = await _unitOfWork.StorageRow.GetSingleOrDefaultAsync(
                filter:
                    m => m.RowCode == RowCode);

            return Json(model);
        }
    }
}
