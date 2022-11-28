using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class StoreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<JsonResult> GetStoreById(int StoreId)
        {
            var masStore = await _unitOfWork.Store.GetSingleOrDefaultAsync(
                filter:
                    m => m.StoreId == StoreId,
                includeProperties:
                    m => m.Include(m => m.MasKelurahan.MasKecamatan.MasKabupaten.MasProvinsi));
                
            return Json(masStore);
        }

        [HttpGet]
        public async Task<JsonResult> GetStoresByTenantIdAndPlatformId(Guid TenantId, int PlatformId)
        {
            var masStores = await _unitOfWork.Store.GetAllAsync(
                filter:
                    m => m.TenantId == TenantId &&
                    m.PlatformId == PlatformId &&
                    m.Flag == FlagEnum.Active);
                
            return Json(masStores);
        }
    }
}
