using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    //bu controllerın hangi area aşt olduğun belrt.
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager; //Appuser listeleyeceğimiz için burada readonly tanımladık constr. vericez

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList() //kullanıcıları listeleyeceğimiz metod get metodu.
        {
            var userList = await _userManager.Users.ToListAsync(); //usermanagerdeki userları listele

            var userViewModelList = userList.Select(x => new UserViewModel() //admin kullanıcının göreceği user bilgileri userlistten tek tek seç 
            {
                Id = x.Id,
                Name = x.UserName,
                Email = x.Email
            }).ToList();
            return View(userViewModelList);
        }
    }
}
