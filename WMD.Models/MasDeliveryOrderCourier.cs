using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasDeliveryOrderCourier
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<IncDeliveryOrder>? IncDeliveryOrders { get; set; }
    }
}
