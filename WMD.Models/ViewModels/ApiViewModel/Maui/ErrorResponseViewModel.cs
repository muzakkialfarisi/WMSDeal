using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models.ViewModels.ApiViewModel.Maui
{
    public class ErrorResponseViewModel
    {
        public string StatusCode { get; set; } = StatusCodes.Status500InternalServerError.ToString();
        public string Error { get; set; } = "Internal Server Error";
        public string Message { get; set; } = "Can't connect to server!";
        public string Code { get; set; } = "ER001";
    }
}
