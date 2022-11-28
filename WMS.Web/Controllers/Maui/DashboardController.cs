using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models.ViewModels.ApiViewModel.Maui;

namespace WMS.Web.Controllers.Maui
{
    [Route("maui/[controller]")]
    [ApiController]
    [Authorize(Policy = "Bearer")]
    public class DashboardController : ControllerBase
    {
        IUnitOfWork unitOfWork;
        ErrorResponseViewModel errorResponse = new ErrorResponseViewModel();

        public DashboardController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("Home")]
        public async Task<IActionResult> GetHomeDashboard()
        {
            try
            {
                var deliveryOrders = await unitOfWork.DeliveryOrder.GetAllAsync(
                   filter:
                    m => m.HouseCode == User.FindFirst("HouseCode").Value &&
                    m.DateDelivered > DateTime.Now.AddDays(-31) && m.Status != "CANCEL");

                var salesOrders = await unitOfWork.SalesOrder.GetAllAsync(
                    filter:
                        m => m.HouseCode == User.FindFirst("HouseCode").Value &&
                        m.DateOrdered > DateTime.Now.AddDays(-31) && m.Status > 1);


                List<HomeDashboard> models = new List<HomeDashboard>();
                models.Add(new HomeDashboard
                {
                    Title = "Incoming",
                    Total = deliveryOrders.Count(),
                    Done = deliveryOrders.Where(m => m.Status != "DO").Count(),
                    Outstanding = deliveryOrders.Where(m => m.Status == "DO").Count(),
                });
                models.Add(new HomeDashboard
                {
                    Title = "Outgoing",
                    Total = salesOrders.Count(),
                    Done = salesOrders.Where(m => m.Status > 2).Count(),
                    Outstanding = salesOrders.Where(m => m.Status == 2).Count(),
                });
                return Ok(models);
            }
            catch (Exception ex)
            {
                errorResponse.StatusCode = "400";
                errorResponse.Error = "Error Exception";
                errorResponse.Message = ex.Message;
                errorResponse.Code = "IC0001";
                return BadRequest(errorResponse);
            }
        }

    }
}
