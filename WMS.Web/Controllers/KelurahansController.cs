#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using WMS.Models.ViewModels;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class KelurahansController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public KelurahansController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> GetProvinsis()
        {
            var model = await _unitOfWork.Provinsi.GetAllAsync();

            return Json(model);
        }

        public async Task<IActionResult> GetKabupatensByProId(string ProId)
        {
            var model = await _unitOfWork.Kabupaten.GetAllAsync(
                filter:
                    m => m.ProId == ProId);

            return Json(model);
        }

        public async Task<IActionResult> GetKecamatansByKabId(string KabId)
        {
            var model = await _unitOfWork.Kecamatan.GetAllAsync(
                filter:
                    m => m.KabId == KabId);

            return Json(model);
        }

        public async Task<IActionResult> GetKelurahansByKecId(string KecId)
        {
            var model = await _unitOfWork.Kelurahan.GetAllAsync(
                filter:
                    m => m.KecId == KecId);

            return Json(model);
        }
    }
}
