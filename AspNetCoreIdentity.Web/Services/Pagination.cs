using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCoreIdentity.Web.Services
{
    public class Pagination : IPagination
    {

        //veriuyi sayfalayan metod
        public async Task<List<UserViewModel>> UserList(UserManager<AppUser> userManager, int page, int pageSize)
        {
            var userList = userManager.Users
            .OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var userViewModelList = userList.Select(x => new UserViewModel()//admin kullanıcının göreceği user bilgileri userlistten tek tek seç mapleme
            {
                Id = x.Id,
                Name = x.UserName,
                Email = x.Email,

            }).Where(x => x.Name == "mami" || x.Name == "ali").ToList();

            return await Task.FromResult(userViewModelList);

        }

    }
}
