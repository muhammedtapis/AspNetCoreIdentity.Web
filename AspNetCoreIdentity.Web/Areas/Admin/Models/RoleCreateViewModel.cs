using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Display(Name = "Rol İsim :")]
        [Required(ErrorMessage = "Rol İsmi alanı boş bırakılamaz!")]
        public string Name { get; set; } = null!;
    }
}
