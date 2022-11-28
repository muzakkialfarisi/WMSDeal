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
    public class SupplierServiceController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierServiceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _context.MasSupplierServices.AsNoTracking().ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int ServiceId, MasSupplierService model)
        {
            var result = await _context.MasSupplierServices.SingleOrDefaultAsync(
                m => m.ServiceId == ServiceId);

            var update = true;

            if (result == null)
            {
                result = new MasSupplierService();
                update = false;
            }

            result.SupplierServiceCode = model.SupplierServiceCode.Trim().ToUpper();
            result.SupplierServiceName = model.SupplierServiceName.ToUpper();
            result.Flag = model.Flag;

            var checkname = await _context.MasSupplierServices.AsNoTracking().Where(
                m => (m.SupplierServiceCode == result.SupplierServiceCode ||
                m.SupplierServiceName == result.SupplierServiceName) &&
                m.ServiceId != ServiceId).ToListAsync();

            if (checkname.Count > 0)
            {
                TempData["error"] = "Code or Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _context.MasSupplierServices.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _context.MasSupplierServices.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
