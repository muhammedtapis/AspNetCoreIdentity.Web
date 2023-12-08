using AspNetCoreIdentity.Core.Models;
using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task<UserViewModel> GetUserViewModelByUserNameAsync(string username)
        {
            //claims erişim controller sınıfında direkt User üzerinden erişebilirsin ama controller dışında HttpContext.User şeklinde erişebilirsin.
            //var userClaims = User.Claims.ToList();

            //email claimine erişim
            //var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            //kullanıcı bilgisini alıp gösterme

            var currentUser = (await _userManager.FindByNameAsync(username))!; //o anki kllanıcının idsine ait olan isim
            var userViewModel = new UserViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
            return userViewModel;
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> CheckPasswordAsync(string username, string password)
        {
            var currentUser = (await _userManager.FindByNameAsync(username))!; //authorize attr. geçtiği için bu zaten bir üye bu sebeple Identity Name null olamaz.
            return await _userManager.CheckPasswordAsync(currentUser, password); //true false döncek
        }

        //changePasswordAsync metodunu buraya taşıcaz bu metod iki şey dönğyo identityerrors yani başarılıysa true başarısızsa hata yani identity result
        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string username, string oldpassword, string newpassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(username))!;
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldpassword, newpassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }

            //COOKIE YENİLEME İŞLEMİ KULLANICINI ÖNEMLİ BİLGİSİ DEĞİŞTİĞİ İÇİN COOKİE YENİLEMEMİZ GEREK Kİ DİĞER OTURUMLAR KAPANSIN
            await _userManager.UpdateSecurityStampAsync(currentUser); //önemli bilgiler değiştiğimiz için security stamp değerini güncelledik bunu en son yaparsan o kullanıcıya erişemezsin.veri gözükmez
            await _signInManager.SignOutAsync();//çıkış yaptırdık
            await _signInManager.PasswordSignInAsync(currentUser, newpassword, true, false);  // kullanıcı üyelik bilgisi kalıcı olsun = true,lockoutfailure = false sign in fail olursa lock olup olmadığının bilgisi

            return (true, null); //task success ise true dönüyoruz o sebeple error yok null döncek diğer parametre
        }

        public async Task<UserEditViewModel> GetUserEditViewModelAsync(string username)
        {
            var currentUser = (await _userManager.FindByNameAsync(username))!;
            var userEditViewModel = new UserEditViewModel()
            {
                Username = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
            return userEditViewModel;
        }

        public SelectList GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel request,string username)
        {
            //güncelleme
            var currentUser = (await _userManager.FindByNameAsync(username))!; //boş gelmeyeceğini belirt ! ile

            currentUser.UserName = request.Username;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            //fotoğrafı aşağıda vericez bu bilgileri güncelledikten sonra fotoğraf null değilse varsa orada eklicez
            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwrootFolder = _fileProvider.GetDirectoryContents("wwwroot"); //AspNetCoreIdentity.Web klasöü referransımızdı onun altındaki wwwroot klasörüne erişim sağladık

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";//  .jpg , .png Random resim dosyası ismi oluşturma  GUID ile

                //fotoğrafa directory yol ismi verdik userpicturesın fiziksel yolu ve randomfilename yolu birleştiriliyor.
                var newPicturePath = Path.Combine(wwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create); //stream aç fotoğrafın yolunu ver ve filemodu oluşturma olarak ver

                //requestten gelen picture dosyaını kopyala stream at
                await request.Picture.CopyToAsync(stream);
                //kopyaladıktan sonra bu dosyanın yolunu da veritabanına yazdırmamız lazım,resim yolu kaydederken klasör yoluyla kaydedilmez dosyanın isimleriyle yazdır.

                //fotoğraf set

                currentUser.Picture = randomFileName; //sadece dosyanın ismi yazılıyor veritabanına.
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);  //geriye IdentityResult dönüyor daha sonra update işleminin hata varsa bunları ele alcaz.
            
            if(!updateToUserResult.Succeeded)
            {
                return (false, updateToUserResult.Errors);  //başarısızsa false ve hataları dön
            }

            //username ve email gibi kritik bilgileri güncellediği için SecurityStamp güncellenmesi gerekiyor.
            await _userManager.UpdateSecurityStampAsync(currentUser);
            //daha sonra önce çıkış sonra tekrar giriş yapıcaz bunun sebebi eski cookienin içindeki bu bilgileri güncellemek.
            await _signInManager.SignOutAsync();

            //birthdate POLICY eklicez çünkü burada birthdate güncellemesi yapılıyor ama eğer viewmodelden gelen  tarih değeri var ise
            if (request.BirthDate.HasValue)
            {   //doğum tarihi bilgisi varsa claimle giriş yap  doğum tarihi claim oluştur
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDate!.Value.ToString()) });
            }
            else
            {   //doğum tarihi bilgisi yoksa claimsiz giriş yap.
                await _signInManager.SignInAsync(currentUser, isPersistent: true);
            }

            //her şey başarılı ise
            return (true, null);
        }

        public List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
        {
            //burda yapılan okumayı framework claim içinden yapıyor
            var userClaimList = principal.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();

            return userClaimList;
        }
    }
}