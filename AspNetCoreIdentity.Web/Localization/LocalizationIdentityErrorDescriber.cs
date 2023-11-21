using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Localization
{
    public class LocalizationIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "DuplicateUsername",
                Description = $" {userName} kullanıcı adı başka bir kullanıcı tarafından alınmıştır" //türkçeeştirdik mesajı override ederek.
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = "DuplicateEmail",
                Description = $" {email} email başka bir kullanıcı tarafından alınmıştır"
            };
            //return base.DuplicateEmail(email);
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = "PasswordTooShort",
                Description = $" Şifre en az  6 karakter olmalıdır."
            };
            
        }
    }
}
