using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Dashboard1> Dashboard { get; set; } = new List<Dashboard1>();

        public List<int> MonthlyIncoming { get; set; } = new List<int>();
        public List<int> MonthlyOutgoing { get; set; } = new List<int>();
    }

    public class Dashboard1
    {
        public string HouseCode { get; set; } = string.Empty;
        public string HouseName { get; set; } = string.Empty;
        public int Incoming { get; set; }
        public int IncomingProduct { get; set; }
        public int IncomingQuantity { get; set; }
        public int Inventory { get; set; }
        public int Outgoing { get; set; }
        public int OutgoingProduct { get; set; }
        public int OutgoingQuantity { get; set; }
    }
}
