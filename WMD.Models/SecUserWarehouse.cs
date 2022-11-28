using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class SecUserWarehouse
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public SecUser? SecUser { get; set; }

        public string? HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        public FlagEnum Flag { get; set; }
    }
}
