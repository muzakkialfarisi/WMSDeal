using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class MasProductBundlingData
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }
        public string BundlingId { get; set; }
        [ForeignKey("BundlingId")]
        public MasProductBundling? MasProductBundling { get; set; }
        public int Quantity { get; set; } = 1;
        public FlagEnum Flag { get; set; } = FlagEnum.Active;
    }
}