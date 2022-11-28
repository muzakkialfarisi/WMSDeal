using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class InvStorageCategory
    {
        [Key]
        public int StorageCategoryId { get; set; }
        [Required,StringLength(25,MinimumLength =2),Display(Name ="Code")]
        public string StorageCategoryCode { get; set; } = "";
        [Required,StringLength (25,MinimumLength =2),Display(Name ="Name")]
        public string StorageCategoryName { get; set;} = "";
        public FlagEnum Flag { get; set; }
    }
}
