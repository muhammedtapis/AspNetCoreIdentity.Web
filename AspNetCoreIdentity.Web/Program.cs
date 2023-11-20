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


//namespace ekledik ve yukar�da yorumi�inde olan kodla ayn� i�i yap�yoruz.
builder.Services.AddIdentityWithExtension();

var app = builder.Build();


//yukar�daki kod blo�u �nceki projelerde kulland���n configureServices methoduyla ayn� servisleri ekledi�imiz yer.




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

app.MapControllerRoute( //olu�turulan arean�n readme.txt dosyas�ndaki kodu ekle -- bunu defaultun alt�nda yazarsan �al��m�yor d�zg�n.
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"); //bu area alan�na yani admin klas�r�ndeki  homecontrollera eri�mek i�in Home belirttik.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
