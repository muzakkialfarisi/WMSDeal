using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using WMS.Models.ViewModels;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class RepackingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork unitOfWork;

        public RepackingsController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
             this.unitOfWork = unitOfWork;
        }

        public async Task<ActionResult> Index()
        {
            RepackAndRelableViewModel repack = new RepackAndRelableViewModel();
            repack.invRepackings =  await _context.InvRepackings.Include(m => m.MasProductData).ToListAsync();
            return View(repack);
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RepackAndRelableViewModel model)
        {
            IncDeliveryOrderProduct incDeliveryOrderProduct = await _context.IncDeliveryOrderProducts.AsNoTracking().SingleOrDefaultAsync(m => m.DOProductId == model.invRepacking.ProductId);

            InvRepacking repack = new InvRepacking
            {
                DOProductId = model.invRepacking.ProductId,
                ProductId = (int)incDeliveryOrderProduct.ProductId,
                Quantity = model.invRepacking.Quantity,
                DateRepacked = DateTime.Now,
                RepackedBy = model.invRepacking.RepackedBy
            };

            await _context.InvRepackings.AddAsync(repack);
            await _context.SaveChangesAsync();

            TempData["success"] = "Success!";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            RepackAndRelableViewModel model = new RepackAndRelableViewModel();
            model.invRepacking = await _context.InvRepackings.SingleOrDefaultAsync(m => m.Id == id);

            if(model.invRepacking == null)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Index");
            }

            ViewData["ProductId"] = new SelectList(_context.InvRepackings.Include(m => m.MasProductData), "ProductId", "MasProductData.ProductName", model.invRepacking.ProductId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RepackAndRelableViewModel model)
        {
            var repack = await _context.InvRepackings.SingleOrDefaultAsync(m => m.Id == model.invRepacking.Id);

            if(null == repack)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Index");
            }

            var incdoproduct = await _context.IncDeliveryOrderProducts.SingleOrDefaultAsync(m => m.DOProductId == repack.DOProductId);

            var invrepack = await _context.InvRepackings.Where(m => m.DOProductId == repack.DOProductId).ToListAsync();

            if (model.invRepacking.Quantity > (incdoproduct.Quantity - (invrepack.Sum(m => m.Quantity) - repack.Quantity)))
            {
                TempData["error"] = "Over Quantity!";
                return RedirectToAction("Edit", new { id = model.invRepacking.Id });
            }

            repack.Quantity = model.invRepacking.Quantity;
            repack.DateRepacked = DateTime.Now;
            repack.RepackedBy = model.invRepacking.RepackedBy;

            await _context.SaveChangesAsync();

            TempData["success"] = "Success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetDOProductByDONumber(string DONumber)
        {

            List<IncDeliveryOrderProduct> products = await _context.IncDeliveryOrderProducts.AsNoTracking().Include(m => m.MasProductData)
                                                        .Where(app => app.DONumber == DONumber).Where(m => m.IncDeliveryOrder.Status == "DO").ToListAsync();
            return Json(products);
        }

        public async Task<IActionResult> GetProductByDOProductId(int DOProductId)
        {
            RepackAndRelableViewModel model = new RepackAndRelableViewModel();

            model.incDeliveryOrderProduct = await _context.IncDeliveryOrderProducts.AsNoTracking().Include(m => m.MasProductData).Include(m => m.IncDeliveryOrder)
                                                        .SingleOrDefaultAsync(app => app.DOProductId == DOProductId);
            model.invRepackings = await _context.InvRepackings.AsNoTracking().Where(m => m.DOProductId == DOProductId).ToListAsync();

            return Json(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RepackAndRelableViewModel model)
        {

            var pack = await _context.InvRepackings.SingleOrDefaultAsync(m => m.Id == model.invRepacking.Id);

            if (pack == null)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Index");
            }

            _context.InvRepackings.Remove(pack);
            await _context.SaveChangesAsync();
            TempData["success"] = "Success!";
            return RedirectToAction(nameof(Index));
        }
    }
}
