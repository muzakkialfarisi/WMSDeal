#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class DivisionController : Controller
    {
        private readonly AppDbContext _context;

        public DivisionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int DivId)
        {
            var model = await _context.MasDivisions.AsNoTracking()
                .Include(m => m.MasJabatans)
                .SingleOrDefaultAsync(
                    m => m.DivId == DivId);

            if (model == null)
            {
                TempData["error"] = "Division Notfund!";
                return RedirectToAction("Index", "Directorate");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertJabatan(int JobPosId, MasJabatan model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid ModelState!";
                return RedirectToAction("Index", "Directorate");
            }

            var result = await _context.MasJabatans.SingleOrDefaultAsync(
                m => m.JobPosId == JobPosId);

            var codechecker = await _context.MasJabatans.AsNoTracking().Where(
                m => m.JobPosCode == model.JobPosCode &&
                m.JobPosId != JobPosId).ToListAsync();

            if (codechecker.Count > 0)
            {
                TempData["error"] = "Code already used!";
                return RedirectToAction("Detail", new { DivId = model.DivId });
            }

            var update = true;

            if (result == null)
            {
                result = new MasJabatan();

                result.DivId = model.DivId;

                update = false;
            }

            result.JobPosCode = model.JobPosCode.Trim().ToUpper();
            result.JobPosName = model.JobPosName;
            result.Flag = model.Flag;

            if (update == true)
            {
                _context.MasJabatans.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _context.MasJabatans.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { DivId = model.DivId });
        }
    }
}
