using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AspNetCoreIdentity.Service.Services
{
    public class HomeService : IHomeService
    {
        private readonly UserManager<AppUser> _userManager; //kullanıcı ile ilgili işlem için kullancağımız sınıf. Identity kütüphanesinden geliyo.

        private readonly SignInManager<AppUser> _signInManager;  //kullanıcı giriş için eklendi bu.

        private readonly IEmailService _emailService; //email reset password için

        //private readonly ServiceProvider _serviceProvider;

        //private readonly ModelStateDictionary _modelStateDictionary;

        public HomeService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        //hatalı giriş sayısı metodu
        public async Task<int> AccessFailedAccountAsync(AppUser hasUser)
        {
            return (await _userManager.GetAccessFailedCountAsync(hasUser));
        }

        public async Task<SignInResult> EditUserAsync(SignInViewModel request, AppUser hasUser)
        {
            //var hasUser = await HasUserAsync(request);
            //var controllerContext = _serviceProvider.GetService<ControllerContext>();

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password!, request.RememberMe, true);

            if (hasUser.BirthDate.HasValue) //kullanıcının giriş yaptığında doğum tarihi olmayabilir onu kontrol et eğer varsa claimle signin yap birthdate claim oluştur ve kullanıcının doğum tarihini ver
            {   //sadece login olduğunda eklencek bu claim çünkü signin action metodunda bunu membercontrollerda  userEdit e eklicez. bu claim db de Claims tablosunda tutulmuyor cookiede tutuluyo
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
            }

            return signInResult;

        }

        public async Task<AppUser> HasUserSignInAsync(SignInViewModel request)
        {
            return (await _userManager.FindByEmailAsync(request.Email!))!;
        }

        //kullanıcı birthdate claim oluşturma

        public async Task SignInBirthDateClaimAsync(AppUser hasUser, SignInViewModel request)
        {
            await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
        }

        //kullanıcı oluşturma
        public async Task<(bool, IEnumerable<IdentityError>?)> CreateUserAsync(SignUpViewModel request)
        {

            ////hash => password123121* => kajhsgdakhjdbaksjncakl  hashlenmiş data geri alamazsınız.
            //// encrypt  => paswrd123412 => aksjdnbaksjdna  bu datayıgeri döndürebilirsiniz encrypt edilen decrypt edeilirsiniz.
            ////hash algoritmaları MD5,SHA,512... istersek Identity hash algoritmasını değiştirebiliriz ama yapmıycaz defaultu güçlü bi algoritma
            ////dbde hata varsa aynı kullanıcı adı vs bu identityREsult ile alıyoruz.

            var identityResult = await _userManager.CreateAsync(new AppUser()
            { UserName = request.Username, PhoneNumber = request.Phone, Email = request.Email }, request.PasswordConfirm!);

            if (!identityResult.Succeeded)
            {
                //ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                //bi üst satırdaki extension alttaki foreachın yaptığı işi yapıyor hataları tek tekmodelstate ekliyor
                //foreach (IdentityError item in identityResult.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, item.Description);  //string.empty yani soldaki kısım bu hatanın nereye ait olduğunu nerde görünmesini istediiğini belirttiğin yer
                //                                                               //item.description ise hatanın mesajı buradaki item identityResult.Errors dan gelior.
                //}
                return (false, identityResult.Errors);
                //hata alırsak textboxların doldurulduğu view gösterilcek. almazsak redirecttoaction yaptık yukarda
            }

            //POLICY BASE yetkilendirme kullanıcı üye olursa belirli gün boyunca(10) bu sayfaya erişimi için oluşturcaz ona göre görebilcek
            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString()); //kullanıcı oluşturulan tarihten 10 gün sonrasını ver
            var user = await _userManager.FindByNameAsync(request.Username); //o anki eklenen kullanıcıyı al AddClaimAsync() için gerekiyo parametre olarak

            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);   //claim nesnesini buraya belirli kullanıcıya ekliyoruz
            if (!claimResult.Succeeded)//claim oluşturma hata alırsa
            {
                //ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return (false, claimResult.Errors);
            }

            return (true, null);


        }

        public async Task<AppUser> HasUserForgetPasswordAsync(ForgetPasswordViewModel request)
        {
            return (await _userManager.FindByEmailAsync(request.Email!))!;
        }
        public async Task ForgetPassword(IUrlHelper urlHelper, HttpContext HttpContext, AppUser hasUser)
        {

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser); //token oluştur hasUser kullanıcısı için.

            var passwordResetLink = urlHelper.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }
             , HttpContext.Request.Scheme);                        //url oluşturma birinci kısım action ikinci kısım controller
            //örnek link göndercez
            //https://localhost:7188?userId=12341&token=lakjbskahbsdiadjlnaksd  //token göndermemizin sebebi gönderilen bu şifre sıfırlama linkine geçerlilik süresi  vericez

            //emailservice
            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);


        }

        public async Task<AppUser> HasUserFindByIdAsync(string userId)
        {
            return (await _userManager.FindByIdAsync(userId))!;
        }

        public async Task<(IdentityResult, IEnumerable<IdentityError>)> ResetPassword(AppUser hasUser, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(hasUser, token, newPassword);

            return (result, result.Errors);
        }
    }


}
