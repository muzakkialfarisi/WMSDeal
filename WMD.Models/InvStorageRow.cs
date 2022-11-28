using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageRow
    {
        [Key]
        [StringLength(20),MaxLength(20,ErrorMessage ="Row Code cannot be longer than 20 characters")]
        [Display(Name ="Row Code")]
        public string RowCode { get; set; } = "";
        [StringLength(100,ErrorMessage ="Row Name cannot be longer than 100 characters")]
        [Display(Name ="Row Name")]
        public string RowName { get; set; } = "";
        [StringLength(150), Display(Name = "Plan Photo")]
        public string? RowPlanPhoto { get; set; }
        [NotMapped]
        [Display(Name ="Select Row Plan Photo")]
        public IFormFile? FormRowPlanPhoto { get; set; }
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        [Display(Name = "Zone")]
        [Required(ErrorMessage = "The Zone is Required")]
        [StringLength(20)]
        public string ZoneCode { get; set; } = "";
        [ForeignKey("ZoneCode")]
        [Display(Name ="Zone")]
        public InvStorageZone? InvStorageZone { get; set; }
        public string? HouseCode { get; set; } = "";
        [ForeignKey("HouseCode")]
        [Display(Name = "Warehouse")]
        public MasHouseCode? MasHouseCode { get; set; }
        public DateTime DateCreated { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; } = "";
        public DateTime DateModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; } = "";
        public virtual List<InvStorageColumn>? InvStorageColumns { get; set; } = new List<InvStorageColumn>();
        public virtual List<InvStorageSection>? InvStorageSections { get; set; } = new List<InvStorageSection>();
    }
}
