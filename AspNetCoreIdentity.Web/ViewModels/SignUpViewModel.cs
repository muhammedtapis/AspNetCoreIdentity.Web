using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class SignUpViewModel
    {
        //apuser signup karşılamak için oluşturduğumuz viewmodel
        public SignUpViewModel()
        {
            
        }
        public SignUpViewModel(string username, string email, string phone, string password)
        {
            Username = username;
            Email = email;
            Phone = phone;
            Password = password;

        }
        //htmlde belirtmek için display verdik tek tek orda yazmak istemiyoruz koddan gelsin istiyoruz
        [Display(Name ="Kullanıcı Adı :")]
        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz!")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Email formatı yanlış")] //email formatını kontrol
        [Display(Name = "Email :")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
        public string? Email { get; set; }

        [Display(Name = "Telefon :")]
        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz!")]
        public string? Phone { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre :")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Girmiş olduğunuz şifre aynı değildir!")] //iki şifrenin aynı olup olmaması
        [Display(Name = "Şifre Tekrar :")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz!")]
        public string? PasswordConfirm { get; set; }

    }
}
