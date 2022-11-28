using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using WMS.Models.ViewModels;
using WMS.Utility;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class SalesOrderDeliveriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public SalesOrderDeliveriesController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.SalesOrder.GetAllAsync(
                filter:
                    m => m.Status == SD.FlagSO_Dispatch &&
                    m.OutsalesOrderDelivery.AirwayBill == string.Empty,
                includeProperties:
                    m => m.Include(m => m.MasHouseCode)
                    .Include(m => m.MasDataTenant)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutsalesOrderDelivery.MasSalesCourier)
                    .Include(m => m.OutSalesOrderConsignee));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAirwayBill(OutsalesOrderDelivery model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var AirwayBill = model.AirwayBill.Trim();

            if (AirwayBill == null || AirwayBill == string.Empty)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            var delivery = await _unitOfWork.SalesOrderDelivery.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == model.OrderId);

            if (delivery == null)
            {
                TempData["error"] = "Sales Order not found!!";
                return RedirectToAction("Index");
            }

            delivery.AirwayBill = AirwayBill;
            _unitOfWork.SalesOrderDelivery.Update(delivery);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Airway Bill Updated Successfully!";
            return RedirectToAction("Index");
        }
    }
}
