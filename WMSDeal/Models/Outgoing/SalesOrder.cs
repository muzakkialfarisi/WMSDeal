using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Outgoing
{
    public class SalesOrder
    {
        [Key]
        public string OrderId { get; set; }
        public string SalesType { get; set; }
        public string StoreName { get; set; }
        public string Tenant { get; set; }
        public string ImageTenant { get; set; }
        public string CneeName { get; set; }
        public string CneeCity { get; set; }
        public DateTime DateOrdered { get; set; }
        public string Status { get; set; }
        public string HouseCode { get; set; }
        public int Total { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
