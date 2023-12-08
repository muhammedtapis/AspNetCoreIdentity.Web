using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AspNetCoreIdentity.Service.Services
{
    public interface IHomeService
    {
        Task<SignInResult> EditUserAsync(SignInViewModel request, AppUser hasUser);

        Task<AppUser> HasUserSignInAsync(SignInViewModel request);

        Task<int> AccessFailedAccountAsync(AppUser hasUser);

        Task SignInBirthDateClaimAsync(AppUser hasUser, SignInViewModel request);

        Task<(bool, IEnumerable<IdentityError>?)> CreateUserAsync(SignUpViewModel request);

        Task ForgetPassword(IUrlHelper urlHelper, HttpContext HttpContext, AppUser hasUser);  //controller dışında Url ve HttpContext erişemediğimiz için parametre olarak istiyoruz.

        //for forgetpassword viewmodel
        Task<AppUser> HasUserForgetPasswordAsync(ForgetPasswordViewModel request);

        Task<AppUser> HasUserFindByIdAsync(string userId);

        //parola resetle geriye result ve hata listesi dön
        Task<(IdentityResult, IEnumerable<IdentityError>)> ResetPassword(AppUser hasUser, string token, string newPassword);

        Task<(bool, IEnumerable<IdentityError>)> ExternalResponse();
    }
}