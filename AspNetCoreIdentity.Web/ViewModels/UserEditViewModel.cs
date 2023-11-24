using AspNetCoreIdentity.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class UserEditViewModel
    {
        [Display(Name = "Kullanıcı Adı :")]
        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz!")]
        public string Username { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email formatı yanlış")] //email formatını kontrol
        [Display(Name = "Email :")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
        public string Email { get; set; } = null!;

        [Display(Name = "Telefon :")]
        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz!")]
        public string Phone { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi :")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Şehir :")]
        public string? City { get; set; }

        [Display(Name = "Profil Resmi :")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Cinsiyet :")]
        public Gender? Gender { get; set; }
    }
}
