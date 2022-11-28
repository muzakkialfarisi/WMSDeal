using WMS.DataAccess;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class DirectorateController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public DirectorateController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var model = await _context.MasDirectorates.AsNoTracking()
                .Include(m => m.MasDivisions).ToListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string DirCode, MasDirectorate model)
        {
            var result = await _context.MasDirectorates.SingleOrDefaultAsync(
                m => m.DirCode.Trim().ToUpper() == DirCode.Trim().ToUpper());

            var update = true;

            if (result == null)
            {
                result = new MasDirectorate();

                result.DirCode = model.DirCode.Trim().ToUpper();

                update = false;
            }

            result.DirName = model.DirName;

            if (update == true)
            {
                _context.MasDirectorates.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _context.MasDirectorates.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string DirCode)
        { 
            var model = await _context.MasDirectorates.AsNoTracking()
                .Include(m => m.MasDivisions)
                    .ThenInclude(m => m.MasJabatans)
                .SingleOrDefaultAsync(
                    m => m.DirCode == DirCode);

            if (model == null)
            {
                TempData["error"] = "Directorate Notfound!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertDivision(int DivId, MasDivision model)
        {
            if (model == null)
            {
                TempData["error"] = "Division Notfund!";
                return RedirectToAction("Index", "Directorate");
            }

            var result = await _context.MasDivisions.SingleOrDefaultAsync(
                m => m.DivId == DivId);

            var codechecker = await _context.MasDivisions.AsNoTracking().Where(
                m => m.DivCode == model.DivCode &&
                m.DivId != DivId).ToListAsync();

            if (codechecker.Count > 0)
            {
                TempData["error"] = "Code already used!";
                return RedirectToAction("Detail", new { DirCode = model.DirCode });
            }

            var update = true;

            if (result == null)
            {
                result = new MasDivision();

                result.DirCode = model.DirCode.Trim().ToUpper();

                update = false;
            }

            result.DivCode = model.DivCode.Trim().ToUpper();
            result.DivName = model.DivName;
            result.Flag = model.Flag;

            if (update == true)
            {
                _context.MasDivisions.Update(result);
                TempData["success"] = "Updated Successfully!";
            }
            else
            {
                await _context.MasDivisions.AddAsync(result);
                TempData["success"] = "Added Successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { DirCode = model.DirCode });
        }
    }
}
