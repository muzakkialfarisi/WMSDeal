#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class HouseCodeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HouseCodeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.HouseCode.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? HouseCode)
        {
            var model = await _unitOfWork.HouseCode.GetSingleOrDefaultAsync(
                filter:
                    m => m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));

            var provinsi = await _unitOfWork.Provinsi.GetAllAsync();
            var kabupaten = await _unitOfWork.Kabupaten.GetAllAsync();
            var kecamatan = await _unitOfWork.Kecamatan.GetAllAsync();
            var kelurahan = await _unitOfWork.Kelurahan.GetAllAsync();

            if (model != null)
            {
                kabupaten = kabupaten.Where(m => m.ProId == model.MasKelurahan.MasKecamatan.MasKabupaten.ProId).ToList();
                kecamatan = kecamatan.Where(m => m.KabId == model.MasKelurahan.MasKecamatan.KabId).ToList();
                kelurahan = kelurahan.Where(m => m.KecId == model.MasKelurahan.KecId).ToList();
            }
            else
            {
                kabupaten = kabupaten.Where(m => m.ProId == provinsi.Select(m => m.ProId).FirstOrDefault()).ToList();
                kecamatan = kecamatan.Where(m => m.KabId == kabupaten.Select(m => m.KabId).FirstOrDefault()).ToList();
                kelurahan = kelurahan.Where(m => m.KecId == kecamatan.Select(m => m.KecId).FirstOrDefault()).ToList();
            }

            ViewData["ProId"] = new SelectList(provinsi, "ProId", "ProName", model?.MasKelurahan.MasKecamatan.MasKabupaten.ProId);
            ViewData["KabId"] = new SelectList(kabupaten, "KabId", "KabName", model?.MasKelurahan.MasKecamatan.KabId);
            ViewData["KecId"] = new SelectList(kecamatan, "KecId", "KecName", model?.MasKelurahan.KecId);
            ViewData["KelId"] = new SelectList(kelurahan, "KelId", "KelName", model?.KelId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string? HouseCode, MasHouseCode model)
        {
            var result = await _unitOfWork.HouseCode.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.HouseCode == HouseCode);

            var update = true;

            if (result == null)
            {
                result = new MasHouseCode();
                result.HouseCode = model.HouseCode.Trim().ToUpper();

                update = false;
            }

            result.HouseName = model.HouseName;
            result.KelId = model.KelId;
            result.Address = model.Address;
            result.KodePos = model.KodePos;
            result.Email = model.Email;
            result.OfficePhone = model.OfficePhone;
            result.Fax = model.Fax;

            if (update == true)
            {
                _unitOfWork.HouseCode.Update(result);
                TempData["success"] = "Updated Successfully!";

            }
            else
            {
                await _unitOfWork.HouseCode.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }


        //[HttpGet]
        //public async Task<JsonResult> GetAllHouseCodes()
        //{
        //    var masHouseCode = await _context.MasHouseCodes.ToListAsync();
        //    return Json(masHouseCode);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetHouseCode(string HouseCode)
        //{
        //    var masHouseCode = await _context.MasHouseCodes.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi).SingleOrDefaultAsync(m => m.HouseCode == HouseCode);
        //    return Json(masHouseCode);
        //}
    }
}
