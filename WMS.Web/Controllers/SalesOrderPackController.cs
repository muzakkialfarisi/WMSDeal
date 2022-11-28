using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Models;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using WMS.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using WMS.DataAccess;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "AdminWarehouse")]
    public class SalesOrderPackController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;

        public SalesOrderPackController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var model = await _unitOfWork.SalesOrder.GetAllAsync(
                filter:
                    m => m.Status == SD.FlagSO_Staged,
                includeProperties:
                    m => m.Include(m => m.MasSalesType)
                    .Include(m => m.MasDataTenant)
                    .Include(M => M.MasHouseCode)
                    .Include(m => m.OutSalesOrderProducts));

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string OrderId)
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var ProfileId = User.FindFirst("ProfileId")?.Value;

            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId && 
                    m.Status == SD.FlagSO_Staged,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode)
                    .Include(m => m.MasSalesType)
                    .Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData)
                    .Include(m => m.OutSalesOrderProducts)
                        .ThenInclude(m => m.IncSerialNumbers)
                    .Include(m => m.OutsalesOrderDelivery.MasSalesCourier));

            if(model == null)
            {
                TempData["error"] = "Sales Order Notfound!";
                return RedirectToAction("Index");
            }

            if (ProfileId == SD.Role_WarehouseAdmin)
            {
                if (model.HouseCode != HouseCode)
                {
                    TempData["error"] = "Sales Order Notfound!";
                    return RedirectToAction("Index");
                }
            }

            ViewData["PackTypeId"] = new SelectList(_context.MasPackingType, "PackTypeId", "PackTypeName");

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string OrderId, OutSalesOrder model)
        {
            if(model.OutSalesOrderProducts == null)
            {
                TempData["error"] = "Product Notfound!";
                return RedirectToAction("Create", new { OrderId = OrderId });
            }

            ViewData["PackTypeId"] = new SelectList(_context.MasPackingType, "PackTypeId", "PackTypeName");

            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var order = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.OrderId == OrderId &&
                    m.Status == SD.FlagSO_Staged &&
                    m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProducts)
                            .ThenInclude(m => m.IncSerialNumbers)
                        .Include(m => m.OutSalesOrderProducts)
                            .ThenInclude(m => m.MasProductData));

            if (order == null)
            {
                TempData["error"] = "OrderId Notfound!";
                return RedirectToAction("Index");
            }

            if (model.OutSalesOrderProducts.Count() != order.OutSalesOrderProducts.Count())
            {
                TempData["error"] = "Unbalanced Quantity!";
                return RedirectToAction("Create", new { OrderId = OrderId });
            }

            for (int i = 0; i < model.OutSalesOrderProducts.Count; i++)
            {
                var outSalesOrderPack = new OutSalesOrderPack()
                {
                    OrdProductId = model.OutSalesOrderProducts[i].OrdProductId,
                    DatePacked = DateTime.Now,
                    PackedBy = User.FindFirst("UserName")?.Value,
                    PackTypeId = model.OutSalesOrderProducts[i].OutSalesOrderPack.PackTypeId
                };
                await _unitOfWork.SalesOrderPack.AddAsync(outSalesOrderPack);

                var product = order.OutSalesOrderProducts.SingleOrDefault(m => m.OrdProductId == outSalesOrderPack.OrdProductId);

                if (product == null)
                {
                    TempData["error"] = "product Notfound!";
                    return RedirectToAction("Create", new { OrderId = OrderId });
                }

                if(product.MasProductData.ProductLevel == SD.ProductLvl_SKU && product.MasProductData.SerialNumber == "SN")
                {
                    if(product.IncSerialNumbers.Count != product.Quantity)
                    {
                        TempData["warning"] = product.MasProductData.ProductName + " Required Serial Number!";
                        return RedirectToAction("Create", new { OrderId = OrderId });
                    }
                }

                product.Flag = SD.FlagSOProduct_Packed;
                _unitOfWork.SalesOrderProduct.Update(product);
            }

            order.Status = SD.FlagSO_Packed;
            _unitOfWork.SalesOrder.Update(order);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Sales Order Packed Successfully!";
            return RedirectToAction("Index");
        }
    }
}
