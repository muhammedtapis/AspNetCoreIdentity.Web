using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Service.Services
{
    public class HomeService : IHomeService
    {
        private readonly UserManager<AppUser> _userManager; //kullanıcı ile ilgili işlem için kullancağımız sınıf. Identity kütüphanesinden geliyo.

        private readonly SignInManager<AppUser> _signInManager;  //kullanıcı giriş için eklendi bu.

        private readonly IEmailService _emailService;

        private readonly ModelStateDictionary _modelStateDictionary;

        public HomeService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, ModelStateDictionary modelStateDictionary)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _modelStateDictionary = modelStateDictionary;
        }

      
        

    }
}
