using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class PutAwaysController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PutAwaysController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("IKU")]
        public async Task<IActionResult> PutAwayIKU(int DOProductId, [FromBody] PutAwayViewModel model)
        {
            var incDeliveryOrderProduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.IncItemProducts)
                        .ThenInclude(m => m.InvStorageCode));

            if (incDeliveryOrderProduct == null)
            {
                return BadRequest("DO Product Notfound!");
            }

            var incItemProduct = incDeliveryOrderProduct.IncItemProducts
                .Where(m => m.IKU.ToLower().Trim() == model.IKU.ToLower().Trim())
                .SingleOrDefault();

            if (incItemProduct == null)
            {
                return BadRequest("IKU Notfound!");
            }

            if (incItemProduct.StorageCode.ToString().ToLower().Trim() != model.StorageCode.ToString().ToLower().Trim())
            {
                return BadRequest("Storage Code Does not Match!");
            }

            incItemProduct.DatePutedAway = DateTime.Now;
            incItemProduct.PutedAwatBy = User.FindFirst("UserName")?.Value;
            incItemProduct.Status = 4;
            _unitOfWork.ItemProduct.Update(incItemProduct);

            incItemProduct.InvStorageCode.Flag = 3;
            incItemProduct.InvStorageCode.Qty = incItemProduct.InvStorageCode.Qty + 1;
            _unitOfWork.StorageCode.Update(incItemProduct.InvStorageCode);

            if (!incDeliveryOrderProduct.IncItemProducts.Any(m => m.Status == 3))
            {
                incDeliveryOrderProduct.Status = "Puted";
                _unitOfWork.DeliveryOrderProduct.Update(incDeliveryOrderProduct);

                await DOStatusChecker(incDeliveryOrderProduct.DONumber);
            }

            await _unitOfWork.SaveAsync();

            return Ok("Product Puted Successfully!");
        }

        [HttpPost("SKU")]
        public async Task<IActionResult> PutAwaySKU(int DOProductId, [FromBody] PutAwayViewModel model)
        {
            var incDeliveryOrderProduct = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == DOProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductData)
                    .Include(m => m.IncDeliveryOrderArrivals));


            if (incDeliveryOrderProduct == null)
            {
                return BadRequest("DO Product Notfound!");
            }

            var HouseCode = User.FindFirst("HouseCode")?.Value.ToString();
            var invStorageCode = await _unitOfWork.StorageCode.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.StorageCode.ToString().ToLower() == model.StorageCode.ToString().ToLower() &&
                    m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode.ToLower() == HouseCode.ToLower() &&
                    m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.ZoneCode == incDeliveryOrderProduct.MasProductData.ZoneCode &&
                    m.Flag >= 1);

            if (invStorageCode == null)
            {
                return BadRequest("Warehouse Or Zone Does not Match!");
            }

            InvProductPutaway invProductPutaway = new InvProductPutaway
            {
                DOProductId = DOProductId,
                StorageCode = model.StorageCode,
                Quantity = model.Quantity,
                QtyStock = model.Quantity,
                DatePutaway = DateTime.Now,
                PutBy = User.FindFirst("UserName")?.Value.ToString()
            };
            await _unitOfWork.PutAway.AddAsync(invProductPutaway);

            invStorageCode.Flag = 3;
            invStorageCode.Qty = invStorageCode.Qty + model.Quantity;
            _unitOfWork.StorageCode.Update(invStorageCode);

            await _unitOfWork.SaveAsync();

            var putedQuantity = await _unitOfWork.PutAway.SumAsync(filter: m => m.DOProductId == DOProductId, selector: m => m.Quantity);
            if (putedQuantity == incDeliveryOrderProduct.IncDeliveryOrderArrivals.Quantity)
            {
                incDeliveryOrderProduct.Status = "Puted";
                _unitOfWork.DeliveryOrderProduct.Update(incDeliveryOrderProduct);

                await DOStatusChecker(incDeliveryOrderProduct.DONumber);
            }

            await _unitOfWork.SaveAsync();

            return Ok("Product Puted Successfully!");
        }

        private async Task DOStatusChecker(string DONumber)
        {
            var incDeliveryOrder = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DONumber == DONumber,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts));

            if (!incDeliveryOrder.IncDeliveryOrderProducts.Any(m => m.Status == "Arrived"))
            {
                incDeliveryOrder.Status = "PUT";
                _unitOfWork.DeliveryOrder.Update(incDeliveryOrder);
            }
        }
    }
}
