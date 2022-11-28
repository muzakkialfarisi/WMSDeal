using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS.Utility;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "Tenant")]
    public class DeliveryOrderCreateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryOrderCreateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;
            var UserId = new Guid(User.FindFirst("UserId")?.Value);

            DeliveryOrderViewModel model = new DeliveryOrderViewModel();

            model.incDeliveryOrders = await _unitOfWork.DeliveryOrder.GetAllAsync(
                    filter:
                        m => m.Status == SD.FlagDO_OPN,
                    includeProperties:
                        m => m.Include(m => m.MasDataTenant)
                        .Include(m => m.MasHouseCode)
                        .Include(m => m.MasDeliveryOrderCourier)
                        .Include(m => m.IncDeliveryOrderProducts));

            model.incPurchaseOrders = await _unitOfWork.PurchaseOrder.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasDataTenantWarehouse.MasHouseCode)
                    .Include(m => m.IncPurchaseOrderProducts)
                    .Include(m => m.IncRequestPurchase),
                filter:
                    m => m.Status == "Pending");

            if (ProfileId == SD.Role_Tenant)
            {
                var userWarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                    filter:
                        m => m.UserId == UserId);

                model.incDeliveryOrders = model.incDeliveryOrders.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.HouseCode));
                model.incPurchaseOrders = model.incPurchaseOrders.Where(m => userWarehouses.Select(m => m.HouseCode).Contains(m.MasDataTenantWarehouse.HouseCode));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? DONumber)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;
            var HouseCode = User.FindFirst("HouseCode")?.Value;

            var tenants = await _unitOfWork.Tenant.GetAllAsync();
            var warehouses = await _unitOfWork.HouseCode.GetAllAsync();
            var suppliers = await _unitOfWork.Supplier.GetAllAsync();

            var model = new DeliveryOrderCreateViewModel();
                
            model.incDeliveryOrder = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                includeProperties:
                     m => m.Include(m => m.IncDeliveryOrderProducts).ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts).ThenInclude(m => m.IncSerialNumbers),
                filter:
                    m => m.DONumber == DONumber &&
                    m.Status == SD.FlagDO_OPN);


            if (ProfileId == SD.Role_Tenant)
            {
                var usertenant = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                var userwarehouses = await _unitOfWork.UserWarehouse.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                tenants = tenants.Where(m => usertenant.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString())).ToList();
                warehouses = warehouses.Where(m => userwarehouses.Select(m => m.HouseCode).Contains(m.HouseCode)).ToList();
            }

            if (model.incDeliveryOrder != null)
            {
                suppliers = suppliers.Where(m => m.TenantId == model.incDeliveryOrder.TenantId).ToList();

                model.masProductDatas = await _unitOfWork.Product.GetAllAsync(
                    filter:
                        m => m.TenantId == model.incDeliveryOrder.TenantId &&
                        m.Flag == FlagEnum.Active);

                model.masProductBundlings = await _unitOfWork.ProductBundling.GetAllAsync(
                    filter:
                        m => m.TenantId == model.incDeliveryOrder.TenantId &&
                        m.Flag == FlagEnum.Active,
                    includeProperties:
                        m => m.Include(m => m.MasProductBundlingDatas));

                ViewData["TenantId"] = new SelectList(tenants.Where(m => m.TenantId == model.incDeliveryOrder.TenantId), "TenantId", "Name", model.incDeliveryOrder.TenantId);
                ViewData["HouseCode"] = new SelectList(warehouses.Where(m => m.HouseCode == model.incDeliveryOrder.HouseCode), "HouseCode", "HouseName", model.incDeliveryOrder.HouseCode);
                ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "Name", model.incDeliveryOrder.SupplierId);
                ViewData["DeliveryCourierId"] = new SelectList(await _unitOfWork.DeliveryOrderCourier.GetAllAsync(), "Id", "Name", model.incDeliveryOrder.DeliveryCourierId);

                return View(model);
            }

            suppliers = suppliers.Where(m => m.TenantId == tenants.Select(m => m.TenantId).FirstOrDefault() &&
                    m.Flag == FlagEnum.Active).ToList();

            model.masProductDatas = await _unitOfWork.Product.GetAllAsync(
                filter:
                    m => m.TenantId == tenants.Select(m => m.TenantId).FirstOrDefault() &&
                    m.Flag == FlagEnum.Active);

            model.masProductBundlings = await _unitOfWork.ProductBundling.GetAllAsync(
                filter:
                    m => m.TenantId == tenants.Select(m => m.TenantId).FirstOrDefault() &&
                    m.Flag == FlagEnum.Active,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas));

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name");
            ViewData["HouseCode"] = new SelectList(warehouses, "HouseCode", "HouseName");
            ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "Name");
            ViewData["DeliveryCourierId"] = new SelectList(await _unitOfWork.DeliveryOrderCourier.GetAllAsync(), "Id", "Name");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(string? DONumber, DeliveryOrderCreateViewModel model)
        {
            var result = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.DONumber == DONumber &&
                        m.Status == SD.FlagDO_OPN);

            if (result != null)
            {
                result.DOSupplier = model.incDeliveryOrder.DOSupplier;
                result.SupplierId = model.incDeliveryOrder.SupplierId;
                result.DateDelivered = model.incDeliveryOrder.DateDelivered;
                result.DeliveryCourierId = model.incDeliveryOrder.DeliveryCourierId;
                result.ShippingCost = model.incDeliveryOrder.ShippingCost;
                result.KTP = model.incDeliveryOrder.KTP;
                result.Name = model.incDeliveryOrder.Name;
                result.Note = model.incDeliveryOrder.Note;

                _unitOfWork.DeliveryOrder.Update(result);
                TempData["success"] = "Delivery Order berhasil diupdate!";
            }
            else
            {
                var Code = "DO";
                var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
                var Last = 1.ToString("D4");

                DONumber = Code + Tanggal + Last;
                if (await _unitOfWork.DeliveryOrder.AnyAsync(m => m.DONumber == DONumber))
                {
                    var checker = await _unitOfWork.DeliveryOrder.GetAllAsync(m => m.DONumber.Contains(Code + Tanggal));
                    int LastCount = int.Parse(checker.Max(m => m.DONumber.Substring(Code.Length + Tanggal.Length)));
                    DONumber = Code + Tanggal + (LastCount + 1).ToString("000#");
                }

                string DOSupplier = model.incDeliveryOrder.DOSupplier;
                if (DOSupplier == SD.AutoGenerated)
                {
                    DOSupplier = DONumber;
                }

                result = new IncDeliveryOrder();

                result.DONumber = DONumber;
                result.DOSupplier = DOSupplier;
                result.TenantId = model.incDeliveryOrder.TenantId;
                result.HouseCode = model.incDeliveryOrder.HouseCode;
                result.SupplierId = model.incDeliveryOrder.SupplierId;
                result.DateDelivered = model.incDeliveryOrder.DateDelivered;
                result.DeliveryCourierId = model.incDeliveryOrder.DeliveryCourierId;
                result.ShippingCost = model.incDeliveryOrder.ShippingCost;
                result.KTP = model.incDeliveryOrder.KTP;
                result.Name = model.incDeliveryOrder.Name;
                result.Note = model.incDeliveryOrder.Note;
                result.CreatedBy = User.FindFirst("UserName")?.Value;

                await _unitOfWork.DeliveryOrder.AddAsync(result);
                TempData["success"] = "Berhasil! Tambahkan produk anda!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("Upsert", new { DONumber = DONumber });
        }


        [HttpPost]
        public async Task<JsonResult> AddProductByProductId(bool Type, IncDeliveryOrderProduct model)
        {
            if (model == null)
            {
                return Json(BadRequest("Invalid Modelstate!"));
            }

            var result = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DONumber == model.DONumber &&
                    m.Status == SD.FlagDO_OPN,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts)
                    .ThenInclude(m => m.IncSerialNumbers));

            if (result == null)
            {
                return Json(BadRequest("Invalid Modelstate!"));
            }

            var resultproduct = result.IncDeliveryOrderProducts.SingleOrDefault(
                m => m.ProductId == model.ProductId);

            var product = await _unitOfWork.Product.GetSingleOrDefaultAsync(
                    filter:
                        m => m.ProductId == model.ProductId &&
                        m.TenantId == result.TenantId);

            if (product == null)
            {
                return Json(BadRequest("Produk gagal ditambahkan!"));
            }

            if (resultproduct != null)
            {
                if (model.Quantity < 1)
                {
                    if (resultproduct.IncSerialNumbers.Count > 0)
                    {
                        _unitOfWork.SerialNumber.RemoveRange(resultproduct.IncSerialNumbers);
                    }
                    _unitOfWork.DeliveryOrderProduct.Remove(resultproduct);
                }
                else
                {
                    if (Type)
                    {
                        resultproduct.Quantity += 1;
                    }
                    else
                    {
                        resultproduct.Quantity = model.Quantity;
                        resultproduct.UnitPrice = model.UnitPrice;
                        resultproduct.SubTotal = resultproduct.Quantity * resultproduct.UnitPrice;

                        if (product.StorageMethod == "FEFO")
                        {
                            resultproduct.DateOfExpired = model.DateOfExpired;
                        }
                    }
                    _unitOfWork.DeliveryOrderProduct.Update(resultproduct);
                }
            }
            else
            {
                resultproduct = new IncDeliveryOrderProduct();

                resultproduct.DONumber = result.DONumber;
                resultproduct.ProductId = product.ProductId;
                resultproduct.UnitPrice = product.PurchasePrice;
                resultproduct.SubTotal = resultproduct.UnitPrice;

                if (product.StorageMethod == "FEFO")
                {
                    resultproduct.DateOfExpired = DateTime.Now;
                }

                await _unitOfWork.DeliveryOrderProduct.AddAsync(resultproduct);
            }

            await _unitOfWork.SaveAsync();

            result = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts.OrderByDescending(m => m.DOProductId)).ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts.OrderByDescending(m => m.DOProductId)).ThenInclude(m => m.IncSerialNumbers),
                filter:
                    m => m.DONumber == model.DONumber &&
                    m.Status == SD.FlagDO_OPN);

            return Json(Ok(result));
        }

        [HttpPost]
        public async Task<JsonResult> AddProductByBundlingId(string DONumber, string BundlingId, Guid TenantId)
        {
            var model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DONumber == DONumber &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagDO_OPN,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts));

            var bundling = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                filter:
                    m => m.BundlingId == BundlingId &&
                    m.TenantId == TenantId &&
                    m.Flag == FlagEnum.Active,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas).ThenInclude(m => m.MasProductData));

            if (model == null || bundling == null)
            {
                return Json(BadRequest("Invalid Modelstate!"));
            }

            foreach (var item in bundling.MasProductBundlingDatas)
            {
                var modelproduct = model.IncDeliveryOrderProducts.SingleOrDefault(m => m.ProductId == item.ProductId);

                if (modelproduct != null)
                {
                    modelproduct.Quantity += item.Quantity;
                    modelproduct.SubTotal = modelproduct.Quantity * modelproduct.UnitPrice;

                    _unitOfWork.DeliveryOrderProduct.Update(modelproduct);
                }
                else
                {
                    modelproduct = new IncDeliveryOrderProduct();

                    modelproduct.DONumber = model.DONumber;
                    modelproduct.ProductId = item.ProductId;
                    modelproduct.Quantity = item.Quantity;
                    modelproduct.UnitPrice = item.MasProductData.PurchasePrice;
                    modelproduct.SubTotal = modelproduct.UnitPrice;

                    if (item.MasProductData.StorageMethod == "FEFO")
                    {
                        modelproduct.DateOfExpired = DateTime.Now;
                    }

                    await _unitOfWork.DeliveryOrderProduct.AddAsync(modelproduct);
                }
            }

            await _unitOfWork.SaveAsync();

            model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts.OrderByDescending(m => m.DOProductId)).ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts.OrderByDescending(m => m.DOProductId)).ThenInclude(m => m.IncSerialNumbers),
                filter:
                    m => m.DONumber == model.DONumber &&
                    m.Status == SD.FlagDO_OPN);

            return Json(Ok(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(DeliveryOrderCreateViewModel model)
        {
            if (model.incDeliveryOrder == null)
            {
                TempData["error"] = "Delivery Order Notfound!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DONumber == model.incDeliveryOrder.DONumber &&
                    m.TenantId == model.incDeliveryOrder.TenantId &&
                    m.Status == SD.FlagDO_OPN,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts)
                    .ThenInclude(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderProducts)
                    .ThenInclude(m => m.IncSerialNumbers));

            if (result == null)
            {
                TempData["error"] = "Delivery Order Notfound!";
                return RedirectToAction(nameof(Index));
            }

            if (result.IncDeliveryOrderProducts.Count < 1)
            {
                TempData["warning"] = "Please add some product!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber });
            }

            var SNChecker = result.IncDeliveryOrderProducts.Where(m => m.MasProductData.ProductLevel == SD.ProductLvl_SKU && m.MasProductData.SerialNumber == "SN");

            if (SNChecker.Any(m => m.IncSerialNumbers.Count() != m.Quantity))
            {
                TempData["warning"] = "Serial Number not equals Quantity!";
                return RedirectToAction("Upsert", new { DONumber = result.DONumber });
            }

            result.Status = SD.FlagDO_DO;
            result.DateCreated = DateTime.Now;
            _unitOfWork.DeliveryOrder.Update(result);

            foreach (var resultproduct in result.IncDeliveryOrderProducts)
            {
                resultproduct.Status = SD.FlagDOProduct_Booked;
                _unitOfWork.DeliveryOrderProduct.Update(resultproduct);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Delivery Order Successfully!";
            return RedirectToAction("Detail", "DeliveryOrderList", new { DONumber = result.DONumber , TenantId = result.TenantId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDeliveryOrder(string DONumber, Guid TenantId)
        {
            var model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.DONumber == DONumber &&
                    m.TenantId == TenantId &&
                    m.Status == SD.FlagDO_OPN,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts)
                    .ThenInclude(m => m.IncSerialNumbers));

            if (model == null)
            {
                TempData["error"] = "Invalid Modelstate!";
                return RedirectToAction("Index");
            }

            if (model.IncDeliveryOrderProducts.Count > 0)
            {
                foreach (var item in model.IncDeliveryOrderProducts)
                {
                    if (item.IncSerialNumbers.Count > 0)
                    {
                        _unitOfWork.SerialNumber.RemoveRange(item.IncSerialNumbers);
                    }
                    _unitOfWork.DeliveryOrderProduct.Remove(item);
                }
            }

            _unitOfWork.DeliveryOrder.Remove(model);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Product Deleted Successfullly!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetDeliveryOrderProductByDONumberByProductId(string DONumber, int ProductId)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductData),
                filter:
                    m => m.DONumber == DONumber &&
                    m.ProductId == ProductId);

            if (model == null)
            {
                return Json(BadRequest("Produk tidak ditemukan!"));
            }

            return Json(Ok(model));
        }

    }
}
