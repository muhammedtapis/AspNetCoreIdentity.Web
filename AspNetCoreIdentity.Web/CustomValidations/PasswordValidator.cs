using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            //serverside custom validator.
            var errors = new List<IdentityError>();

            if (password!.ToLower().Contains(user.UserName!.ToLower())) //paswword ve username aynı şeyi içeriyorsa ve ! iki taraftan da kesin değer gelecek demeek
            {
                errors.Add(new IdentityError() { Code = "PasswordNoContainUsername", Description = "Şifre alanı kullanıcı adı içeremez!" });
            }
            if(password!.ToLower().StartsWith("1234")) //1234 ile başlarsa
            {
                errors.Add(new IdentityError() { Code = "PasswordNoContain1234", Description = "Şifre alanı ardışık sayı içeremez!" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray())); //bizden task dönmemizi istiyor ama async bir method kullanmadık nbu sebeple task ile wraplıyoruz.
            }

            return Task.FromResult(IdentityResult.Success); //eğer bir hata yoksa success.

            //bu oluşturduğun customValidator Iservice içinde eklemen lazım Identity ye haber vermen gerek.



        }
    }
}
