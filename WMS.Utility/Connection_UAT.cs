using PdfSharpCore.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Utility
{
    public class Connection_UAT
    {
        public string Data_Source { get; set; } = "Data Source=172.25.0.93;";
        public string Password { get; set; } = "Password=WMScore2022;";
        public string Persist_Security_Info { get; set; } = "Persist Security Info=True;";
        public string User_ID { get; set; } = "User ID=sa;";
        public string Initial_Catalog { get; set; } = "Initial Catalog=UAT_WMSDeal;";
        public string Connect_Timeout { get; set; } = "connect timeout=0;";
        public string Max_Pool_Size { get; set; } = "max pool size=5000000;";
        public string TrustServerCertificate { get; set; } = "TrustServerCertificate=True;";
        
    }
}
