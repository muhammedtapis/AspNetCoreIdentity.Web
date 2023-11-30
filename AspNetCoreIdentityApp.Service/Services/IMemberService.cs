using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Service.Services
{
    public interface IMemberService
    {
        Task<UserViewModel> GetUserViewModelByUserNameAsync(string username);

        Task LogOutAsync();

        Task<bool> CheckPasswordAsync(string username, string password);

        Task<(bool, IEnumerable<IdentityError>)> ChangePasswordAsync(string username, string oldpassword, string newpassword);

        Task<UserEditViewModel> GetUserEditViewModelAsync(string username);

        SelectList GetGenderSelectList();

        Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel request, string username);

        List<ClaimViewModel> GetClaims(ClaimsPrincipal principal);
    }
}