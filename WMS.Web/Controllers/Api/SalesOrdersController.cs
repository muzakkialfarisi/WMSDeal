using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class SalesOrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesOrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string HouseCode, string DateOrdered, int? FlagPick, Guid? TenantId, string Status)
        {
            var model = await _unitOfWork.SalesOrder.GetAllAsync(
                includeProperties:
                    m => m.Include(m => m.OutSalesOrderConsignee)
                    .Include(m => m.MasDataTenant));

            if (HouseCode != null)
            {
                model = model.Where(m => m.HouseCode == HouseCode).ToList();
            }
            if (DateOrdered != null)
            {
                model = model.Where(m => m.DateOrdered.ToString("yyyy-MM-dd") == DateOrdered).ToList();
            }
            if (FlagPick != null)
            {
                model = model.Where(m => m.FlagPick == FlagPick).ToList();
            }
            if (TenantId != null)
            {
                model = model.Where(m => m.TenantId == TenantId).ToList();
            }
            if (Status != null)
            {
                char[] delimiterChars = { ',' };
                string[] words = Status.Split(delimiterChars);

                var tempStatus = new List<OutSalesOrder>();
                for (int i = 0; i < words.Length; i++)
                {
                    tempStatus.AddRange(model.Where(m => m.Status == short.Parse(words[i])));
                }

                model = tempStatus;
            }

            return Ok(model);
        }

        [HttpGet("{OrderId}")]
        public async Task<IActionResult> GetByOrderId(string OrderId)
        {
            var model = await _unitOfWork.SalesOrder.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrderId == OrderId,
                includeProperties:
                    m => m.Include(m => m.MasDataTenant));

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post(OutSalesOrder model)
        {
            model.OrderId = await OrderIdBuilder();
            model.FlagPick = 0;
            model.Status = 1;
            model.FlagApi = 2;

            var stock = await _unitOfWork.ProductStock.GetAllAsync(
                disableTracking:
                    false,
                filter:
                    m => model.OutSalesOrderProducts.Select(c => c.ProductId).Contains(m.ProductId) &&
                    m.HouseCode == model.HouseCode,
                orderBy:
                    m => m.OrderBy(m => m.ProductId));

            for (int i = 0; i < model.OutSalesOrderProducts.Count; i++)
            {
                var productInStorage = stock.Where(m => m.ProductId == model.OutSalesOrderProducts[i].ProductId).SingleOrDefault();
                if (productInStorage == null)
                {
                    return BadRequest("Product " + model.OutSalesOrderProducts[i].ProductId + " Notfound!");
                }

                if (productInStorage.Stock < model.OutSalesOrderProducts[i].Quantity)
                {
                    return BadRequest("Product " + model.OutSalesOrderProducts[i].ProductId + " Over Quantity!");
                }

                productInStorage.Stock = productInStorage.Stock - model.OutSalesOrderProducts[i].Quantity;
                productInStorage.QtyOrder = productInStorage.QtyOrder + model.OutSalesOrderProducts[i].Quantity;
                _unitOfWork.ProductStock.Update(productInStorage);

                model.OutSalesOrderProducts[i].Flag = 2;
            }

            //model.del.GrandWeight = await context.OutSalesOrdersProducts.Where(m => m.OrderId == order.OrderId).SumAsync(m => m.TotalWeight);
            //context.OutsalesOrderDeliverys.Update(deliv);

            return Ok(model);
        }

        private async Task<string> OrderIdBuilder()
        {
            var Code = "SO";
            var Tanggal = DateTime.Now.ToString("yyMMddHHmm");
            var Last = 1.ToString("D4");

            string SONumber = Code + Tanggal + Last;
            if (await _unitOfWork.SalesOrder.AnyAsync(m => m.OrderId == SONumber))
            {
                var temp = await _unitOfWork.SalesOrder.GetAllAsync(filter: m => m.OrderId.Contains(Code + Tanggal));
                int LastCount = int.Parse(temp.Max(m => m.OrderId.Substring(Code.Length + Tanggal.Length)));
                SONumber = Code + Tanggal + (LastCount + 1).ToString("000#");
            }
            return SONumber;
        }

        [HttpGet("Products/{OrdProductId}")]
        public async Task<IActionResult> GetSOProductByOrdProductId(int OrdProductId)
        {
            var model = await _unitOfWork.SalesOrderProduct.GetSingleOrDefaultAsync(
                filter:
                    m => m.OrdProductId == OrdProductId,
                includeProperties:
                    m => m.Include(m => m.MasProductData));

            return Ok(model);
        }

        [HttpGet("{OrderId}/Products")]
        public async Task<IActionResult> GetSOProductBySONumber(string OrderId, string ProductLevel, string ProductName)
        {
            var model = await _unitOfWork.SalesOrderProduct.GetAllAsync(
                filter:
                    m => m.OrderId == OrderId,
                includeProperties:
                    m => m.Include(m => m.MasProductData));

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
