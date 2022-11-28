using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models
{
    public class ErrorResponse
    {
        public string StatusCode { get; set; } = HttpStatusCode.InternalServerError.ToString();
        public string Error { get; set; } = "Internal Server Error";
        public string Message { get; set; } = "An internal server error occurred";
        public string Code { get; set; } = "1230";
    }
}
