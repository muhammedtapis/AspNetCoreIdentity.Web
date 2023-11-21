using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Localization;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtensions
    {
        //Program.cs dosyasındaki methodun katmanlı mimari şeklindeyazımı startup dosyasını kalabalık tutmamak için metodu burda yazdık
        public static void AddIdentityWithExtension(this IServiceCollection services)  //IServiceCollection için yazılan extension metodu.
                                                                                       //this olarak belirtmezsen çalışmaz o this methodu çağırdığın yerdeki IServiceCollectionu temsil Ediyor
        {

            //url için oluşturulan tokena süre vercez

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(2);  //tokenin öçmrü iki saat olcak
            });


            services.AddIdentity<AppUser, AppRole>(options =>
            {
                //aşağıdaki options kodlarını yazdığın zaman default server validator hata veriyor 
                ////password validation kuralları
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";//username karakterleri

                options.Password.RequiredLength = 6; //6 karakter olsun
                options.Password.RequireNonAlphanumeric = false; //alphanumeric zorunlu değil * ?
                options.Password.RequireLowercase = true; //küçük harf zorunlu
                options.Password.RequireUppercase = false; //büyük harf zorunlu değil
                options.Password.RequireDigit = false; //sayı zorunlu değil
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); //lockout mekanizmasını 3 dakikaya ayarladık.
                options.Lockout.MaxFailedAccessAttempts = 3;  //3 yanlış girişte kitlenecek.


            })
            .AddPasswordValidator<PasswordValidator>() //bu alan customvalidatorü identitymize tanıtıp çağırmak için.
            .AddUserValidator<UserValidator>()        //custom user validator.
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>() //mesajları türkçeye çevirdiğimiz sınıfı verdik.
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();    //şifre yenileme linki için token kullancaz burada ekliyoruz identitye
            //oluşturduğmuz custom passwordvalidatoru Identity ye belirttik.
        }
    }
}
