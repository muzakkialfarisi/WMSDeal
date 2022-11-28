using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class SecProfileMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required ]
        public int ProfileId { get; set; }
        [Required,StringLength(25)]
        public string MenuId { get; set; } = "";
        public char IsView { get; set; }
        public char IsInsert { get; set; }
        public char IsEdit { get; set; }
        public char IsDelete { get; set; }
        public char IsPrint { get; set; }
        [StringLength(5)]
        public FlagEnum Flag { get; set; }

        [ForeignKey("ProfileId")]
        public SecProfile? SecProfile { get; set; }
        [ForeignKey("MenuId")]
        public SecMenu? SecMenu { get; set; }

    }
}
