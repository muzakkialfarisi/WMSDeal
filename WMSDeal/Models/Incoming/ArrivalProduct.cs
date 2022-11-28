using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Incoming
{
    public class ArrivalProduct
    {
        public int DOProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class DeliveryOrderUpload
    {
        public string DONumber { get; set; }
        public string? NotaImage { get; set; }

    }

    public class DeliveryOrderArrival
    {
        public int DOProductId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public string? ProductImage { get; set; }
        public string? NotaImage { get; set; }
        public DateTime DateArrived { get; set; } = DateTime.Now;
    }
}
