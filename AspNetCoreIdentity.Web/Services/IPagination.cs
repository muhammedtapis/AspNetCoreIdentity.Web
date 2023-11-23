using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;


namespace AspNetCoreIdentity.Web.Services
{
    public interface IPagination
    {
       Task<List<UserViewModel>> UserList(UserManager<AppUser> userManager, int page, int pageSize);

    }
}
