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
    public class SupplierTypeController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierTypeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _context.MasSupplierTypes.AsNoTracking().ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int TypeId, MasSupplierType model)
        {
            var result = await _context.MasSupplierTypes.SingleOrDefaultAsync(
            m => m.TypeId == TypeId);

            var update = true;

            if (result == null)
            {
                result = new MasSupplierType();
                update = false;
            }

            result.SupplierTypeCode = model.SupplierTypeCode.Trim().ToUpper();
            result.SupplierTypeName = model.SupplierTypeName.ToUpper();
            result.Flag = model.Flag;

            var checkname = await _context.MasSupplierTypes.AsNoTracking().Where(
                m => (m.SupplierTypeName == result.SupplierTypeName ||
                m.SupplierTypeCode == result.SupplierTypeCode) &&
                m.TypeId != TypeId).ToListAsync();

            if (checkname.Count > 0)
            {
                TempData["error"] = "Code or Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _context.MasSupplierTypes.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _context.MasSupplierTypes.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
