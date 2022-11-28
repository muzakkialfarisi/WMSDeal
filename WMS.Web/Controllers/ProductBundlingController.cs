using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Utility;

namespace WMS.Web.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "Tenant")]
    public class ProductBundlingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBundlingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            var model = await _unitOfWork.ProductBundling.GetAllAsync(
                    includeProperties:
                        m => m.Include(m => m.MasDataTenant)
                        .Include(m => m.MasProductBundlingDatas));

            if (ProfileId == SD.Role_Tenant)
            {
                var UserTenants = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                model = model.Where(m => UserTenants.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString())).ToList();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? BundlingId, Guid? TenantId)
        {
            var ProfileId = User.FindFirst("ProfileId")?.Value;
            var UserId = User.FindFirst("UserId")?.Value;

            var tenants = await _unitOfWork.Tenant.GetAllAsync(
                filter:
                    m => m.Flag == FlagEnum.Active);

            var model = new ProductBundlingCreateViewModel();
            
            model.masProductBundling = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                    filter:
                        m => m.BundlingId == BundlingId &&
                        m.TenantId == TenantId,
                    includeProperties:
                        m => m.Include(m => m.MasDataTenant)
                        .Include(m => m.MasProductBundlingDatas).ThenInclude(m => m.MasProductData));
            
            if (ProfileId == SD.Role_Tenant)
            {
                var UserTenants = await _unitOfWork.UserTenant.GetAllAsync(
                    filter:
                        m => m.UserId.ToString() == UserId);

                tenants = tenants.Where(m => UserTenants.Select(m => m.TenantId.ToString()).Contains(m.TenantId.ToString())).ToList();
            }

            if (model.masProductBundling != null)
            {
                model.masProductDatas = await _unitOfWork.Product.GetAllAsync(
                    filter:
                        m => m.TenantId == model.masProductBundling.TenantId &&
                        m.Flag == FlagEnum.Active);

                tenants = tenants.Where(m => m.TenantId == model.masProductBundling.TenantId).ToList();
            }

            ViewData["TenantId"] = new SelectList(tenants, "TenantId", "Name", model.masProductBundling?.TenantId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductBundlingCreateViewModel model)
        {
            if (model.masProductBundling == null)
            {
                TempData["error"] = "invalid Modelstate!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.BundlingId == model.masProductBundling.BundlingId &&
                    m.TenantId == model.masProductBundling.TenantId);

            var counter = await _unitOfWork.ProductBundling.GetAllAsync(
                filter:
                    m => m.Name == model.masProductBundling.Name &&
                    m.TenantId == model.masProductBundling.TenantId &&
                    m.BundlingId != model.masProductBundling.BundlingId);

            if (counter.Count > 0)
            {
                TempData["error"] = "Bunling name already used!";
                return RedirectToAction("Upsert", new { BundlingId = result?.BundlingId, TenantId = result?.TenantId });
            }

            if (result != null)
            {
                result.Name = model.masProductBundling.Name;
                result.Flag = model.masProductBundling.Flag;
                result.UpdatedBy = User.FindFirst("UserName")?.Value;
                result.DateUdated = DateTime.Now.ToString();

                _unitOfWork.ProductBundling.Update(result);
            }
            else
            {
                result = new MasProductBundling();

                result.TenantId = model.masProductBundling.TenantId;
                result.Name = model.masProductBundling.Name;
                result.Flag = model.masProductBundling.Flag;
                result.CreatedBy = User.FindFirst("UserName")?.Value;

                await _unitOfWork.ProductBundling.AddAsync(result);
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction("Upsert", new { BundlingId = result.BundlingId, TenantId = result.TenantId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBundling(string BundlingId, Guid TenantId)
        {
            var model = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                filter:
                    m => m.BundlingId == BundlingId &&
                    m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas));

            if (model == null)
            {
                TempData["error"] = "invalid Modelstate!";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.ProductBundlingData.RemoveRange(model.MasProductBundlingDatas);
            _unitOfWork.ProductBundling.Remove(model);

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Bundling deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> UpsertData(bool Type, MasProductBundlingData model)
        {
            if (model == null)
            {
                return Json(BadRequest("Invalid Modelstate!"));
            }

            var result = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.BundlingId == model.BundlingId,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas)
                    .ThenInclude(m => m.MasProductData));

            if (result == null)
            {
                return Json(BadRequest("Invalid Modelstate!"));
            }

            var resultproduct = result.MasProductBundlingDatas.SingleOrDefault(
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
                    _unitOfWork.ProductBundlingData.Remove(resultproduct);
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
                    }
                    _unitOfWork.ProductBundlingData.Update(resultproduct);
                }
            }
            else
            {
                resultproduct = new MasProductBundlingData();

                resultproduct.BundlingId = result.BundlingId;
                resultproduct.ProductId = product.ProductId;

                await _unitOfWork.ProductBundlingData.AddAsync(resultproduct);
            }

            await _unitOfWork.SaveAsync();

            result = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas).ThenInclude(m => m.MasProductData),
                filter:
                    m => m.BundlingId == model.BundlingId);

            return Json(Ok(result));
        }

        [HttpGet]
        public async Task<JsonResult> GetProductBundlingByTenantId(Guid TenantId)
        {
            var model = await _unitOfWork.ProductBundling.GetAllAsync(
                filter:
                    m => m.TenantId == TenantId,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas));

            return Json(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductBundlingByBundlingId(string BundlingId)
        {
            var model = await _unitOfWork.ProductBundling.GetSingleOrDefaultAsync(
                filter:
                    m => m.BundlingId == BundlingId,
                includeProperties:
                    m => m.Include(m => m.MasProductBundlingDatas).ThenInclude(m => m.MasProductData));

            return Json(model);
        }
    }
}
