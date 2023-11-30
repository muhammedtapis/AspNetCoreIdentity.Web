using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Core.ViewModels
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
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre :")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string Password { get; set; } = null!;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}