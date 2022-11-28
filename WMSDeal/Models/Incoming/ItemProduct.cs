using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Incoming
{
    public class ItemProduct
    {
        [Key]
        public string IKU { get; set; }
        public int DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public ProductData? ProductData { get; set; }
        public string StorageCode { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string StorageCategoryCode { get; set; }
        public string StorageCategoryName { get; set; }
        public string SizeCode { get; set; }
        public string SizeName { get; set; }
        public string BinCode { get; set; }
        public string BinName { get; set; }
        public string LevelCode { get; set; }
        public string LevelName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string RowCode { get; set; }
        public string RowName { get; set; }
        public Color Color { get; set; } = Color.FromHex("FFFFFF");
        public Color StatusColor { get; set; } = Color.FromHex("#ff0000");
    }
}
