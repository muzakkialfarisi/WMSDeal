using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models.ViewModels;
using WMS.Utility;
using WMS.Models;

namespace App.WMSDeal.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class ReturnsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var model = await _unitOfWork.Return.GetAllAsync(
                filter:
                    m => m.HouseCode == HouseCode,
                includeProperties:
                    m => m.Include(m => m.MasHouseCode)
                    .Include(m => m.MasDataTenant));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string OrderId)
        {
            var model = new ReturnedViewModel();

            var HouseCode = User.FindFirst("HouseCode")?.Value;

            model.outSalesOrder = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
               filter:
                   m => m.OrderId == OrderId &&
                   m.HouseCode == HouseCode &&
                   m.Status >= SD.FlagSO_Ordered,
               includeProperties:
                   m => m.Include(m => m.MasDataTenant)
                   .Include(m => m.MasHouseCode)
                   .Include(m => m.MasSalesType)
                   .Include(m => m.MasPlatform)
                   .Include(m => m.OutSalesOrderProducts)
                       .ThenInclude(m => m.MasProductData)
                   .Include(m => m.OutSalesOrderProducts)
                       .ThenInclude(m => m.OutSalesOrderStorages)
                   .Include(m => m.OutSalesOrderCustomer.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                   .Include(m => m.OutSalesOrderConsignee.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi)
                   .Include(m => m.OutsalesOrderDelivery.MasSalesCourier));

            if (model.outSalesOrder == null)
            {
                TempData["error"] = "Sales Order NotFound!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReturnedViewModel model)
        {
            if(model.invReturn.InvReturnProducts.Count() < 1)
            {
                TempData["error"] = "Product Notfound!";
                return View(model);
            }

            var Code = "RD";
            var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
            var Last = 1.ToString("D4");

            string DONumber = Code + Tanggal + Last;
            if (await _unitOfWork.DeliveryOrder.AnyAsync(m => m.DONumber == DONumber))
            {
                var checker = await _unitOfWork.DeliveryOrder.GetAllAsync(m => m.DONumber.Contains(Code + Tanggal));
                int LastCount = int.Parse(checker.Max(m => m.DONumber.Substring(Code.Length + Tanggal.Length)));
                DONumber = Code + Tanggal + (LastCount + 1).ToString("000#");
            }

            model.outSalesOrder = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == model.outSalesOrder.OrderId);

            model.invReturn.ReturnNumber = DONumber;
            model.invReturn.CreatedBy = User.FindFirst("UserName")?.Value;
            model.invReturn.DateCreated = DateTime.Now;
            model.invReturn.HouseCode = User.FindFirst("HouseCode")?.Value;
            model.invReturn.TenantId = model.outSalesOrder.TenantId;
            model.invReturn.Flag = 1;

            await _unitOfWork.Return.AddAsync(model.invReturn);

            for(int i = 0; i < model.invReturn.InvReturnProducts.Count; i++)
            {
                model.invReturn.InvReturnProducts[i].ReturnNumber = DONumber;

                await _unitOfWork.ReturnProduct.AddAsync(model.invReturn.InvReturnProducts[i]);
            }

            var DeliveryCourierId = await _unitOfWork.DeliveryOrderCourier.GetSingleOrDefaultAsync(m => m.Name == "Antar Sendiri");

            var deliveryOrder = new IncDeliveryOrder();
            deliveryOrder.DONumber = DONumber;
            deliveryOrder.DOSupplier = DONumber;
            deliveryOrder.HouseCode = User.FindFirst("HouseCode")?.Value;
            deliveryOrder.TenantId = model.outSalesOrder.TenantId;
            deliveryOrder.DateDelivered = model.invReturn.DateReceived;
            deliveryOrder.DeliveryCourierId = DeliveryCourierId.Id;
            deliveryOrder.ShippingCost = 0;
            deliveryOrder.Note = model.invReturn.Description;
            deliveryOrder.Status = SD.FlagDO_DO;
            deliveryOrder.CreatedBy = User.FindFirst("UserName")?.Value;
            deliveryOrder.DateDelivered = model.invReturn.DateReceived;

            await _unitOfWork.DeliveryOrder.AddAsync(deliveryOrder);

            for (int i = 0; i < model.invReturn.InvReturnProducts.Count; i++)
            {
                var DeliveryOrderProduct = new IncDeliveryOrderProduct();
                DeliveryOrderProduct.DONumber = DONumber;
                DeliveryOrderProduct.DOProductCode = Guid.NewGuid();
                DeliveryOrderProduct.Quantity = model.invReturn.InvReturnProducts[i].Quantity;
                DeliveryOrderProduct.ProductId = model.invReturn.InvReturnProducts[i].ProductId;
                DeliveryOrderProduct.DateOfExpired = model.invReturn.InvReturnProducts[i].DateOfExpired;
                DeliveryOrderProduct.UnitPrice = model.invReturn.InvReturnProducts[i].UnitPrice;
                DeliveryOrderProduct.SubTotal = model.invReturn.InvReturnProducts[i].Quantity * model.invReturn.InvReturnProducts[i].UnitPrice;
                DeliveryOrderProduct.Status = "Booked";

                await _unitOfWork.DeliveryOrderProduct.AddAsync(DeliveryOrderProduct);

                DeliveryOrderProduct.MasProductData = await _unitOfWork.Product.GetSingleOrDefaultAsync(m => m.ProductId == DeliveryOrderProduct.ProductId);
                deliveryOrder.IncDeliveryOrderProducts.Append(DeliveryOrderProduct);
            }

            var temp = deliveryOrder;

            var ikuproducts = temp.IncDeliveryOrderProducts.Where(m => m.MasProductData.ProductLevel == "IKU");
            var skuproducts = temp.IncDeliveryOrderProducts.Where(m => m.MasProductData.ProductLevel == "SKU");

            var ProductGroupIKU = ikuproducts.GroupBy(m => new { m.MasProductData.ZoneCode, m.MasProductData.SizeCode })
                                                                .Select(m => new { ct = m.Count(), code = m.Key }).ToArray();

            var ProductGroupSKU = skuproducts.GroupBy(m => new { m.MasProductData.ZoneCode, m.MasProductData.SizeCode })
                                                                .Select(m => new { ct = m.Count(), code = m.Key }).ToArray();

            if (!ProductGroupIKU.Any() && !ProductGroupSKU.Any())
            {
                TempData["error"] = "Something Wrong!";
                return RedirectToAction("Create", new { OrderId = model.outSalesOrder.OrderId });
            }

            int[] CountProductGroup = new int[ProductGroupIKU.ToList().Count];
            int[] ArrQtyStorage = new int[ProductGroupIKU.ToList().Count];

            for (int i = 0; i < ProductGroupIKU.ToList().Count; i++)
            {
                ArrQtyStorage[i] = await CheckStorageCode(temp.HouseCode, ProductGroupIKU[i].code.ZoneCode, ProductGroupIKU[i].code.SizeCode);

                CountProductGroup[i] = CountProductGroup[i] + ikuproducts.Where(m => m.MasProductData.ZoneCode == ProductGroupIKU[i].code.ZoneCode)
                                                                            .Where(m => m.MasProductData.SizeCode == ProductGroupIKU[i].code.SizeCode).Sum(m => m.Quantity);

                if (CountProductGroup[i] > ArrQtyStorage[i])
                {
                    TempData["error"] = "Over Quantity!";
                    return RedirectToAction("Create", new { OrderId = model.outSalesOrder.OrderId });
                }
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "ReturedSuccessfully!";
            return RedirectToAction("Index");
        }

        private async Task<int> CheckStorageCode(string HouseCode, string ZoneCode, string SizeCode)
        {
            var temp = await _unitOfWork.StorageCode.CountAsync(
                filter:
                    m => m.InvStorageBin.InvStorageLevel.InvStorageSection.InvStorageRow.ZoneCode == ZoneCode &&
                    m.InvStorageBin.InvStorageLevel.InvStorageSection.InvStorageRow.HouseCode == HouseCode &&
                    m.SizeCode == SizeCode &&
                    m.Flag == 1);

            return temp;
        }
    }
}