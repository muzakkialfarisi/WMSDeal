using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models
{
    public class HomeDashboard
    {
        public string Title { get; set; }
        public int Total { get; set; }
        public int Done { get; set; }
        public int Outstanding { get; set; }
    }
}
