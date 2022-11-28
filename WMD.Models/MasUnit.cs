using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public enum UnitOperantor
    {
        Multiply='*',
        Divide='/'
    }
    public class MasUnit
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The Unit Name is Required")]
        [StringLength(50)]
        public string UnitName { get; set; } = "";
        [StringLength(50)]
        public string ShortName { get; set; } = "";
        [StringLength(50)]
        public string BaseUnit { get; set; } = "";
        public UnitOperantor UnitOperantor { get; set; }
        public int OperatorValue { get; set; }
        [StringLength(100)]
        public string UnitDescription { get; set; } = "";
    }
}
