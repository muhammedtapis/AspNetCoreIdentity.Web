using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using Microsoft.EntityFrameworkCore;

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


//namespace ekledik ve yukarýda yorumiçinde olan kodla ayný iþi yapýyoruz.
builder.Services.AddIdentityWithExtension();

var app = builder.Build();


//yukarýdaki kod bloðu önceki projelerde kullandýðýn configureServices methoduyla ayný servisleri eklediðimiz yer.




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute( //oluþturulan areanýn readme.txt dosyasýndaki kodu ekle -- bunu defaultun altýnda yazarsan çalýþmýyor düzgün.
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); //bu area alanýna yani admin klasöründeki  homecontrollera eriþmek için Home belirttik.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
