using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class TrackingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrackingController(IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? OrderId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.Status > 1,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts)
                    .Include(m => m.OutSalesOrderConsignee)
                    .Include(m => m.OutsalesOrderDelivery));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Tracker(string OrderId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId &&
                    m.Status > 1,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.MasPlatform)
                    .Include(m => m.OutSalesOrderProducts)
                    .ThenInclude(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                    .Include(m => m.OutsalesOrderDelivery.MasSalesCourier));

            if(model == null)
            {
                return Json(false);
            }

            return PartialView("Detail", model);
        }

    }
}
