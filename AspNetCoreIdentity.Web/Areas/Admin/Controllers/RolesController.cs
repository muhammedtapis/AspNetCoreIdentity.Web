using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            return View(roles);
        }

        [Authorize(Roles = "role-action")]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name }); // yeni bir rol ekleme

            if (!result.Succeeded)//başarısız ise
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol oluşturulmuştur.";
            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            //hangi rolü update edeceğiz onun rol bilgisini çek göster.
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır");
            }

            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate!.Name! });
        }

        [HttpPost]
        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır");
            }
            //update
            roleToUpdate.Name = request.Name;

            await _roleManager.UpdateAsync(roleToUpdate);
            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir.";
            return View();
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleDelete(string id) //bu metodda post a gerek yok listeden direkt seçip idsini alıp sileceğiz
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete == null)
            {
                throw new Exception("Silinecek rol bulunamamıştır");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }
            TempData["SuccessMessage"] = "Rol bilgisi silinmiştir.";  //tempdata yapmamızın sebebi roleDelete den roleList yani indexe yönlendiriyoruz başka metoda gidiyor.
            return RedirectToAction(nameof(RolesController.Index));
        }

        //rol atama yapacağımız sayfa bu atamayı userlist sayfasında yapmadık direkt oraya bi buton koyup bu methoda gönderdik.
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            ViewBag.userId = id; //html sayfasında asp-route-id vererek bu idyi post metoduna yollayacağız

            var currentUser = (await _userManager.FindByIdAsync(id))!; //güncel kullanıcıyı al

            var roles = await _roleManager.Roles.ToListAsync();  //genel rol bilgilerini al ne kadar rol varsa

            //AssignRoleToUSerViewModel i rol listesi olarak dönüp checkbox ta göstermek için ihtiyacımız var.

            var assignRoleToUserViewModelList = new List<AssignRoleToUserViewModel>(); //post metoduna göndereceğimiz rollerin listesi

            //kullanıcının rolü  var mı yok mu onun kontrolü için.

            var userRoles = await _userManager.GetRolesAsync(currentUser); //kullanıcının rollerni alıp userRoles ataması yaptık aşağıda kontrol yapacağız.

            foreach (var role in roles) //tüm rolleri tek tek dön
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name! }; //rolleri modele ata

                if (userRoles.Contains(role.Name!)) //şu anki kullanıcınn rolü rollerin içinde bulunuyorsa
                {
                    assignRoleToUserViewModel.Exist = true; //existi true yap checkbox için
                }

                assignRoleToUserViewModelList.Add(assignRoleToUserViewModel); //roleViewModelList e roleViewModeli ekle
            }
            return View(assignRoleToUserViewModelList); //bu viewmodeli liste halinde view sayfasına gönderdik orada foreach ile dönüp satırlara checkbox durumuna ekleyeceğiz.
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(List<AssignRoleToUserViewModel> requestList, string userId)
        {
            //Rol ataması yapınca buraya rollerin listesi geliyor ama hangi kullanıcıya rol atayacağımız bilmek için get metodundan id yi buraya çekmemiz lazım onun için viewbag kullanıcaz.
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!; //AssignRoleToUser html sayfasında rol ata tıkladığımzdaki kullanıcının idsi buraya geldi ve kullanıcıyı bulduk

            foreach (var role in requestList)
            {
                if (role.Exist) //eğerki belirli rol kullanıcıda true ise butona basınca post edince kullanıcıya o rolü ata
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else //eğer exist durumu false ise yani checkbox işaretlenmemişse eklemiceksin kaldıracaksın.
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }
            return RedirectToAction(nameof(HomeController.UserList), "Home"); //burada tekrar "Home" neden verdik onu tam anlamadım.
        }
    }
}