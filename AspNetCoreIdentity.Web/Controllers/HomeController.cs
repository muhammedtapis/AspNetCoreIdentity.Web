using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //readonly dememizin sebebi sadece constructorda initialize edilmesin istiyoryz.const ekliyruz
        private readonly UserManager<AppUser> _userManager; //kullanıcı ile ilgili işlem için kullancağımız sınıf. Identity kütüphanesinden geliyo.

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
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

        //bu method kullanıcı bir de kayıt ol butonuna bastığında post haline gelecek.veri gönderiyoruz.
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request) 
        {


            if(!ModelState.IsValid) 
            {
                return View();
            }

            //hash => password123121* => kajhsgdakhjdbaksjncakl  hashlenmiş data geri alamazsınız.
            // encrypt  => paswrd123412 => aksjdnbaksjdna  bu datayıgeri döndürebilirsiniz encrypt edilen decrypt edeilirsiniz.
            //hash algoritmaları MD5,SHA,512... istersek Identity hash algoritmasını değiştirebiliriz ama yapmıycaz defaultu güçlü bi algoritma
            //dbde hata varsa aynı kullanıcı adı vs bu identityREsult ile alıyoruz.
            var identityResult = await _userManager.CreateAsync(new AppUser() { UserName = request.Username, PhoneNumber = request.Phone, Email = request.Email }
             , request.PasswordConfirm);


            if (identityResult.Succeeded)
            {
                //ViewBag.SuccessMessage = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir."; //signuphtml de göstercez
                //return View(); //aynı sayfaya geri döm ilk seçenekte kayıttan sonra aynı sayfaya geliyor textboxlar aynı bilgilerle dolu boş değil. bnda viewbag kullandık

                //ikinci seçenekte SignUp get methoduna gönderirsek textboxlar boş olarak gelir fakat ViewBag oraya gidemez o yğzden onun yerine TempData kullancaz.
                TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıyla gerçekleştirilmiştir.";
                return RedirectToAction(nameof(HomeController.SignUp)); //signup get methoduna yönlendir.
            }
            foreach (IdentityError item in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);  //string.empty yani soldaki kısım bu hatanın nereye ait olduğunu nerde görünmesini istediiğini belirttiğin yer
                                                                           //item.description ise hatanın mesajı buradaki item identityResult.Errors dan gelior.
            }
            return View();
            //hata alırsak

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}