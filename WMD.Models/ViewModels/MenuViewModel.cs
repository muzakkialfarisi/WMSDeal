using System.ComponentModel.DataAnnotations;

namespace WMS.Models.ViewModels
{
    public class MenuViewModel
    {
        [Required(ErrorMessage = "{0} harus diisi"), StringLength(25)]
        public string MenuId { get; set; } = "";
        [Required(ErrorMessage = "{0} harus diisi"), StringLength(25)]
        public string ParentId { get; set; } = "";
        [Required(ErrorMessage = "{0} harus diisi")]
        [StringLength(150, ErrorMessage = "{0} tidak boleh lebih dari {1} karakter")]
        public string MenuName { get; set; } = "";
        [Required]
        public int MenuSort { get; set; } =0;
        [StringLength(50)]
        public string Controller { get; set; } = "";
        [StringLength(50)]
        public string Action { get; set; } = "";
        [StringLength(50)]
        public string IconClass { get; set; } = "";
        [Required, StringLength(50)]
        public string MenuGroup { get; set; } = "";
        [StringLength(25)]
        public string MenuKey { get; set; } = "";

    }
}
