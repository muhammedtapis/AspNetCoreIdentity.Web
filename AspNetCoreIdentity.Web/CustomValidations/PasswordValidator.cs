using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser> //ınterface implement et bunu implement edince aşağıdaki methodu istiyor senden.
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password) //IdentityResult döncek.
        {
            //serverside custom validator.
            var errors = new List<IdentityError>();

            if (password!.ToLower().Contains(user.UserName!.ToLower())) //paswword ve username aynı şeyi içeriyorsa ve ! iki taraftan da kesin değer gelecek demeek
            {
                errors.Add(new IdentityError() { Code = "PasswordNoContainUsername", Description = "Şifre alanı kullanıcı adı içeremez!" });  //errors listesine ekle bu error varsa
            }
            if (password!.ToLower().StartsWith("1234")) //1234 ile başlarsa
            {
                errors.Add(new IdentityError() { Code = "PasswordNoContain1234", Description = "Şifre alanı ardışık sayı içeremez!" }); //errors listesine ekle bu error varsa
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray())); //bizden task dönmemizi istiyor ama async bir method kullanmadık nbu sebeple task ile wraplıyoruz.
                                                                                 //errors listesi doluysa bunları arraye atayıp dön.
            }

            return Task.FromResult(IdentityResult.Success); //eğer bir hata yoksa success.
            //Task.FromResult() içine vermiş olduğunuz methodu task ile wraplar biz yukarda asenkron bi method kullanmadık,bu sebeple wraplememiz gerekti.
            //bu oluşturduğun customValidator Iservice içinde eklemen lazım Identity ye haber vermen gerek.
        }
    }
}