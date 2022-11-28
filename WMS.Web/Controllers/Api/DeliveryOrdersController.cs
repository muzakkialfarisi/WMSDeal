using Core.DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WMS.DataAccess;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class DeliveryOrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryOrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string DONumber, string DOSupplier, string HouseCode, Guid? TenantId, string Status, string DateDelivered)
        {
            var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.MasDataTenant)
                    .Include(m => m.MasHouseCode));


            if (DONumber != null)
            {
                model = model.Where(m => m.DONumber.Contains(DONumber)).ToList();
            }
            if (DOSupplier != null)
            {
                model = model.Where(m => m.DONumber.Contains(DOSupplier)).ToList();
            }
            if (HouseCode != null)
            {
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }
            if (TenantId != null)
            {
                model = model.Where(m => m.TenantId == TenantId).ToList();
            }
            if (Status != null)
            {
                char[] delimiterChars = { ',' };
                string[] words = Status.Split(delimiterChars);

                var tempStatus = new List<IncDeliveryOrder>();
                for (int i = 0; i < words.Length; i++)
                {
                    tempStatus.AddRange(model.Where(m => m.Status == words[i]));
                }

                model = tempStatus;
            }
            if (DateDelivered != null)
            {
                model = model.Where(m => m.DateDelivered?.ToString("yyyy-MM-dd") == DateDelivered).ToList();
            }

            return Ok(model);
        }

        [HttpGet("{DONumber}")]
        public async Task<IActionResult> GetByDONumber(string DONumber)
        {
            var model = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(filter: m => m.DONumber == DONumber);
            return Ok(model);
        }

        [HttpGet("Products/{DOProductId}")]
        public async Task<IActionResult> GetDOProductByDOProductId(int DOProductId)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductData));
            return Ok(model);
        }

        [HttpGet("{DONumber}/Products")]
        public async Task<IActionResult> GetDOProductByDONumber(string DONumber, string ProductLevel, string ProductName)
        {
            var model = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                filter:
                    m => m.DONumber == DONumber,
                includeProperties:
                    m => m.Include(m => m.MasProductData.InvStorageSize)
                    .Include(m => m.IncItemProducts)
                    .Include(m => m.IncDeliveryOrderArrivals.InvProductPutaways));

            if (ProductLevel != null)
            {
                model = model.Where(m => m.MasProductData.ProductLevel == ProductLevel).ToList();
            }
            if (ProductName != null)
            {
                model = model.Where(m => m.MasProductData.ProductName.ToLower().Contains(ProductName.ToLower())).ToList();
            }

            return Ok(model);
        }
    }
}