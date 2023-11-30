using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.Requirements
{
    public class ViolenceRequirement : IAuthorizationRequirement
    {
        public int thresholdAge { get; set; }  //eşik yaş
    }

    //handler sınıf

    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            //kullanıcının yaşı tutuyor mu bu sayfaya erişebilir mi onun testini yapıyoruz biz

            if (!context.User.HasClaim(x => x.Type == "birthdate")) //bu claim var mı yok mu bu claimi homecontrollerda ekledik ismi ordan geliyor
            {
                context.Fail(); //başarısız
                return Task.CompletedTask;
            }

            Claim birthdateClaim = context.User.FindFirst("birthdate")!;

            var today = DateTime.Now; //şuanki tarih
            var birthDate = Convert.ToDateTime(birthdateClaim.Value); //kullanıcının doğduğu tarih
            var age = today.Year - birthDate.Year; //yaş hesaplaması yaptık

            //artık yıl bilgisi 4 yılda bir 28 çeken şubat
            if (birthDate > today.AddYears(-age)) --age;//doğum tarihinden bugünün tarihini yaşı kadar çıkartıyoruz eğer bu şartı sğlıyorsa yaşını bir eksiltiyoruz.

            if (requirement.thresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask; //metodda async await kullanılmadığı için Task.CompletedTask yazıyoruz  bu işlemin bittiğini belirtmek için
        }
    }
}