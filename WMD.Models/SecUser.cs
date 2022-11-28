using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class SecUser
    {
        [Key]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "{0} harus diisi"), StringLength(25, MinimumLength = 5)]
        public string UserName { get; set; } = "";
        [Required(ErrorMessage = "{0} harus diisi"), StringLength(100,MinimumLength =6), DataType(DataType.Password)]
        public string Password { get; set; } = "";
        [Required,StringLength(100)]
        public string Salt { get; set; } = "";
        [Required,StringLength(100),Display(Name ="First Name")]
        public string FirstName { get; set; } = "";
        [StringLength(100), Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [StringLength(100)]
        [DataType(DataType.EmailAddress,ErrorMessage ="Email Address Not Valid")]
        public string? Email { get; set; }
        [StringLength (100)]
        public string? PhoneNumber { get; set; }
        [StringLength(100)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email Address Not Valid")]
        public string? EmailConfirmed { get; set; }
        [StringLength(100)]
        public string? PhoneNumberConfirmed { get; set; }
        [Required ]
        public int ProfileId { get; set; }
        [Required]
        [StringLength(25)]
        public string HouseCode { get; set; } = "";
        [Required]
        public int JobPosId { get; set; }

        [DataType(DataType.Date),Display(Name ="Expired Date")]
        public DateTime ExpireDate { get; set; }
        public int ExpireFlag { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
        [StringLength(50), Display(Name = "Create by")]
        public string CreateBy { get; set; } = "";
        [DataType(DataType.Date)]
        public DateTime ModifiedDate { get; set; }= DateTime.Now;
        [StringLength(50), Display(Name = "Create by")]
        public string ModifiedBy { get; set; } = "";
        [StringLength(250)]
        public string ProfileImageUrl { get; set; } = "";
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }

        //ForeignKey
        [ForeignKey("ProfileId"),Display(Name ="Profile")]
        public SecProfile? SecProfile { get; set; }
        [ForeignKey("HouseCode"),Display(Name ="Warehouse")]
        public MasHouseCode? MasHouseCode { get; set; }
        [ForeignKey("JobPosId"),Display(Name ="Division")]
        public MasJabatan? MasJabatan { get; set; }

        public virtual SecUserTenant? SecUserTenant { get; set; }
        public virtual List<SecUserWarehouse>? SecUserWarehouses { get; set; } = new List<SecUserWarehouse>();
    }
}
