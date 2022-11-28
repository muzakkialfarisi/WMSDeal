using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Utility
{
    public class Jwt
    {
        public string key { get; set; } = "DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4";
        public string Issuer { get; set; } = "https://localhost:44391/";
        public string Audience { get; set; } = "https://localhost:44391/";
        public string Subject { get; set; } = "https://localhost:44391/";
    }
}
