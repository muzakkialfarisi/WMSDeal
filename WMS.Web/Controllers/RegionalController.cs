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
    public class RegionalController : Controller
    {
        private readonly AppDbContext _context;

        public RegionalController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            return View(await _context.MasRegionals.AsNoTracking().Include(m => m.MasProvinsis).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int RegionId, MasRegional model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var result = await _context.MasRegionals.SingleOrDefaultAsync(
                m => m.RegionId == RegionId);

            var codechecker = await _context.MasRegionals.AsNoTracking().Where(
                m => m.RegionCode == model.RegionCode &&
                m.RegionId != RegionId).ToListAsync();

            if (codechecker.Count > 0)
            {
                TempData["error"] = "Code Already Used!";
                return RedirectToAction("Index");
            }

            var update = true;

            if (result == null)
            {
                result = new MasRegional();

                update = false;
            }

            result.RegionCode = model.RegionCode.Trim().ToUpper();
            result.RegionName = model.RegionName;
            result.Flag = model.Flag;

            if (update == true)
            {
                _context.MasRegionals.Update(result);
                TempData["success"] = "Updated Successfully!!";
            }
            else
            {
                await _context.MasRegionals.AddAsync(result);
                TempData["success"] = "Created Successfully!!";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
