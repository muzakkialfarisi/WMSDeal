using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasBrand
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        [MaxLength(100, ErrorMessage = "Brand Name can't be more than 100 characters")]
        [Required,Display(Name ="Brand Name")]
        public string BrandName { get; set; } = "";
        [StringLength(200)]
        [Display(Name ="Brand Image")]
        public string? BrandImage { get; set; }
        [NotMapped]
        public IFormFile? FormBrandImage { get; set; }
        [StringLength(250)]
        [Display(Name = "Brand Description")]
        public string BrandDescription { get; set; } = "";
    }
}
