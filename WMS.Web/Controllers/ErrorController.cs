using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ErrorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Expired()
        {
            await _unitOfWork.UserManager.SignOut(HttpContext);
            return View();
        }

        public async Task<IActionResult> Unauthorized()
        {
            await _unitOfWork.UserManager.SignOut(HttpContext);
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult UnderConstruction()
        {
            return View();
        }

        public IActionResult NotAllowed()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult InternalServer()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}