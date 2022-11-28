using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Utility;
using WMS.Models.ViewModels;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class DashboardsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardsController(IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? Year)
        {
            var ProfileId = User.FindFirst("ProfileId").Value;
            var HouseCode = User.FindFirst("HouseCode").Value;
            var UserId = User.FindFirst("UserId").Value;

            if (Year == null)
            {
                Year = DateTime.Now;
            }

            var ProductStock = await _unitOfWork.ProductStock.GetAllAsync();

            var DeliveryOrder = await _unitOfWork.DeliveryOrder.GetAllAsync(
                filter:
                    m => m.DateDelivered.Value.Year == Year.Value.Year,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts.Where(m => m.IncDeliveryOrderArrivals != null))
                    .ThenInclude(m => m.IncDeliveryOrderArrivals));

            var SalesOrder = await _unitOfWork.SalesOrder.GetAllAsync(
                filter:
                    m => m.DateOrdered.Year == Year.Value.Year,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts.Where(m => m.Flag >= SD.FlagSOProduct_Picked)));

            var Warehouse = await _unitOfWork.HouseCode.GetAllAsync();

            if (ProfileId == SD.Role_Tenant)
            {
                var userwarehouse = await _unitOfWork.UserWarehouse.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                var usertenant = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                var producttenant = await _unitOfWork.Product.GetAllAsync(
                    filter:
                        m => usertenant.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString()));

                ProductStock = ProductStock.Where(m => userwarehouse.Select(m => m.HouseCode).Contains(m.HouseCode) &&
                                producttenant.Select(m => m.ProductId).Contains(m.ProductId)).ToList();

                DeliveryOrder = DeliveryOrder.Where(m => userwarehouse.Select(m => m.HouseCode).Contains(m.HouseCode) &&
                                usertenant.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString())).ToList();

                SalesOrder = SalesOrder.Where(m => userwarehouse.Select(m => m.HouseCode).Contains(m.HouseCode) &&
                                usertenant.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString())).ToList();

                Warehouse = Warehouse.Where(m => userwarehouse.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }
            else if (ProfileId == SD.Role_WarehouseAdmin)
            {
                ProductStock = ProductStock.Where(m => m.HouseCode == HouseCode).ToList();
                DeliveryOrder = DeliveryOrder.Where(m => m.HouseCode == HouseCode).ToList();
                SalesOrder = SalesOrder.Where(m => m.HouseCode == HouseCode).ToList();
                Warehouse = Warehouse.Where(m => m.HouseCode == HouseCode).ToList();
            }

            var model = new DashboardViewModel();

            foreach (var item in Warehouse)
            {
                model.Dashboard.Add(new Dashboard1
                {
                    HouseCode = item.HouseCode,
                    HouseName = item.HouseName,
                    Incoming = DeliveryOrder.Where(m => m.HouseCode == item.HouseCode).Count(),
                    IncomingProduct = DeliveryOrder.Where(m => m.HouseCode == item.HouseCode).Sum(m => m.IncDeliveryOrderProducts.Count()),
                    IncomingQuantity = DeliveryOrder.Where(m => m.HouseCode == item.HouseCode).Sum(m => m.IncDeliveryOrderProducts.Sum(m => m.IncDeliveryOrderArrivals.Quantity)),
                    Inventory = ProductStock.Where(m => m.HouseCode == item.HouseCode).Sum(m => m.Stock) + ProductStock.Where(m => m.HouseCode == item.HouseCode).Sum(m => m.QtyOrder),
                    Outgoing = SalesOrder.Where(m => m.HouseCode == item.HouseCode).Count(),
                    OutgoingProduct = SalesOrder.Where(m => m.HouseCode == item.HouseCode).Sum(m => m.OutSalesOrderProducts.Count()),
                    OutgoingQuantity = SalesOrder.Where(m => m.HouseCode == item.HouseCode).Sum(m => m.OutSalesOrderProducts.Sum(m => m.Quantity))
                });
            }

            for (int i = 1; i <= 12; i++)
            {
                model.MonthlyIncoming.Add(DeliveryOrder.Where(m => m.DateDelivered.Value.Month == i).Count());
                model.MonthlyOutgoing.Add(SalesOrder.Where(m => m.DateOrdered.Month == i).Count());
            }

            return View(model);
        }
    }
}
