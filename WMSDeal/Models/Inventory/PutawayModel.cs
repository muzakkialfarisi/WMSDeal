using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Inventory
{
    public class PutawayModel
    {
        public int DOProductId { get; set; }
        public string IKU { get; set; }
        public Guid StorageCode { get; set; }
        public int Quantity { get; set; }
    }
}
