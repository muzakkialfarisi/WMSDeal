using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class StoragesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoragesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Zones")]
        public async Task<IActionResult> GetZones()
        {
            var model = await _unitOfWork.StorageZone.GetAllAsync();
            return Ok(model);
        }

        [HttpGet("Zones/{ZoneCode}")]
        public async Task<IActionResult> GetZonesByZoneCode(string ZoneCode)
        {
            var model = await _unitOfWork.StorageZone.GetSingleOrDefaultAsync(
                filter:
                    m => m.ZoneCode == ZoneCode);
            return Ok(model);
        }

        [HttpGet("Sizes")]
        public async Task<IActionResult> GetSizes()
        {
            var model = await _unitOfWork.StorageSize.GetAllAsync();
            return Ok(model);
        }

        [HttpGet("Sizes/{SizeCode}")]
        public async Task<IActionResult> GetSizesBySizeCode(string SizeCode)
        {
            var model = await _unitOfWork.StorageSize.GetSingleOrDefaultAsync(
                filter:
                    m => m.SizeCode == SizeCode);
            return Ok(model);
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            var model = await _unitOfWork.StorageCategory.GetAllAsync();
            return Ok(model);
        }

        [HttpGet("Categories/{StorageCategoryId}")]
        public async Task<IActionResult> GetCategoriesByStorageCategoryId(int StorageCategoryId)
        {
            var model = await _unitOfWork.StorageCategory.GetSingleOrDefaultAsync(
                filter:
                    m => m.StorageCategoryId == StorageCategoryId);
            return Ok(model);
        }

        [HttpGet("StorageCodes")]
        public async Task<IActionResult> GetStorageCodes(string HouseCode)
        {
            var model = await _unitOfWork.StorageCode.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.InvStorageSize)
                    .Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode)
                    .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone));

            if (HouseCode != null)
            {
                model = model.Where(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.HouseCode == HouseCode).ToList();
            }

            return Ok(model);
        }

        [HttpGet("StorageCodes/{StorageCode}")]
        public async Task<IActionResult> GetStorageCodesByStorageCode(Guid StorageCode)
        {
            var model = await _unitOfWork.StorageCode.GetSingleOrDefaultAsync(
                filter:
                    m => m.StorageCode == StorageCode,
                includeProperties:
                    m => m.Include(m => m.InvStorageSize)
                    .Include(m => m.InvStorageCategory)
                    .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.MasHouseCode)
                    .Include(m => m.InvStorageBin.InvStorageLevel.InvStorageColumn.InvStorageRow.InvStorageZone));
            return Ok(model);
        }
    }
}