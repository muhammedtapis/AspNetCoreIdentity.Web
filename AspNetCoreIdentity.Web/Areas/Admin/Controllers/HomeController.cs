using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    //bu controllerın hangi area aşt olduğun belrt.
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager; //Appuser listeleyeceğimiz için burada readonly tanımladık constr. vericez

        private readonly IPagination _pagination;

        public HomeController(UserManager<AppUser> userManager, IPagination pagination)
        {
            _userManager = userManager;
            _pagination = pagination;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList() //kullanıcıları listeleyeceğimiz metod get metodu.
        {
            var userList = await _userManager.Users.ToListAsync(); //usermanagerdeki userları listele

            var userViewModelList = userList.Select(x => new UserViewModel() //admin kullanıcının göreceği user bilgileri userlistten tek tek seç mapleme yap
            {
                Id = x.Id,
                Name = x.UserName,
                Email = x.Email,
            }).ToList();
            return View(userViewModelList);

            //pagination yapan method ÇALIŞTI LANN
            //return View(await _pagination.UserList(_userManager, 1, 4));
        }

        //[HttpPost]

        //public async Task<IActionResult> UserList(UserViewModel request)
        //{
        //}
    }
}