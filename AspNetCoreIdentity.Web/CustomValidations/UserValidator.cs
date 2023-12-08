using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            //custom user validator
            var errors = new List<IdentityError>();

            var isDigit = int.TryParse(user.UserName![0].ToString(), out _); //ikinci parametresi bize sayısal karakter varsa o değişkene atıyor ama biz kullanmıcaz bunu (out _) ile belirtik
                                                                             //usernamein ilk karakterinin int olup olmadığı kontrolü
            if (isDigit == true)
            {
                errors.Add(new IdentityError()
                {
                    Code = "UsernameContainFirstLetterDigit",
                    Description = "Kullanıcı adının ilk karakteri sayısal bir karakter olamaz!"
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray())); //bizden task dönmemizi istiyor ama async bir method kullanmadık nbu sebeple task ile wraplıyoruz.
                                                                                 //errors listesi doluysa bunları arraye atayıp dön.
            }

            return Task.FromResult(IdentityResult.Success); //eğer bir hata yoksa success.
        }
    }
}