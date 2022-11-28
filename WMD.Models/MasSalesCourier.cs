using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasSalesCourier
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Url { get; set; } = "";

        public ICollection<OutsalesOrderDelivery>? OutsalesOrderDelivery { get; set; }
    }
}
