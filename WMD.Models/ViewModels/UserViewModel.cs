using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models.ViewModels
{
    public class UserViewModel
    {
        public Guid UserId { get; set; } 
        [Display(Name ="User Name")]
        public string UserName { get; set; } = "";
        [ Display(Name = "First Name")]
        public string FirstName { get; set; } = "";
        [ Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [DataType(DataType.EmailAddress,ErrorMessage ="Email Address Not Valid")]
        public string? Email { get; set; }
        [Display(Name ="Phone")]
        public string? PhoneNumber { get; set; }
        public string? EmailConfirmed { get; set; }
        public string? PhoneNumberConfirmed { get; set; }
        [Display(Name ="Profile")]
        public int ProfileId { get; set; }
        [Display(Name ="Warehouse")]
        public string HouseCode { get; set; } = "";
        [Display(Name ="Jabatan")]
        public int JobPosId { get; set; }
        [StringLength(150)]
        public string? ProfileImageUrl { get; set; } 
        [Display(Name = "Profile Image")]
        [NotMapped]
        public IFormFile? ProfileImage { get; set; }

        [ Display(Name = "Expired Date"),DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}")]
        public DateTime ExpireDate { get; set; }
        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        //ForeignKey
        [ForeignKey("ProfileId"), Display(Name = "Profile")]
        public SecProfile? SecProfile { get; set; }
        [ForeignKey("JobPosId"), Display(Name = "Level")]
        public MasJabatan? MasJabatan { get; set; }
        [ForeignKey("HouseCode"), Display(Name = "Warehouse")]
        public MasHouseCode? MasHouseCode { get; set; }

        public virtual SecUserTenant? SecUserTenant { get; set; }
        public virtual List<SecUserWarehouse>? SecUserWarehouses { get; set; } = new List<SecUserWarehouse>();
    }
}
