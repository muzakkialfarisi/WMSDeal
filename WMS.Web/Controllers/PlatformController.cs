using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PlatformController : Controller
    {
        private readonly AppDbContext _context;

        public PlatformController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _context.MasPlatforms.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int PlatformId, MasPlatform model)
        {
            if(model == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var result = await _context.MasPlatforms.SingleOrDefaultAsync(
                m => m.PlatformId == PlatformId);

            var codechecker = await _context.MasPlatforms.AsNoTracking().Where(
                m => m.Code == model.Code &&
                m.PlatformId != PlatformId).ToListAsync();

            if (codechecker.Count > 0)
            {
                TempData["error"] = "Code Already Used!";
                return RedirectToAction("Index");
            }

            var update = true;

            if (result == null)
            {
                result = new MasPlatform();

                update = false;
            }

            result.Code = model.Code.Trim().ToUpper();
            result.Name = model.Name;
            result.Flag = model.Flag;

            if (update == true)
            {
                _context.MasPlatforms.Update(result);
                TempData["success"] = "Updated Successfully!!";
            }
            else 
            {
                await _context.MasPlatforms.AddAsync(result);
                TempData["success"] = "Created Successfully!!";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
