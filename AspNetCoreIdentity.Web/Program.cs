using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.ClaimProviders;
using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.OptionsModels;
using AspNetCoreIdentity.Web.Requirements;
using AspNetCoreIdentity.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

//aþaðýdaki methodu extension olarak Extension klasöründe tanýmlayacaðoýz.
//builder.Services.AddIdentity<AppUser,AppRole>(options =>
//{
//    options.User.RequireUniqueEmail = true;
//    options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";//username karakterleri

//    options.Password.RequiredLength = 6; //6 karakter olsun
//    options.Password.RequireNonAlphanumeric = false; //alphanumeric zorunlu deðil * ?
//    options.Password.RequireLowercase = true; //küçük harf zorunlu
//    options.Password.RequireUppercase = false; //büyük harf zorunlu deðil
//    options.Password.RequireDigit = false; //sayý zorunlu deðil

//}).AddEntityFrameworkStores<AppDbContext>(); //identityi kullanýcaz onu belirttik bu identitynin aldýðý
//iki parametre var appUser ve approle istiyo bizden daha sonra da
//bu identity kütüphanesinin kullanacaðý dbContexti belirtiyoruz.

//namespace extension ekledik ve yukarýda yorumiçinde olan kodla ayný iþi yapýyoruz.
builder.Services.AddIdentityWithExtension();

//EMAIL konfigürasyon
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));  //appsettingsteki sectionu veriyoruz
builder.Services.AddScoped<IEmailService, EmailService>(); //IEmailService e herhangi bi classýn ctorunda karþýlaþýrsan bitane EmailService nesne örneði oluþtur demek.
builder.Services.AddScoped<IPagination, Pagination>();
//AddScope yapmamýzýn sebebi request yaþam döngüsü request response döndüðü anda EmailService memoryden gitsin request gelýnce tekrar oluþtursun

//controller dýþýndaki sýnýflarda claim eriþimi için kullanýlan private readonly IHttpContextAccessor _contextAccessor; interface eklenmesi için gereken servis
//builder.Services.AddHttpContextAccessor();

//oluþturduðumuz Claim provider frameworke bildirim
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
//þehir bilgisi üzerinden yetkilendirme yapmak için policy ekleme
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ÝstanbulPolicy", policy =>
    {
        policy.RequireClaim("city", "Ýstanbul");  //city bilgisinde istanbul olanlar istanbulPolicy hangi sayfaya uyguladýysam oraya eriþebilir onun dýþýndakiler yapamaz.
        //policy.RequireRole("role", "admin"); //bu þekilde rol de belirtebiliriz
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
        //policy.AddRequirements(new ExchangeExpireRequirement() { Age = 31 }); //parametre göndermek istersek bu sýnýfta prop tanýmlayýp burda vercez.
    });
});

//eðer Iauthorization interface görürsen bu interface karþýlýk benim oluþturduðum sýnýfýn nesne örneðini olþtur.POLICY BASE  yetki için requirementda
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpirationRequirementHandler>();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromSeconds(30); //30 dakikada bir security stamp deðeri karþýlaþtýrmasý için konf.
});

//COOKIE options
builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";

    //kullanýcýlar üye olmadan üyelere özel sayfalara giriþ yapmaya çalýþanlar için onlarý login sayfasýna yönlendirdiyoruz
    //frameworke , üye ol sayfasýný belirtiyoruz

    options.LoginPath = new PathString("/Home/SignIn");   //giriþin yapýldýðý yer Identity buradan üyelerin girip giremeyeceði yerleri anlýyo girmesini istemediðimiz controllerlara ya da sayfalara [Authorize] yazýyoruz.
    options.LogoutPath = new PathString("/Member/LogOut");  //çýkýþýn yapýldýðý yeri belirtiyoruz. yönlendirileceði sayfayý navbarLogin de verdik

    options.AccessDeniedPath = new PathString("/Member/AccessDenied"); //yetkisi olmayan kullanýcýlarýn yönlendirildiði sayfayý oluþturup accessdenied path verdik.

    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60); //cookie ömrü.
    options.SlidingExpiration = true;  //cookienin expiretimespan ini arttýrmaya yarýyor o 60 gün içinde bir kez giriþ yapýlsa bile yine 60 gün uzatýlcak.
});

//WWWROOT eriþim ayarý userpictures dosyasýna eriþmek için
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); //herhangi bir classýn constructorunda IFileprovider verirsen projedeki tüm klasörlere eriþim saðlarsýn.!!!

var app = builder.Build();

//referans noktamýz ise içiçnde olduðumuz genel proje klasörü olarak verdilk => Directory.GetCurrentDirectory()

//yukarýdaki kod bloðu önceki projelerde kullandýðýn configureServices methoduyla ayný servisleri eklediðimiz yer.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) //eðer development ortamýnda deðilse Error.cshtml yönlendir ama deðilse normal exception patlat sayfada
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //kimlik doðrulama her zaman authorizationdan once gelir.
app.UseAuthorization();  //kimlik yetkilendirme

app.MapControllerRoute( //oluþturulan areanýn readme.txt dosyasýndaki kodu ekle -- bunu defaultun altýnda yazarsan çalýþmýyor düzgün.
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); //bu area alanýna yani admin klasöründeki  homecontrollera eriþmek için Home belirttik.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();