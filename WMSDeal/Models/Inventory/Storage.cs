using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Inventory
{
    public class Storage
    {
        [Key]
        public string StorageCode { get; set; }
        public string BinCode { get; set; }
        public string LevelCode { get; set; }
        public string SectionCode { get; set; }
        public string RowCode { get; set; }
        public string RowName { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneName { get; set; }
        public string HouseCode { get; set; }
        public string HouseName { get; set; }
        public string SizeCode { get; set; }
        public string SizeName { get; set; }
        public string Flag { get; set; }
    }
}
