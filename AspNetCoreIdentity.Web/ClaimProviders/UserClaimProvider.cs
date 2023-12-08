using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.ClaimProviders
{
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //authorize attr olduğu her yerde burası çalışıyor.sebebini tam anlamadım ama
        //cookieden almış olduğu değerleri User üzerinden claims dönüştürüyor.
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //diyoruz ki principaldan gelen Identity  bir CLaimsIdentity olacak bu claim üzerinden kullanıcının claimlerine erişeceğiz
            var claimIdentityUser = principal.Identity as ClaimsIdentity;

            var currentUser = await _userManager.FindByNameAsync(claimIdentityUser!.Name!);
            if (currentUser == null || String.IsNullOrEmpty(currentUser.City)) //eğer kullanıcı yoksa veya kullanıcının citysi yoksa principal dön
            {
                return principal;
            }
            if (!principal.HasClaim(x => x.Type == "city")) //cookide bu data var mı yok mu ona bakıyoruz yani city adında bi claim var mı yok mu
            {
                Claim cityClaim = new Claim("city", currentUser.City); //eğer yoksa claim nesnei oluşturup cookiye kaydedicez
                claimIdentityUser.AddClaim(cityClaim);
            }

            //yukarıdaki if bloğuna girdiği anda principal güncellenmiş oluyor.
            return principal;
        }
    }
}