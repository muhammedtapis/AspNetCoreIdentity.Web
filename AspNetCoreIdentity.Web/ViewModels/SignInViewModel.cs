using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
            
        }
        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [EmailAddress(ErrorMessage = "Email formatı yanlış")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
        [Display(Name = "Email :")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre :")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        public string? Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
