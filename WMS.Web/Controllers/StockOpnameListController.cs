using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Utility;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class StockOpnameListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public StockOpnameListController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            var models = await _unitOfWork.StockOpname.GetAllAsync(
                filter:
                    m => m.Status == SD.FlagOpname_Done,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                models = models.Where(m => m.HouseCode == HouseCode).ToList();
            }
            else if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                    filter:
                        m => m.UserId == UserId);

                models = models.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string OpnameId)
        {
            var model = await _unitOfWork.StockOpname.GetSingleOrDefaultAsync(
               filter:
                   m => m.OpnameId == OpnameId,
               includeProperties:
                   m => m.Include(m => m.InvStockOpnameProducts).ThenInclude(m => m.MasProductData)
                    .Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));

            if (model == null)
            {
                TempData["error"] = "Opname not found!";
                return RedirectToAction("Index");
            }

            if (model.Status != SD.FlagOpname_Done)
            {
                TempData["error"] = "Opname not finished!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

    }
}