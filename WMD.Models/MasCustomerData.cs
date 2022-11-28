using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasCustomerData
    {
        [Key]
        public Guid CustId { get; set; }
        [Required(ErrorMessage ="{0} Required")]
        [StringLength(250,MinimumLength =5,ErrorMessage ="{0} tidak boleh lebih dari {1} karakter")]
        [Display(Name = "Customer Name")]
        public string CustName { get; set; } = "";
        [StringLength(250)]
        public string Address { get; set; } = "";
        [StringLength(20)]
        [Display(Name = "Kelurahan")]
        public string KelId { get; set; } = "";
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }
              [StringLength(25)]
        public string KodePos { get; set; } = "";
        [StringLength(50)]
        public string Email { get; set; } = "";
        [StringLength(25)]
        [Display(Name = "Office Phone")]
        public string OfficePhone { get; set; } = "";
        [StringLength(25)]
        [Display(Name = "HP Number")]
        public string HandPhone { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "Rekening Number")]
        public string RekeningNo { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; } = "";
        [Required,StringLength(50)]
        [Display(Name = "Term of Payment")]
        public string TermOfPayment { get; set; } = "";
        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }  
        public decimal SaldoAkhir { get; set; }  
        [Required, Display(Name = "Customer Type")]
        public int CustTypeId { get; set; }
        [ForeignKey("CustTypeId"), Display(Name = "Customer Type")]
        public MasCustomerType? MasCustomerType { get; set; }
        [Required, Display(Name = "Industry Id")]
        public int IndustryId { get; set; }
        [ForeignKey("IndustryId")]
        [Display(Name = "Industry")]
        public MasIndustry? MasIndustry { get; set; }
        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
