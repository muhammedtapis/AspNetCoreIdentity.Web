using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Core.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre :")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)] //data tipini password verdik gözükmeyecek.
        [Compare(nameof(Password), ErrorMessage = "Girmiş olduğunuz şifre aynı değildir!")] //iki şifrenin aynı olup olmaması
        [Display(Name = "Yeni Şifre Tekrar :")] //htmlde gözükecek labeli
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        public string PasswordConfirm { get; set; } = null!;
    }
}