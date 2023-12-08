using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentity.Core.ViewModels;

namespace AspNetCoreIdentity.Service.Services
{
    public interface IPagination
    {
        Task<List<UserViewModel>> UserList(UserManager<AppUser> userManager, int page, int pageSize);
    }
}