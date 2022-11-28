using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Models.Incoming;

namespace WMSDeal.Services
{

    public interface IDeliveryOrderService
    {
        Task<DefaultResponse> GetDeliveryOrders(string status,string Periode=null);
        Task<DefaultResponse> GetDeliveryOrders();
        Task<DefaultResponse> GetDeliveryOrderProducts(string DONumber);
        Task<DefaultResponse> GetProductItemsDeliveryOrder(string DOProductId);
        Task<DefaultResponse> GetProductsDeliveryOrder(string DONumber);
        Task<DefaultResponse> GetTotalArrival(int DOProductId, string ProductLevel);
        Task<DefaultResponse> SaveArrival(DeliveryOrderArrival model);
    }
    public class DeliveryOrderService : IDeliveryOrderService
    {
        public async Task<DefaultResponse> GetDeliveryOrderProducts(string DONumber)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/incoming/arrival/doproduct/" + DONumber);

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);

                    var _response = new DefaultResponse();

                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetDeliveryOrders(string status,string Periode=null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/incoming/arrival?status=" + status + "&PeriodeDelivered=" + Periode);

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);
                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetDeliveryOrders()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/incoming/arrival");

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);
                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetProductItemsDeliveryOrder(string DOProductId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/incoming/arrival/ProductItems/" + DOProductId);

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);

                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetProductsDeliveryOrder(string DONumber)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/incoming/arrival/products/" + DONumber);

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);

                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetTotalArrival(int DOProductId, string ProductLevel)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/incoming/arrival/totalarrived/" + DOProductId);

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);

                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.Unauthorized;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> SaveArrival(DeliveryOrderArrival model)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string jsonPost = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonPost, Encoding.UTF8, "Application/json");
                    string url = AppConstant.BaseUrl + "/maui/incoming/arrival";
                    client.BaseAddress = new Uri(url);
                    var responseMessage = await client.PostAsync(url, content);

                    // HttpResponseMessage responseMessage = await client.PostAsync(url,content);

                    // List<IncDeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<IncDeliveryOrder>>(deliveryOrders);

                    var _response = new DefaultResponse();

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = responseMessage.Content.ReadAsStringAsync().Result;
                        return _response;
                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var json = await responseMessage.Content.ReadAsStringAsync();

                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Error";
                        _response.Data = json;
                        return _response;
                    }
                    else
                    {
                        _response.Code = responseMessage.StatusCode;
                        _response.Message = "Error";
                        _response.Data = "";
                        return _response;
                    }

                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }
    }
}

