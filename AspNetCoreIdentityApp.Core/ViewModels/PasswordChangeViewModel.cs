using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Şifre :")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string PasswordOld { get; set; } = null!;  //nullable olamaz

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre :")]
        [Required(ErrorMessage = "Yeni Şifre alanı boş bırakılamaz!")]
        public string PasswordNew { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar:")]
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifrele aynı değildir")]
        [Required(ErrorMessage = "Şifre Tekrar alanı boş bırakılamaz!")]
        public string PasswordNewConfirm { get; set; } = null!;
    }
}