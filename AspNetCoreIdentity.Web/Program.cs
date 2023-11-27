using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.OptionsModels;
using AspNetCoreIdentity.Web.Services;
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


//a�a��daki methodu extension olarak Extension klas�r�nde tan�mlayaca�o�z.
//builder.Services.AddIdentity<AppUser,AppRole>(options =>
//{
//    options.User.RequireUniqueEmail = true;
//    options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";//username karakterleri

//    options.Password.RequiredLength = 6; //6 karakter olsun
//    options.Password.RequireNonAlphanumeric = false; //alphanumeric zorunlu de�il * ?
//    options.Password.RequireLowercase = true; //k���k harf zorunlu
//    options.Password.RequireUppercase = false; //b�y�k harf zorunlu de�il
//    options.Password.RequireDigit = false; //say� zorunlu de�il


//}).AddEntityFrameworkStores<AppDbContext>(); //identityi kullan�caz onu belirttik bu identitynin ald���
//iki parametre var appUser ve approle istiyo bizden daha sonra da
//bu identity k�t�phanesinin kullanaca�� dbContexti belirtiyoruz.




//namespace extension ekledik ve yukar�da yorumi�inde olan kodla ayn� i�i yap�yoruz.
builder.Services.AddIdentityWithExtension();

//EMAIL konfig�rasyon
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));  //appsettingsteki sectionu veriyoruz
builder.Services.AddScoped<IEmailService, EmailService>(); //IEmailService e herhangi bi class�n ctorunda kar��la��rsan bitane EmailService nesne �rne�i olu�tur demek.
builder.Services.AddScoped<IPagination, Pagination>();
//AddScope yapmam�z�n sebebi request ya�am d�ng�s� request response d�nd��� anda EmailService memoryden gitsin request gel�nce tekrar olu�tursun
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromSeconds(30); //30 dakikada bir security stamp de�eri kar��la�t�rmas� i�in konf.
});



//COOKIE options
builder.Services.ConfigureApplicationCookie(options =>
{

    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";

    //kullan�c�lar �ye olmadan �yelere �zel sayfalara giri� yapmaya �al��anlar i�in onlar� login sayfas�na y�nlendirdiyoruz
    //frameworke , �ye ol sayfas�n� belirtiyoruz

    options.LoginPath = new PathString("/Home/SignIn");   //giri�in yap�ld��� yer Identity buradan �yelerin girip giremeyece�i yerleri anl�yo girmesini istemedi�imiz controllerlara ya da sayfalara [Authorize] yaz�yoruz.
    options.LogoutPath = new PathString("/Member/LogOut");  //��k���n yap�ld��� yeri belirtiyoruz. y�nlendirilece�i sayfay� navbarLogin de verdik

    options.AccessDeniedPath = new PathString("/Member/AccessDenied"); //yetkisi olmayan kullan�c�lar�n y�nlendirildi�i sayfay� olu�turup accessdenied path verdik.

    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60); //cookie �mr�.
    options.SlidingExpiration = true;  //cookienin expiretimespan ini artt�rmaya yar�yor o 60 g�n i�inde bir kez giri� yap�lsa bile yine 60 g�n uzat�lcak.

});


//WWWROOT eri�im ayar� userpictures dosyas�na eri�mek i�in
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory())); //herhangi bir class�n constructorunda IFileprovider verirsen projedeki t�m klas�rlere eri�im sa�lars�n.!!!


var app = builder.Build();

//referans noktam�z ise i�i�nde oldu�umuz genel proje klas�r� olarak verdilk => Directory.GetCurrentDirectory()              





//yukar�daki kod blo�u �nceki projelerde kulland���n configureServices methoduyla ayn� servisleri ekledi�imiz yer.




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) //e�er development ortam�nda de�ilse Error.cshtml y�nlendir ama de�ilse normal exception patlat sayfada
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //kimlik do�rulama her zaman authorizationdan once gelir.
app.UseAuthorization();  //kimlik yetkilendirme

app.MapControllerRoute( //olu�turulan arean�n readme.txt dosyas�ndaki kodu ekle -- bunu defaultun alt�nda yazarsan �al��m�yor d�zg�n.
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); //bu area alan�na yani admin klas�r�ndeki  homecontrollera eri�mek i�in Home belirttik.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
