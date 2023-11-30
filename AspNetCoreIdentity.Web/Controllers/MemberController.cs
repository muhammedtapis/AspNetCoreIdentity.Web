using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.FileProviders;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.Security.Claims;
using AspNetCoreIdentity.Core.Models;
using AspNetCoreIdentity.Service.Services;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize] //sadece üyelerin erişebileceği controller içindeki sayfalar sadece üyeler erişebilir.ÖNEMLİ bunu sadece ındex sayfasına da verebilirsin.
    public class MemberController : Controller
    {
        //çıkış işlemini signinmanager üzerinden signout işlemiyle yapcaz o yüzden bunu tanımladık.
        //BU TANIMLAMALARI SERVİS OLUŞTURDUKTAN SONRA SİLEBİLİRİZ HERŞEYİ MEMBERSERVICE UZERİNDEN YAPCAZ
        //private readonly SignInManager<AppUser> _signInManager;
        //private readonly UserManager<AppUser> _userManager;
        //private readonly IFileProvider _fileProvider;


        //NLAYER servisleri buradan taşıyacağız fakat bundan önce biz bu metodlarda User.Identity.Name kullandığımız için burada readonly oluşturduk hep kullanıcaz çünkü SERVİS İÇİN.
        private string userName => User.Identity!.Name!; //authroize attrr var user null olamaz => ile tanımlarsan sadece get i oluyor.
        private readonly IMemberService _memberService;  //kullanıcı üye işlemlerini olduğu servisi çağırdık

        //ÖEMLİ NOT controller dışında Claims erişebilmek için kullandıımız HttpContext erişimi için aşağıdakini yazıp constructorda geç
        //PROGRAM.CS tarafında ise servis olarak eklemen lazım yoksa burda çağıramazsın.
        //private readonly IHttpContextAccessor _contextAccessor; _contextAccessor.HttpContext
        public MemberController(IMemberService memberService)
        {
            //_signInManager = signInManager;
            //_userManager = userManager;
            //_fileProvider = fileProvider;
            _memberService = memberService;
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
            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
        }

        //çıkışı 2. yöntemi geri yönlendirilcek sayfayı burada değil navbarlogin.cshtml sayfasında vercez.
        //Program.cs dosyasında cookie builderda logoutpath vercez.
        public async Task LogOut()
        {
            await _memberService.LogOutAsync();
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid) //html sayfasından gelen PasswordChangeViewModel boş değil kontrolü
            {
                return View();
            }

            if (!await _memberService.CheckPasswordAsync(userName, request.PasswordOld)) //eğerki checkoldPassword false dönerse yani yanlış şifreyse
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
                return View();
            }
            //serviste bu metod iki şey dönüyor
            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, request.PasswordOld, request.PasswordNew);

            if (!isSuccess) // pşifre değişme işlemi başarılı değilse.
            {
                ModelState.AddModelErrorList(errors!); //hataları alıp açıklamalarını listeye alıp döner.
                return View();
            }

            //program.cs dosyasında securitystamp optionsta timei 1 yapıp denedik diğer sekmelerdeki oturumlar kapanıyor logine yönlendiriyor!
            TempData["SuccessMessage"] = "Şifre değiştirme işlemi başarıyla gerçekleşmiştir.";
            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.GetGenderSelectList(); //model klasöründe oluşturduğumuz enum buraya verildi dropdown list için

            //sayfa açıldığında kullanıcının bilgilerinin textlere dolması lazım

            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)//gelen model maplemesi hatalıysa aynı sayfaya yönlendir.
            {
                return View();
            }

            var (isSuccess, errors) = await _memberService.EditUserAsync(request, userName); //edit metodunu çağır bu metod iki tip dönüyor.

            if (!isSuccess) //başarısız ise
            {
                ModelState.AddModelErrorList(errors!); //OVERLOAD EDİLMİŞ METOT hataları gösteriyor.
                return View();
            }

            TempData["SuccessMessage"] = "Üye bilgileri güncelleme işlemi başarıyla gerçekleşmiştir.";

            //güncelleme bitince butona bastıktan sonra güncel bilgilerin textlere dolmuş halini dön
            return View(await _memberService.GetUserEditViewModelAsync(userName)); //bu da viewmodel dönen metodumuzdu onu da çağırdık.dönüş tipi html bizden viewmodel bekliyor!!!

        }

        //access denied sayfasını bütün üyeler görebileceği için memberda yaptık.
        public IActionResult AccessDenied(string ReturnUrl)
        {
            //bu sayfayı program cs te belirtmen lazım AccessDenied sayfası olduğunu programa tanıtman gerke
            string message = string.Empty;
            message = "Bu sayfaya erişmeye yetkiniz yoktur ,yetki almak için yöneticinizle görüşebilirsiniz.";
            ViewBag.message = message;
            return View();
        }

        //claim listeleme
        public IActionResult Claims()
        {
            //User.Identity.Name değeri User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value(); eşittir aynı işi yapar bu kodlar
            return View(_memberService.GetClaims(User));  //burası önemli User verdik bu bi claimprincioal nesnesi claimlere ulaşmak için kullanıyoruz.
        }

        [Authorize(Policy = "İstanbulPolicy")]
        [HttpGet]
        public IActionResult IstanbulPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult ViolencePage()
        {
            return View();
        }
    }
}