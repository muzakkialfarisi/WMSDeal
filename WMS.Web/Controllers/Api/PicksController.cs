using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;

namespace WMS.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class PicksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PicksController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> Get(Guid UserId, string PickedStatus)
        {
            var model = await _unitOfWork.SalesOrderAssign.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder.OutSalesOrderProducts)
                        .ThenInclude(m => m.OutSalesOrderStorages)
                        .Include(m => m.OutSalesOrder.OutSalesOrderProducts)
                        .ThenInclude(m => m.MasProductData),
                filter:
                    m => m.UserId == UserId &&
                    m.Flag == 1);

            var result = new List<OutSalesOrderStorage>();

            foreach (var order in model)
            {
                foreach (var product in order.OutSalesOrder.OutSalesOrderProducts)
                {
                    result.AddRange(product.OutSalesOrderStorages);
                }
            }

            if (PickedStatus == null)
            {
                result = result.Where(m => m.PickedStatus == PickedStatus).ToList();
            }

            result = result.OrderBy(m => m.Sequence).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(int Id, [FromBody] OutSalesOrderStorage model)
        {
            if (model == null)
            {
                return BadRequest("Invalid Modelstate!");
            }

            var result = await _unitOfWork.SalesOrderStorage.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.Id == Id,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderProduct.MasProductData.InvProductStocks)
                    .Include(m => m.OutSalesOrderProduct.OutSalesOrder)
                    .Include(m => m.IncItemProduct)
                    .Include(m => m.InvStorageCode));

            if (result == null)
            {
                return BadRequest("Product Notfound!");
            }

            if (result.StorageCode != model.StorageCode)
            {
                return BadRequest("Storage code does not match!");
            }

            if (result.PickedStatus != "Ordered")
            {
                return BadRequest("Product failed to pick up!");
            }

            result.DatePicked = DateTime.Now;
            result.PickedBy = User.FindFirst("UserName")?.Value;
            result.PickedStatus = "Picked";

            if (model.QualityCheckedStatus != null)
            {
                result.QualityCheckedStatus = model.QualityCheckedStatus;
                if (model.QualityCheckedStatus == "Rejected")
                {
                    result.QualityCheckedRemark = model.QualityCheckedRemark;
                }
                result.DateQualityChecked = DateTime.Now;
                result.QualityCheckedBy = User.FindFirst("UserName")?.Value;
            }

            _unitOfWork.SalesOrderStorage.Update(result);

            //if (result.InvStorageCode.QtyOrder < 1)
            //{
            //    return BadRequest("Minus storage quanitity!");
            //}

            result.InvStorageCode.QtyOrder = result.InvStorageCode.QtyOrder - result.QtyPick;
            if (result.InvStorageCode.Qty < 1)
            {
                result.InvStorageCode.Flag = 1;
            }
            _unitOfWork.StorageCode.Update(result.InvStorageCode);

            if (result.OutSalesOrderProduct.MasProductData.ProductLevel == "IKU")
            {
                result.IncItemProduct.Status = 6;
                _unitOfWork.ItemProduct.Update(result.IncItemProduct);
            }

            var productStock = result.OutSalesOrderProduct.MasProductData.InvProductStocks
                .SingleOrDefault(m => m.HouseCode == result.OutSalesOrderProduct.OutSalesOrder.HouseCode);

            productStock.QtyOrder = productStock.QtyOrder - result.QtyPick;
            if (productStock.QtyOrder < 0)
            {
                return BadRequest("Minus product stock quanitity!");
            }
            _unitOfWork.ProductStock.Update(productStock);

            var history = new InvProductHistory
            {
                ProductId = result.OutSalesOrderProduct.ProductId,
                HouseCode = result.OutSalesOrderProduct.OutSalesOrder.HouseCode,
                HistoryType = Models.ProductHistoryType.Out,
                TrxNo = result.OutSalesOrderProduct.OrderId,
                Interest = "SO",
                Quantity = result.QtyPick,
                Note = string.Empty,
                Stock = productStock.Stock + productStock.QtyOrder,
                DatedTime = DateTime.Now,
                UserBy = User.FindFirst("UserName")?.Value,
                Flag = 1
            };

            await _unitOfWork.ProductHistory.AddAsync(history);

            var soProduct = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                disableTracking:
                   false,
                filter:
                    m => m.OrdProductId == result.OrdProductId,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderStorages));

            if (!soProduct.OutSalesOrderStorages.Any(m => m.PickedStatus == "Ordered"))
            {
                soProduct.Flag = 3;
                _unitOfWork.SalesOrderProduct.Update(soProduct);

                var order = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                    disableTracking:
                        false,
                    filter:
                        m => m.OrderId == soProduct.OrderId,
                    includeProperties:
                        m => m.Include(m => m.OutSalesOrderProducts));

                if (!order.OutSalesOrderProducts.Any(m => m.Flag == 2))
                {
                    order.Status = 3;
                    _unitOfWork.SalesOrder.Update(order);

                    await _unitOfWork.SaveAsync();

                    return Ok("All product picked up successfully!");
                }
            }

            await _unitOfWork.SaveAsync();

            return Ok("Product picked up successfully!");
        }

        [HttpPost("UploadStage")]
        public async Task<IActionResult> Post([FromBody] SalesOrderAssignViewModel model)
        {
            var result = await _unitOfWork.SalesOrderAssign.GetAllAsync(
                disableTracking:
                    false,
                filter:
                    m => m.UserId == model.UserId &&
                    m.PickAssignId == model.PickAssignId,
                includeProperties:
                    m => m.Include(m => m.OutSalesOrder));

            if (result == null)
            {
                return BadRequest("Invalid Modelstate!");
            }

            if (!result.Any(m => m.Flag == 1)) // assign kosong
            {
                return BadRequest("No data picked!");
            }

            if (result.Any(m => m.OutSalesOrder.Status == 2)) // so blm selesai pick
            {
                return BadRequest("Pick not finished!");
            }

            result = result.Where(m => m.Flag == 1).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (model.ImageStaged == null)
                {
                    return BadRequest("Image cannot be null!");
                }
                //byte[] bytes = Convert.FromBase64String(model.ImageStaged);

                //string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/SalesOrder/Stages");
                //var fileName = result[i].OrderId + ".jpg";
                //string filePath = Path.Combine(uploadsFolder, fileName);

                //using (MemoryStream ms = new MemoryStream(bytes))
                //{
                //    Image pic = Image.FromStream(ms);
                //    pic.Save(filePath);
                //}

                result[i].ImageStaged = result[i].OrderId + ".jpg";
                result[i].Flag = 2;
                result[i].DateStaged = DateTime.Now;
                _unitOfWork.SalesOrderAssign.Update(result[i]);

                result[i].OutSalesOrder.Status = 4;
                _unitOfWork.SalesOrder.Update(result[i].OutSalesOrder);
            }

            await _unitOfWork.SaveAsync();
            return Ok("Uploaded Successfully!");
        }
    }
}