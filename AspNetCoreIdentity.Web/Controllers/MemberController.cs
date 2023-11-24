using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.FileProviders;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize] //sadece üyelerin erişebileceği controller içindeki sayfalar sadece üyeler erişebilir.ÖNEMLİ bunu sadece ındex sayfasına da verebilirsin.
    public class MemberController : Controller
    {
        //çıkış işlemini signinmanager üzerinden signout işlemiyle yapcaz o yüzden bunu tanımladık.
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        //çıkışın 1. Yöntemi
        //üye giriş yaptıktan sonra navbardan çıkış _navbarLogin html dosyasında navbar action verildi.
        //public async Task<IActionResult> LogOut()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index","Home");
        //}

        public async Task<IActionResult> IndexAsync() 
        {
            //kullanıcı bilgisini alıp gösterme
            
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!; //o anki kllanıcının idsine ait olan isim
            var userViewModel = new UserViewModel() 
            { 
                UserName=currentUser.UserName,
                Email=currentUser.Email,
                PhoneNumber=currentUser.PhoneNumber,
                PictureUrl=currentUser.Picture
            };
            return View(userViewModel);
        }

        //çıkışı 2. yöntemi geri yönlendirilcek sayfayı burada değil navbarlogin.cshtml sayfasında vercez.
        //Program.cs dosyasında cookie builderda logoutpath vercez.
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request) 
        {
            if(!ModelState.IsValid) //html sayfasından gelen PasswordChangeViewModel boş değil kontrolü
            {
                return View();
            }

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!; //authorize attr. geçtiği için bu zaten bir üye bu sebeple Identity Name null olamaz.
            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser,request.PasswordOld);
            
            if(!checkOldPassword) //eğerki checkoldPassword false dönerse yani yanlış şifreyse
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser,request.PasswordOld, request.PasswordNew);

            if(!resultChangePassword.Succeeded) // pşifre değişme işlemi başarılı değilse.
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors); //hataları alıp açıklamalarını listeye alıp döner.
                return View();
            }


            //COOKIE YENİLEME İŞLEMİ KULLANICINI ÖNEMLİ BİLGİSİ DEĞİŞTİĞİ İÇİN COOKİE YENİLEMEMİZ GEREK Kİ DİĞER OTURUMLAR KAPANSIN
            await _userManager.UpdateSecurityStampAsync(currentUser); //önemli bilgiler değiştiğimiz için security stamp değerini güncelledik bunu en son yaparsan o kullanıcıya erişemezsin.veri gözükmez
            await _signInManager.SignOutAsync();//çıkış yaptırdık
            await _signInManager.PasswordSignInAsync(currentUser,request.PasswordNew,true,false);  // kullanıcı üyelik bilgisi kalıcı olsun = true,lockoutfailure = false sign in fail olursa lock olup olmadığının bilgisi
            //program.cs dosyasında securitystamp optionsta timei 1 yapıp denedik diğer sekmelerdeki oturumlar kapanıyor logine yönlendiriyor!
            TempData["SuccessMessage"] = "Şifre değiştirme işlemi başarıyla gerçekleşmiştir.";
            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender))); //model klasöründe oluşturduğumuz enum buraya verildi dropdown list için
         
            //sayfa açıldığında kullanıcının bilgilerinin textlere dolması lazım
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            var userEditViewModel = new UserEditViewModel()
            {
                Username = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender

            };
            return View(userEditViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if(!ModelState.IsValid)//gelen model maplemesi hatalıysa aynı sayfaya yönlendir.
            {
                return View();
            }
            //güncelleme
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!; //boş gelmeyeceğini belirt ! ile

            currentUser.UserName = request.Username;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            //fotoğrafı aşağıda vericez bu bilgileri güncelledikten sonra fotoğraf null değilse varsa orada eklicez

            if(request.Picture != null && request.Picture.Length > 0) 
            {
                
                var wwrootFolder = _fileProvider.GetDirectoryContents("wwwroot"); //AspNetCoreIdentity.Web klasöü referransımızdı onun altındaki wwwroot klasörüne erişim sağladık
                
                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";//  .jpg , .png Random resim dosyası ismi oluşturma  GUID ile

                //fotoğrafa directory yol ismi verdik userpicturesın fiziksel yolu ve randomfilename yolu birleştiriliyor.
                var newPicturePath = Path.Combine(wwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create); //stream aç fotoğrafın yolunu ver ve filemodu oluşturma olarak ver
                
                //requestten gelen picture dosyaını kopyala stream at 
                await request.Picture.CopyToAsync(stream);
                //kopyaladıktan sonra bu dosyanın yolunu da veritabanına yazdırmamız lazım,resim yolu kaydederken klasör yoluyla kaydedilmez dosyanın isimleriyle yazdır.

                //fotoğraf set

                currentUser.Picture = randomFileName; //sadece dosyanın ismi yazılıyor veritabanına.
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);  //geriye IdentityResult dönüyor daha sonra update işleminin hata varsa bunları ele alcaz.

            if(!updateToUserResult.Succeeded) 
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors); //OVERLOAD EDİLMİŞ METOT hataları gösteriyor.
                return View();
            }

            //username ve email gibi kritik bilgileri güncellediği için SecurityStamp güncellenmesi gerekiyor.
            await _userManager.UpdateSecurityStampAsync(currentUser);
            //daha sonra önce çıkış sonra tekrar giriş yapıcaz bunun sebebi eski cookienin içindeki bu bilgileri güncellemek.
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, isPersistent: true);

            TempData["SuccessMessage"] = "Üye bilgileri güncelleme işlemi başarıyla gerçekleşmiştir.";
            //güncelleme bitince butona bastıktan sonra güncel bilgilerin textlere dolmuş halini dön
            var userEditViewModel = new UserEditViewModel()
            {
                Username = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
            return View(userEditViewModel); //dönüş tipi html bizden viewmodel bekliyor!!!
        }
    }
}
