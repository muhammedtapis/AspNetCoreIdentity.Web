using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.ClaimProviders;
using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Core.OptionsModels;
using AspNetCoreIdentity.Core.PermissionRoot;
using AspNetCoreIdentity.Web.Requirements;
using AspNetCoreIdentity.Repository.Seeds;
using AspNetCoreIdentity.Service.Services;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options =>
    {
        options.MigrationsAssembly("AspNetCoreIdentity.Repository"); //repository katman�nda migration olu�acak.
    });
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

//controller d���ndaki s�n�flarda claim eri�imi i�in kullan�lan private readonly IHttpContextAccessor _contextAccessor; interface eklenmesi i�in gereken servis
//builder.Services.AddHttpContextAccessor();

//olu�turdu�umuz Claim provider frameworke bildirim
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
//�ehir bilgisi �zerinden yetkilendirme yapmak i�in policy ekleme
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("�stanbulPolicy", policy =>
    {
        policy.RequireClaim("city", "�stanbul");  //city bilgisinde istanbul olanlar istanbulPolicy hangi sayfaya uygulad�ysam oraya eri�ebilir onun d���ndakiler yapamaz.
        //policy.RequireRole("role", "admin"); //bu �ekilde rol de belirtebiliriz
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
        //policy.AddRequirements(new ExchangeExpireRequirement() { Age = 31 }); //parametre g�ndermek istersek bu s�n�fta prop tan�mlay�p burda vercez.
    });

    options.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement() { thresholdAge = 18 });
        //policy.AddRequirements(new ExchangeExpireRequirement() { Age = 31 }); //parametre g�ndermek istersek bu s�n�fta prop tan�mlay�p burda vercez.
    });

    //bu policyde ���ne de sahip olanlar eri�im sa�layabilir !!!!!
    options.AddPolicy("StockDeleteOrderDeleteAndReadPermissionPolicy", policy =>
    {
        policy.RequireClaim("permission", Permission.Stock.Delete); //sa�daki alan string olu�turdu�umuz s�n�ftan gelio
        policy.RequireClaim("permission", Permission.Order.Delete);
        policy.RequireClaim("permission", Permission.Order.Read); //bunlar� ayr� ayr� yaz�p bekledik ki hepsine sahip olan bu policynin �artlar� sa�las�n.
        //policy.REquireClaim progra aya�a kalk�nca olu�turdu�umuz claimlerden 3 tanesini burada verdik dikkat edelim burada bir role tan�mlamas� yok .!!!
    });

    options.AddPolicy("Permission.Order.ReadPolicy", policy =>
    {
        policy.RequireClaim("permission", Permission.Order.Read);
    });

    options.AddPolicy("Permission.Order.DeletePolicy", policy =>
    {
        policy.RequireClaim("permission", Permission.Order.Delete);
    });

    options.AddPolicy("Permission.Stock.DeletePolicy", policy =>
    {
        policy.RequireClaim("permission", Permission.Stock.Delete);
    });
});

//e�er Iauthorization interface g�r�rsen bu interface kar��l�k benim olu�turdu�um s�n�f�n nesne �rne�ini ol�tur.POLICY BASE  yetki i�in requirementda
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

//Policy base 18 ya� s�n�r�
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();

//service katman�ndaki IMemberService programa tan�t�m
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddScoped<IHomeService, HomeService>();

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

//permission seed claim i�in !! uygulamano�n bir kez aya�a kalkt��� yer buras� bunun da bi kez aua�a kalkmas�n� istiyotuz o sebeble burda yazd�k

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    await PermissionSeed.Seed(roleManager);  //scopelar biti�i anda roleManager memoryden d��ecek.
}

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