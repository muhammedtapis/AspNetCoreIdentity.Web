using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize] //sadece üyelerin erişebileceği controller içindeki sayfalar sadece yeler erişebilir.ÖNEMLİ bunu sadece ındex sayfasına da verebilirsin.
    public class MemberController : Controller
    {
        //çıkış işlemini signinmanager üzerinden signout işlemiyle yapcaz o yüzden bunu tanımladık.
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        //çıkışın 1. Yöntemi
        //üye giriş yaptıktan sonra navbardan çıkış _navbarLogin html dosyasında navbar action verildi.
        //public async Task<IActionResult> LogOut()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index","Home");
        //}

        public IActionResult Index() 
        {
            return View();
        }

        //çıkışı 2. yöntemi geri yönlendirilcek sayfayı burada değil navbarlogin.cshtml sayfasında vercez.
        //Program.cs dosyasında cookie builderda logoutpath vercez.
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
