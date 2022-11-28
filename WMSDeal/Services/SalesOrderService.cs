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
using WMSDeal.Models.Outgoing;

namespace WMSDeal.Services
{
    public interface ISalesOrderService
    {
        Task<DefaultResponse> GetSalesOrders(string HouseCode = null, string Status = null, string FlagPick = null,string? Periode=null);
        Task<DefaultResponse> AssignSalesOrder(string HouseCode, List<SalesOrderAssign> model);
        Task<DefaultResponse> GetSalesOrderPick();
        Task<DefaultResponse> SavePick(PickOrder model);
        Task<DefaultResponse> Staging(Staging mmodel);

    }

    public class SalesOrderService : ISalesOrderService
    {
        public async Task<DefaultResponse> AssignSalesOrder(string HouseCode, List<SalesOrderAssign> model)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string jsonPost = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonPost, Encoding.UTF8, "Application/json");
                    string url = AppConstant.BaseUrl + "/maui/outgoing/pick/assign?housecode=" + HouseCode;
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

        public async Task<DefaultResponse> GetSalesOrderPick()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/Outgoing/pick/pickorder");
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
                catch (Exception ex)
                {
                    var _error = new ErrorResponse();

                    _error.Message = ex.Message;
                    _error.Code = "SO6001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.Unauthorized;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetSalesOrders(string HouseCode = null, string Status = null, string FlagPick = null,string? Periode=null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/outgoing/pick/salesorder?housecode=" + HouseCode + "&status=" + Status + "&flagpick=" + FlagPick + "&PeriodeOrder=" + Periode);

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

        public async Task<DefaultResponse> SavePick(PickOrder model)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string url = AppConstant.BaseUrl + "/maui/outgoing/pick?id=" + model.Id + "&storagecode=" + model.StorageCode;
                    string jsonPost = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonPost, Encoding.UTF8, "Application/json");
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

        public async Task<DefaultResponse> Staging(Staging model)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string url = AppConstant.BaseUrl + "/maui/outgoing/pick/staging?userid=" + model.UserId + "&pickassignid=" + model.PickAssignId;
                    string jsonPost = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(jsonPost, Encoding.UTF8, "Application/json");
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
