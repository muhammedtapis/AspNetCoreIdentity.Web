using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Models;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtensions
    {
        //Program.cs dosyasındaki methodun katmanlı mimari şeklindeyazımı startup dosyasını kalabalık tutmamak için metodu burda yazdık
        public static void AddIdentityWithExtension(this IServiceCollection services)  //IServiceCollection için yazılan extension metodu.
                                                                                       //this olarak belirtmezsen çalışmaz o this methodu çağırdığın yerdeki IServiceCollectionu temsil Ediyor
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                ////password validation kuralları
                //options.User.RequireUniqueEmail = true;
                //options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";//username karakterleri

                //options.Password.RequiredLength = 6; //6 karakter olsun
                //options.Password.RequireNonAlphanumeric = false; //alphanumeric zorunlu değil * ?
                //options.Password.RequireLowercase = true; //küçük harf zorunlu
                //options.Password.RequireUppercase = false; //büyük harf zorunlu değil
                //options.Password.RequireDigit = false; //sayı zorunlu değil


            }).AddEntityFrameworkStores<AppDbContext>();
            //oluşturduğmuz custom passwordvalidatoru Identity ye belirttik.
        }
    }
}
