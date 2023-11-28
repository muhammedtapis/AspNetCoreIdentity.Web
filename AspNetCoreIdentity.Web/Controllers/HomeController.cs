using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.Services;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NuGet.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //readonly dememizin sebebi sadece constructorda initialize edilmesin istiyoryz.const ekliyruz
        private readonly UserManager<AppUser> _userManager; //kullanıcı ile ilgili işlem için kullancağımız sınıf. Identity kütüphanesinden geliyo.

        private readonly SignInManager<AppUser> _signInManager;  //kullanıcı giriş için eklendi bu.

        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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

            var hasUser = await _userManager.FindByEmailAsync(request.Email!); //ViewModelden gelen modelin emaili varsa
                                                                               //eğer bu mailden databasede birden fazla varsa exception atıyor!!!!!!!!

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış"); //email kontrolu yaptık ama bu mesajı yazdırdık kötü kullanıcıya önlem için bu mesaj
                return View();
            }
            //ek claim cookie oluşturma login olduğunda çalışacak şiddetli videoların erişimi !

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password!, request.RememberMe, true); //parantezin içindeki en sağdaki false yanlış girişlerde sistemi kitleme oluyo
                                                                                                                               //kullanıcı 5 defa yanlış girse sistem kitleniyo bunu şimdilik false yaptık sonra trueçeviridk.
            if (signInResult.IsLockedOut) //lock olursa bura çalışcak
            {
                ModelState.AddModelErrorList(new List<string>() { "3ten fazla yanlış giriş yaptınız,3 dakika boyunca giriş yapamazsınız" });
                return View();//bunu burda yazmamızın sebebi email veya şifre yanlış hata mesajıyl aynı yerde olmasını istememiz,buraya girince burdaki view göndercek
            }

            if (!signInResult.Succeeded) //eğer giriş başarısız ise
            {
                ModelState.AddModelErrorList(new List<string>() { $"Email veya şifre yanlış", $"Başarısız giriş sayısı = {await _userManager.GetAccessFailedCountAsync(hasUser)}" });//başarısız giriş sayısını al karşılaştır
                return View();
            }

            if (hasUser.BirthDate.HasValue) //kullanıcının giriş yaptığında doğum tarihi olmayabilir onu kontrol et eğer varsa claimle signin yap birthdate claim oluştur ve kullanıcının doğum tarihini ver
            {   //sadece login olduğunda eklencek bu claim çünkü signin action metodunda bunu membercontrollerda  userEdit e eklicez.
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
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

            //hash => password123121* => kajhsgdakhjdbaksjncakl  hashlenmiş data geri alamazsınız.
            // encrypt  => paswrd123412 => aksjdnbaksjdna  bu datayıgeri döndürebilirsiniz encrypt edilen decrypt edeilirsiniz.
            //hash algoritmaları MD5,SHA,512... istersek Identity hash algoritmasını değiştirebiliriz ama yapmıycaz defaultu güçlü bi algoritma
            //dbde hata varsa aynı kullanıcı adı vs bu identityREsult ile alıyoruz.
            var identityResult = await _userManager.CreateAsync(new AppUser() { UserName = request.Username, PhoneNumber = request.Phone, Email = request.Email }
             , request.PasswordConfirm!);

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                //bi üst satırdaki extension alttaki foreachın yaptığı işi yapıyor hataları tek tekmodelstate ekliyor
                //foreach (IdentityError item in identityResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, item.Description);  //string.empty yani soldaki kısım bu hatanın nereye ait olduğunu nerde görünmesini istediiğini belirttiğin yer
                //                                                               //item.description ise hatanın mesajı buradaki item identityResult.Errors dan gelior.
                //}
                return View();
                //hata alırsak textboxların doldurulduğu view gösterilcek. almazsak redirecttoaction yaptık yukarda
            }

            //ViewBag.SuccessMessage = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir."; //signuphtml de göstercez
            //return View(); //aynı sayfaya geri döm ilk seçenekte kayıttan sonra aynı sayfaya geliyor textboxlar aynı bilgilerle dolu boş değil. bnda viewbag kullandık

            //ikinci seçenekte SignUp get methoduna gönderirsek textboxlar boş olarak gelir fakat ViewBag oraya gidemez o yğzden onun yerine TempData kullancaz.
            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir.";

            //POLICY BASE yetkilendirme kullanıcı üye olursa belirli gün boyunca(10) bu sayfaya erişimi için oluşturcaz ona göre görebilcek
            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString()); //kullanıcı oluşturulan tarihten 10 gün sonrasını ver
            var user = await _userManager.FindByNameAsync(request.Username); //o amki eklenen kullanıcıyı al AddClaimAsync() için gerekiyo parametre olarak

            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);   //claim nesnesini buraya belirli kullanıcıya ekliyoruz
            if (!claimResult.Succeeded)//claim oluşturma hata alırsa
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();
            }
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
            var hasUser = await _userManager.FindByEmailAsync(request.Email!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır.");
                return View(); //requestler durum tutmazlar bu sebeple return view dedik return direct deseydik modelStatedeki veriyi kaybederdik temp datayla taşımak zorunda kalırdık.
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser); //token oluştur hasUser kullanıcısı için.

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }
            , HttpContext.Request.Scheme);                        //url oluşturma birinci kısım action ikinci kısım controller
            //örnek link göndercez
            //https://localhost:7188?userId=12341&token=lakjbskahbsdiadjlnaksd  //token göndermemizin sebebi gönderilen bu şifre sıfırlama linkine geçerlilik süresi  vericez

            //email service
            var httpUtilityEncoded = HttpUtility.UrlEncode(request.Message);
            var textlink = @$"https://wa.me/905469334273/?text={httpUtilityEncoded}"; //mesajını whatsappa ekliyip sohbete geçiyo
            var numaram = "https://wa.me/905469334273";
            //await _emailService.SendResetPasswordEmailList(request.Message,numaram, hasUser.Email!);

            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

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

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!); //yukarıdaki null checkten geçtikleri için null olamazlar zaten o yğzden ! ile belirttik.

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password!); //bu işlemle kullanıcının şifresi yenileniyor.ResetPassword sayfasından alınan bilgilerle.

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir.";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());//şifre yenilemede hata varsa onu göstericez.
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}