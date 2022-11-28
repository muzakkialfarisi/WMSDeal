#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class IndustryController : Controller
    {
        private readonly AppDbContext _context;

        public IndustryController(AppDbContext context)
        {
            _context = context;        
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _context.MasIndustries.AsNoTracking().ToListAsync();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int ID, MasIndustry model)
        {
            var result = await _context.MasIndustries.SingleOrDefaultAsync(
            m => m.ID == ID);

            var checkname = await _context.MasIndustries.AsNoTracking().Where(
                m => m.IndustryName == model.IndustryName &&
                m.ID != ID).ToListAsync();

            if (checkname.Count > 0)
            {
                TempData["error"] = "Name already used!";
                return RedirectToAction(nameof(Index));
            }

            var update = true;

            if (result == null)
            {
                result = new MasIndustry();

                result.IndustryCode = 1.ToString("D3");
                if (await _context.MasIndustries.AsNoTracking().AnyAsync(m => m.IndustryCode == result.IndustryCode))
                {
                    var checker = await _context.MasIndustries.AsNoTracking().ToListAsync();
                    int LastCount = int.Parse(checker.Max(m => m.IndustryCode));
                    result.IndustryCode = (LastCount + 1).ToString("00#");
                }

                update = false;
            }

            result.IndustryName = model.IndustryName;
            result.Flag = model.Flag;

            if (update == true)
            {
                _context.MasIndustries.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _context.MasIndustries.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
