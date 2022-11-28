using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Models.ViewModels.ApiViewModel.Maui;

namespace WMS.Web.Controllers.Maui.Inventory
{
    [Route("maui/inventory/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class PutawayController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ErrorResponseViewModel _error = new ErrorResponseViewModel();

        public PutawayController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string DONumber = null, Guid? TenantId = null, string Status = null, string DateDelivered = null)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrder.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts)
                            .ThenInclude(m => m.IncDeliveryOrderArrivals)
                          .Include(m => m.MasDataTenant)
                                .ThenInclude(m => m.MasKelurahan)
                                    .ThenInclude(m => m.MasKecamatan)
                                        .ThenInclude(m => m.MasKabupaten)
                                            .ThenInclude(m => m.MasProvinsi),
                filter:
                    m => m.HouseCode == User.FindFirst("HouseCode").Value);

                var MOD = model.Select(x => new
                {
                    x.DONumber,
                    x.TenantId,
                    x.Status,
                    x.DateDelivered,
                    x.MasDataTenant.Name,
                    x.MasDataTenant.Address,
                    x.MasDataTenant.KodePos,
                    x.MasDataTenant.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi.ProName,
                    x.MasDataTenant.ProfileImageUrl,
                    x.IncDeliveryOrderProducts.Count

                }).ToList();

                if (DONumber != null)
                {
                    MOD = MOD.Where(m => m.DONumber.Contains(DONumber)).ToList();
                }

                if (TenantId != null)
                {
                    MOD = MOD.Where(m => m.TenantId == TenantId).ToList();
                }

                if (Status != null)
                {
                    MOD = MOD.Where(m => m.Status == Status).ToList();
                }

                if (DateDelivered != null)
                {
                    MOD = MOD.Where(m => m.DateDelivered?.ToString("yyyy-MM-dd") == DateDelivered).ToList();
                }

                return Ok(MOD);
            }
            catch (Exception e)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = e.Message;
                _error.Code = "IC0001";
                return BadRequest(_error);
            }
        }

        [HttpGet("Products/{DONumber}")]
        public async Task<IActionResult> GetProductsbyDONumber(string DONumber)
        {
            try
            {
                var model = await _unitOfWork.DeliveryOrderProduct.GetAllAsync(
                   includeProperties:
                       m => m.Include(m => m.MasProductData)
                             .Include(m => m.IncDeliveryOrderArrivals)
                                .ThenInclude(m => m.InvProductPutaways),
                   filter:
                       m => m.DONumber == DONumber);

                var MOD = model.Select(x => new
                {
                    x.DONumber,
                    x.DOProductId,
                    x.DOProductCode,
                    x.Quantity,
                    x.Status,
                    x.DateOfExpired,
                    QtyArrival = x.IncDeliveryOrderArrivals.InvProductPutaways == null ? 0 :  x.IncDeliveryOrderArrivals.InvProductPutaways.Count,
                    x.ClosedNote,
                    x.MasProductData.ProductName,
                    x.MasProductData.ProductLevel,
                    x.MasProductData.ProductCondition,
                    x.MasProductData.SKU,
                    x.MasProductData.Unit,
                    x.MasProductData.ActualWeight,
                    x.MasProductData.BeautyPicture,
                    x.MasProductData.SizeCode,
                    x.MasProductData.ZoneCode,
                    x.MasProductData.SerialNumber,
                });

                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Exception";
                _error.Message = msg.Message;
                _error.Code = "IC0003";
                return BadRequest(_error);
            }
        }

        [HttpGet("TotalPuted/{DOProductId}")]
        public async Task<IActionResult> GetTotalPuted(int DOProductId)
        {
            try
            {
                var model = await _unitOfWork.PutAway.GetAllAsync(
                    filter: m => m.DOProductId == DOProductId);
                var MOD = model.Select(x => new
                {
                    x.DOProductId,
                    x.Quantity,
                });
                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Excepion";
                _error.Message = msg.Message;
                _error.Code = "PA0003";
                return BadRequest(_error);
            }
        }

        [HttpGet("StorageCode/{StorageCode}")]
        public async Task<IActionResult> GetStorageCodesByStorageCode(string StorageCode)
        {
            try
            {
                var model = await _unitOfWork.StorageCode.GetAllAsync(
               filter:
                   m => m.StorageCode.ToString() == StorageCode,
               includeProperties:
                   m => m.Include(m => m.InvStorageSize)
                   .Include(m => m.InvStorageCategory)
                   .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode)
                   .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone));


                var MOD = model.Select(x => new
                {
                    x.StorageCode,
                    x.BinCode,
                    x.InvStorageBin.LevelCode,
                    x.InvStorageBin.InvStorageLevel.SectionCode,
                    x.InvStorageBin.InvStorageLevel.InvStorageColumn.RowCode,
                    x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.RowName,
                    x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.ZoneCode,
                    x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone.ZoneName,
                    x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode,
                    x.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode.HouseName,
                    x.InvStorageSize.SizeCode,
                    x.InvStorageSize.SizeName,
                    x.InvStorageSize.Flag,
                });
                return Ok(MOD);
            }
            catch (Exception msg)
            {
                _error.StatusCode = "400";
                _error.Error = "Error Excepion";
                _error.Message = msg.Message;
                _error.Code = "PA0003";
                return BadRequest(_error);
            }
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
                _error.StatusCode = "400";
                _error.Error = "Error";
                _error.Message = "DO Product tidak ditemukan";
                _error.Code = "PA0010";
                return BadRequest(_error);
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
                _error.StatusCode = "400";
                _error.Error = "Error";
                _error.Message = "Warehouse atau Zone tidak sesuai";
                _error.Code = "AR0010";
                return BadRequest(_error);
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

            return Ok("Product sukses di put!");
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
