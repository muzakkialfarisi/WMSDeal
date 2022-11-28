using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Models.Inventory;

namespace WMSDeal.Services
{
    public interface IPutawayService
    {
        Task<DefaultResponse> GetProductsDeliveryOrder(string DONumber);
        Task<DefaultResponse> GetTotalPuted(int DOProductId);
        Task<DefaultResponse> GetStorageCode(string StorageCode);

        Task<DefaultResponse> UpdatePutaway(PutawayModel model);
    }
    public class PutawayService : IPutawayService
    {
        public async Task<DefaultResponse> GetProductsDeliveryOrder(string DONumber)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/inventory/putaway/products/" + DONumber);

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
        public async Task<DefaultResponse> GetStorageCode(string StorageCode)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/inventory/putaway/storagecode/" + StorageCode);

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

        public async Task<DefaultResponse> GetTotalPuted(int DOProductId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/inventory/putaway/totalputed/" + DOProductId);

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

                    _error.StatusCode = "400";
                    _error.Error = "Error Exception";
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

        public async Task<DefaultResponse> UpdatePutaway(PutawayModel model)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string jsonPost = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonPost, Encoding.UTF8, "Application/json");
                    string url = AppConstant.BaseUrl + "/maui/inventory/putaway/sku?doproductid=" + model.DOProductId;
                    client.BaseAddress = new Uri(url);
                    var response = await client.PostAsync(url, content);

                    var _response = new DefaultResponse();
                    if (response.IsSuccessStatusCode)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response.Content.ReadAsStringAsync().Result;
                        return _response;

                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        _response.Code = response.StatusCode;
                        _response.Message = "Error Exception";
                        _response.Data = json;
                        return _response;
                    }
                    else
                    {
                        _response.Code = response.StatusCode;
                        _response.Message = "Error Exception";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.StatusCode = "400";
                    _error.Error = "Error Exception";
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
    }
}
