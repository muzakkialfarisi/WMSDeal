using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models
{
    public class DefaultResponse
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}
