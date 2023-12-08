using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Service.Services;
using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NuGet.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //readonly dememizin sebebi sadece constructorda initialize edilmesin istiyoryz.const ekliyruz
        //private readonly UserManager<AppUser> _userManager; //kullanıcı ile ilgili işlem için kullancağımız sınıf. Identity kütüphanesinden geliyo.

        //private readonly SignInManager<AppUser> _signInManager;  //kullanıcı giriş için eklendi bu.

        //private readonly IEmailService _emailService;

        private readonly IHomeService _homeService;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, /*UserManager<AppUser> userManager, SignInManager<AppUser> signInManager ,IEmailService emailService,*/ IHomeService homeService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            //_userManager = userManager;
            //_signInManager = signInManager;
            //_emailService = emailService;
            _homeService = homeService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()  //get request default belirtmezsen get olur.
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        //ilk amaç kullanıcıya giriş yaptırıp cookie oluşturmak bununiçin SignInManager eklicez yukarda.

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null) //string returnUrl vermemizin sebebi kullanıcı başka bir sayfada biyere tıklayınca eğer
        {                                                                                        //bu sayfa üye girişi gerektiriyorsa buraya yönlendirme yapmak için verdik.
                                                                                                 //her zaman vermek zounda olmadığımız için de null atadık default
                                                                                                 //kullanıcı login olduktan sonra girmeye çalıştığı bir önceki sayfaya yönlendirme yapmak.
            if (!ModelState.IsValid) //html sayfasından gelen SignInViewModel boş değil kontrolü
            {
                return View();
            }

            returnUrl = returnUrl ?? Url.Action("Index", "Home");  //eğer gelen değer null ise Url.Action() methodu çalışacak homecontroller anasayfaya gidecek.

            var hasUser = await _homeService.HasUserSignInAsync(request);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış"); //email kontrolu yaptık ama bu mesajı yazdırdık kötü kullanıcıya önlem için bu mesaj
                return View();
            }

            var signResult = await _homeService.EditUserAsync(request, hasUser);

            //ek claim cookie oluşturma login olduğunda çalışacak şiddetli videoların erişimi !

            //var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password!, request.RememberMe, true); //parantezin içindeki en sağdaki false yanlış girişlerde sistemi kitleme oluyo
            //kullanıcı 5 defa yanlış girse sistem kitleniyo bunu şimdilik false yaptık sonra trueçeviridk.
            if (signResult.IsLockedOut) //lock olursa bura çalışcak
            {
                ModelState.AddModelErrorList(new List<string>() { "3ten fazla yanlış giriş yaptınız,3 dakika boyunca giriş yapamazsınız" });
                return View();//bunu burda yazmamızın sebebi email veya şifre yanlış hata mesajıyl aynı yerde olmasını istememiz,buraya girince burdaki view göndercek
            }

            if (!signResult.Succeeded) //eğer giriş başarısız ise
            {
                ModelState.AddModelErrorList(new List<string>() { $"Email veya şifre yanlış", $"Başarısız giriş sayısı = {await _homeService.AccessFailedAccountAsync(hasUser)}" });//başarısız giriş sayısını al karşılaştır
                return View();
            }

            if (hasUser.BirthDate.HasValue) //kullanıcının giriş yaptığında doğum tarihi olmayabilir onu kontrol et eğer varsa claimle signin yap birthdate claim oluştur ve kullanıcının doğum tarihini ver
            {   //sadece login olduğunda eklencek bu claim çünkü signin action metodunda bunu membercontrollerda  userEdit e eklicez. bu claim db de Claims tablosunda tutulmuyor cookiede tutuluyo
                await _homeService.SignInBirthDateClaimAsync(hasUser, request);
            }

            return Redirect(returnUrl!);

            //bu extensionu modelstate için oluşturduk bu sayede ModelState. yazıp ulaşabiliyoryz.
            //ModelState.AddModelErrorList(new List<string>(){ "Email veya şifre yanlış"});

            //hata alırsak o anki viewi göster. almazsan returnurlden gelen bilgiyi göster
        }

        //bu method kullanıcı bir de kayıt ol butonuna bastığında post haline gelecek.veri gönderiyoruz.
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (isSuccess, errors) = await _homeService.CreateUserAsync(request);

            if (!isSuccess) //başarısız ise
            {
                ModelState.AddModelErrorList(errors!); //OVERLOAD EDİLMİŞ METOT hataları gösteriyor.
                return View();
            }

            ////ViewBag.SuccessMessage = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir."; //signuphtml de göstercez
            ////return View(); //aynı sayfaya geri döm ilk seçenekte kayıttan sonra aynı sayfaya geliyor textboxlar aynı bilgilerle dolu boş değil. bnda viewbag kullandık

            ////ikinci seçenekte SignUp get methoduna gönderirsek textboxlar boş olarak gelir fakat ViewBag oraya gidemez o yğzden onun yerine TempData kullancaz.

            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir.";

            return RedirectToAction(nameof(HomeController.SignIn)); //signin get methoduna yönlendir giriş sayfasına.
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            //user emaili  var mı  kontrolü
            var hasUser = await _homeService.HasUserForgetPasswordAsync(request);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır.");
                return View(); //requestler durum tutmazlar bu sebeple return view dedik return direct deseydik modelStatedeki veriyi kaybederdik temp datayla taşımak zorunda kalırdık.
            }

            //şifre unuttum maili gönderme servisi
            await _homeService.ForgetPassword(this.Url, this.HttpContext, hasUser); // controller dışında url ve httpcontext erişimi olmadığı çin parametre yolladık.

            TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderilmiştir";

            return RedirectToAction(nameof(ForgetPassword)); //return view yaparsak göndere bastıktan sonra sayfayı yenilerse o maili tekrar göndericek o yüzden HttpGet ForgetPAssword gönderdik.
        }

        public IActionResult ResetPassword(string userId, string token) //get methodundaki resetpasswordün linkinde bize lazım olan userid ve token alanları var onları post methoduna göndermemiz gerek.
        {
            TempData["UserId"] = userId; //bu tempdatalar bir defa okunabilir sayfa yenilenirse bu okunamaz kullanıcı emaildeki linke tıklayınca bu bilgiler
            TempData["Token"] = token;   //linkte yazılı gelir framework bu bilgileri ForgetPassword actionunda oluşturulan passwordResetLinkteki userId ve Token ifadeleriyle
                                         //Tempdata içine yazdığımız UserId ve Token tamamen aynı olduğu için kendisi mapliyor. !!!!!!!

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["UserId"]; //ResetPassword post methodunda userid ve token bilgilerine ihtiyacımız olduğu için tempdata ile tuttuk ve burada aldık.
            var token = TempData["Token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi");
                //ileriki derslerde bu exceptionu Error.cshtml sayfasına yönlendircez
            }

            //var hasUser = await _userManager.FindByIdAsync(userId.ToString()!); //yukarıdaki null checkten geçtikleri için null olamazlar zaten o yğzden ! ile belirttik.
            var hasUser = await _homeService.HasUserFindByIdAsync(userId.ToString()!);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır");
                return View();
            }

            //bu işlemle kullanıcının şifresi yenileniyor.ResetPassword sayfasından alınan bilgilerle.
            var (result, errors) = await _homeService.ResetPassword(hasUser, token.ToString()!, request.Password!);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir.";
            }
            else
            {
                ModelState.AddModelErrorList(errors);//şifre yenilemede hata varsa onu göstericez.
            }
            return View();
        }

        public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
        {
            var (isSuccess, errors) = (await _homeService.ExternalResponse(ModelState));
            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors); //responsedan gelen Identity errors modele eklendi
                List<string> errorList = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList(); ///hatalar listeye atıldı error sayfasında karşılıcaz.
                return View("GoogleAuthenticationError", errorList); //hatalar error sayfasına gönderildi.
            }
            return RedirectToAction(returnUrl);
        }

        public IActionResult GoogleLogin(string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalResponse", "Home", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GoogleAuthenticationError()
        {
            return View();
        }
    }
}