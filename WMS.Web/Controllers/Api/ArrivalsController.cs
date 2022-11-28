using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.Utility;
using ProductHistoryType = WMS.Models.ProductHistoryType;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class ArrivalsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArrivalsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string DONumber)
        {
            var model = await _unitOfWork.DeliveryOrderArrival.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProduct));

            if (DONumber != null)
            {
                model = model.Where(m => m.IncDeliveryOrderProduct.DONumber == DONumber).ToList();
            }
            return Ok(model);
        }

        [HttpGet("{DOProductId}")]
        public async Task<IActionResult> GetByDOProductId(int DOProductId)
        {
            var model = await _unitOfWork.DeliveryOrderArrival.GetSingleOrDefaultAsync(
                filter:
                    m => m.DOProductId == DOProductId);
            return Ok(model);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeliveryOrderArrivalViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Inalid Modelstate!");
            }

            if (model.Quantity <= 0 || model.Quantity == null)
            {
                return BadRequest("Quantity harus lebih besar dari 0!");
            }

            var result = await _unitOfWork.DeliveryOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.DOProductId == model.DOProductId &&
                    m.Status == SD.FlagDOProduct_Booked,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrder)
                    .Include(m => m.MasProductData.InvProductStocks)
                    .Include(m => m.IncDeliveryOrderArrivals)
                    .Include(m => m.IncItemProducts)
                    .Include(m => m.IncSerialNumbers));

            if (result == null)
            {
                return BadRequest("Deivery Order Not Found!");
            }

            var update = true;

            if (result.IncDeliveryOrderArrivals == null)
            {
                if (model.Quantity > result.Quantity)
                {
                    return BadRequest("Quantity Melebihi Batas!");
                }

                result.IncDeliveryOrderArrivals = new IncDeliveryOrderArrival();
                result.IncDeliveryOrderArrivals.DOProductId = model.DOProductId;

                update = false;
            }

            result.IncDeliveryOrderArrivals.ProductImage = model.DOProductId.ToString() + ".jpg";
            result.IncDeliveryOrderArrivals.Quantity = result.IncDeliveryOrderArrivals.Quantity + model.Quantity;
            result.IncDeliveryOrderArrivals.ArrivedBy = result.IncDeliveryOrderArrivals.ArrivedBy + User.FindFirst("UserName")?.Value.ToString() + "; ";
            result.IncDeliveryOrderArrivals.Note = result.IncDeliveryOrderArrivals.Note + model.Note + "; ";

            if (result.IncDeliveryOrderArrivals.Quantity + result.IncDeliveryOrderArrivals.QtyNotArrived > result.Quantity)
            {
                return BadRequest("Quantity Melebihi Batas!");
            }

            if (update == true)
            {
                _unitOfWork.DeliveryOrderArrival.Update(result.IncDeliveryOrderArrivals);
            }
            else
            {
                await _unitOfWork.DeliveryOrderArrival.AddAsync(result.IncDeliveryOrderArrivals);
            }

            var arrivalproduct = new IncDeliveryOrderArrivalProduct();

            if (model.ProductImage != null)
            {
                byte[] bytes = Convert.FromBase64String(model.ProductImage);

                arrivalproduct.ImageUrl = Path.Combine("img/DeliveryOrder/Arrival", arrivalproduct.Id.ToString() + ".jpg"); ;
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, arrivalproduct.ImageUrl);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image pic = Image.FromStream(ms);
                    pic.Save(filePath);
                }
            }

            arrivalproduct.DOProductId = model.DOProductId;
            arrivalproduct.ProductId = result.ProductId;
            arrivalproduct.Quantity = model.Quantity;
            arrivalproduct.CreatedBy = User.FindFirst("UserName")?.Value;
            arrivalproduct.Note = model.Note;

            await _unitOfWork.DeliveryOrderArrivalProduct.AddAsync(arrivalproduct);

            var productStock = result.MasProductData.InvProductStocks.SingleOrDefault(m => m.HouseCode == result.IncDeliveryOrder.HouseCode);

            if (productStock == null)
            {
                productStock = new InvProductStock();
                productStock.Id = result.ProductId + result.IncDeliveryOrder.HouseCode;
                productStock.ProductId = (int)result.ProductId;
                productStock.HouseCode = result.IncDeliveryOrder.HouseCode;
                productStock.Stock = model.Quantity;
                await _unitOfWork.ProductStock.AddAsync(productStock);
            }
            else
            {
                productStock.Stock = productStock.Stock + model.Quantity;
                _unitOfWork.ProductStock.Update(productStock);
            }

            if (result.IncSerialNumbers.Count > 0)
            {
                var serials = result.IncSerialNumbers.Where(m => m.Status == SD.FlagSerialNumber_Open).ToList();
                if (serials.Count() < model.Quantity)
                {
                    return BadRequest("Quantity Melebihi Batas!");
                }

                for (int i = 0; i < model.Quantity; i++)
                {
                    serials[i].Status = SD.FlagSerialNumber_IN;
                    _unitOfWork.SerialNumber.Update(serials[i]);
                }
            }

            InvProductHistory productHistory = new InvProductHistory
            {
                ProductId = (int)result.ProductId,
                HouseCode = result.IncDeliveryOrder.HouseCode,
                HistoryType = ProductHistoryType.In,
                TrxNo = result.DONumber,
                Interest = "Delivery Order",
                Quantity = model.Quantity,
                Note = model.Note + "; ",
                Stock = productStock.Stock + productStock.QtyOrder,
                DatedTime = DateTime.Now,
                UserBy = User.FindFirst("UserName")?.Value
            };

            await _unitOfWork.ProductHistory.AddAsync(productHistory);

            update = false;

            if (result.Quantity == result.IncDeliveryOrderArrivals.Quantity + result.IncDeliveryOrderArrivals.QtyNotArrived)
            {
                result.Status = SD.FlagDOProduct_Arrived;
                _unitOfWork.DeliveryOrderProduct.Update(result);
                update = true;
            }

            await _unitOfWork.SaveAsync();

            if (update == true)
            {
                var resultdo = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.DONumber == result.DONumber &&
                        m.TenantId == result.IncDeliveryOrder.TenantId,
                    includeProperties:
                        m => m.Include(m => m.IncDeliveryOrderProducts));

                if (!resultdo.IncDeliveryOrderProducts.Any(m => m.Status == SD.FlagDOProduct_Booked))
                {
                    resultdo.Status = SD.FlagDO_AR;
                    resultdo.DateArrived = DateTime.Now;
                    _unitOfWork.DeliveryOrder.Update(resultdo);
                    await _unitOfWork.SaveAsync();

                    return Ok("Delivery Order arrived successfully!");
                }
            }

            return Ok("Updated Successfully!");
        }


        [HttpPost("UploadManifest")]
        public async Task<IActionResult> UploadManifestDeliveryOrder([FromBody] DeliveryOrderUploadViewModel model)
        {
            var result = await _unitOfWork.DeliveryOrder.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                includeProperties:
                    m => m.Include(m => m.IncDeliveryOrderProducts).ThenInclude(m => m.IncDeliveryOrderArrivals),
                filter:
                    m => m.DONumber == model.DONumber);

            if (result == null)
            {
                return BadRequest("Data Notfound!");
            }

            if (model.NotaImage == null)
            {
                return BadRequest("Product image is required!");
            }

            byte[] bytes = Convert.FromBase64String(model.NotaImage);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/DeliveryOrder");
            var fileName = model.DONumber + ".jpg";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image pic = Image.FromStream(ms);
                pic.Save(filePath);
            }

            for (int i = 0; i < result.IncDeliveryOrderProducts.Count; i++)
            {
                result.IncDeliveryOrderProducts[i].IncDeliveryOrderArrivals.NotaImage = filePath;
                _unitOfWork.DeliveryOrderArrival.Update(result.IncDeliveryOrderProducts[i].IncDeliveryOrderArrivals);
            }

            result.DateArrived = DateTime.Now;
            _unitOfWork.DeliveryOrder.Update(result);

            await _unitOfWork.SaveAsync();

            return Ok("Manifest Uploaded Successfully!");
        }
    }
}