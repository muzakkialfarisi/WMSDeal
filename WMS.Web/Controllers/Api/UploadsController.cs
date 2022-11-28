using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WMS.Models.ViewModels;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("DeliveryOrderArrival")]
        public IActionResult UploadDeliveryOrderArrival([FromBody] DeliveryOrderArrivalViewModel model)
        {
            byte[] bytes = Convert.FromBase64String(model.ProductImage);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/DeliveryOrder/Arrival");
            var fileName = model.DOProductId.ToString() + ".jpg";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image pic = Image.FromStream(ms);
                pic.Save(filePath);
            }

            return Ok("Uploaded Successfully!");
        }

        [HttpPost("DeliveryOrderManifest")]
        public IActionResult UploadDo([FromBody] DeliveryOrderUploadViewModel model)
        {
            byte[] bytes = Convert.FromBase64String(model.NotaImage);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/DeliveryOrder");
            var fileName = model.DONumber + ".jpg";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image pic = Image.FromStream(ms);
                pic.Save(filePath);
            }

            return Ok("Uploaded Successfully!");
        }

        [HttpPost("SalesOrderDonePick")]
        public IActionResult Post([FromBody] SalesOrderAssignViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid Modelstate!");
            }

            if (model.ImageStaged == null || model.OrderId == null)
            {
                return BadRequest("Image is required!");
            }

            byte[] bytes = Convert.FromBase64String(model.ImageStaged);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/SalesOrder/Stages");
            var fileName = model.OrderId + ".jpg";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image pic = Image.FromStream(ms);
                pic.Save(filePath);
            }
            return Ok("Uploaded Successfully!");
        }
    }
}