using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.Requirements
{
    //policy bazlı yetkilendirme için oluşturuldu . kullanıcı oluşturulunca 10 gün boyunca Exchange page girebilcek süre geçince giremeyecek.
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
        //eğer program.cs ten parametre yollamak istersek buraya da prop ekliyoruz. daha sonra aşağıdaki Handle metodunda requirement üzerinden erişebiliriz.
        //public int Age { get; set; }
    }

    //handler sınıfı

    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            //kullanıcı login olduktan sonra bu sayfaya erişebilir mi onun testini yapıyoruz biz
            var hasExchangeExpireClaim = context.User.HasClaim(x => x.Type == "ExchangeExpireDate"); //bu kullanıcının kullanıcı oluşturulrken oluşrturduğumuz  ExchangeExpireDate claimi varmı

            if (!hasExchangeExpireClaim) //bu claim var mı yok mu
            {
                context.Fail(); //başarısız
                return Task.CompletedTask;
            }

            Claim exchangeExpireDateClaim = context.User.FindFirst("ExchangeExpireDate")!;

            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDateClaim.Value)) //eğer şimdiki tarih claimden gelen tarihten büyükse tarih geçmiştir sğre dolmuş yine fail
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask; //metodda async await kullanılmadığı için Task.CompletedTask yazıyoruz  bu işlemin bittiğini belirtmek için
        }
    }
}