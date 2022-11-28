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
    public class RelabelingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public RelabelingsController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult> Index()
        {


            RepackAndRelableViewModel relabel = new RepackAndRelableViewModel();
            relabel.invRelabelings = await _context.InvRelabelings.Include(m => m.MasProductData).ToListAsync();
            return View(relabel);
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RepackAndRelableViewModel model)
        {
            IncDeliveryOrderProduct incDeliveryOrderProduct = await _context.IncDeliveryOrderProducts.AsNoTracking().SingleOrDefaultAsync(m => m.DOProductId == model.invRelabeling.ProductId);

            InvRelabeling relabel = new InvRelabeling
            {
                DOProductId = model.invRelabeling.ProductId,
                ProductId = (int)incDeliveryOrderProduct.ProductId,
                Quantity = model.invRelabeling.Quantity,
                DateRelabelled = DateTime.Now,
                RelabelledBy = model.invRelabeling.RelabelledBy
            };

            await _context.InvRelabelings.AddAsync(relabel);
            await _context.SaveChangesAsync();

            TempData["success"] = "Success!";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            RepackAndRelableViewModel model = new RepackAndRelableViewModel();
            model.invRelabeling = await _context.InvRelabelings.SingleOrDefaultAsync(m => m.Id == id);

            if (model.invRelabeling == null)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Index");
            }

            ViewData["ProductId"] = new SelectList(_context.InvRelabelings.Include(m => m.MasProductData), "ProductId", "MasProductData.ProductName", model.invRelabeling.ProductId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RepackAndRelableViewModel model)
        {
            var relabel = await _context.InvRelabelings.SingleOrDefaultAsync(m => m.Id == model.invRelabeling.Id);

            if (null == relabel)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Edit", new { id = model.invRelabeling.Id });
            }

            var incdoproduct = await _context.IncDeliveryOrderProducts.SingleOrDefaultAsync(m => m.DOProductId == relabel.DOProductId);

            var invrelabel = await _context.InvRelabelings.Where(m => m.DOProductId == relabel.DOProductId).ToListAsync();

            if (model.invRelabeling.Quantity > (incdoproduct.Quantity - (invrelabel.Sum(m => m.Quantity) - relabel.Quantity))){
                TempData["error"] = "Over Quantity!";
                return RedirectToAction("Edit", new { id = model.invRelabeling.Id });
            }

            relabel.Quantity = model.invRelabeling.Quantity;
            relabel.DateRelabelled = DateTime.Now;
            relabel.RelabelledBy = model.invRelabeling.RelabelledBy;

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
            model.invRelabelings = await _context.InvRelabelings.AsNoTracking().Where(m => m.DOProductId == DOProductId).ToListAsync();

            return Json(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RepackAndRelableViewModel model)
        {
            var label = await _context.InvRelabelings.SingleOrDefaultAsync(m => m.Id == model.invRelabeling.Id);

            if(label == null)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Index");
            }

            _context.InvRelabelings.Remove(label);
            await _context.SaveChangesAsync();
            TempData["success"] = "Success!";
            return RedirectToAction(nameof(Index));
        }
    }
}
