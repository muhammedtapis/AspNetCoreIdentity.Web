using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetCoreIdentity.Web.TagHelpers
{
    //b taghelperi oluşturmamızın sebebi html dosyasında backend kodu yazıp kirletmemek 
    //taghelperi farklı bir AReada kullancağın için o areadaki viewimports dosyasında yoluyla eklemen gerek yoksa UserList.html sayfasında göremezsin
    public class UserRoleNamesTagHelper:TagHelper
    {
        public string userId { get; set; } = null!;

        private readonly UserManager<AppUser> _usermanager;

        public UserRoleNamesTagHelper(UserManager<AppUser> usermanager)
        {
            _usermanager = usermanager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _usermanager.FindByIdAsync(userId); //kullanıcıyı aldık
            var userRoles = await _usermanager.GetRolesAsync(user!); //kullanıcınn rollerini aldık

            var stringBuilder = new StringBuilder(); //string builder ile yanyana ekleyeceğiz o yüzden oluşturduk 

            userRoles.ToList().ForEach(x => //rolleri tek tek dönüp badge olarak ekledik yanyana
            {
                stringBuilder.Append(@$"<span class=""badge bg-secondary mx-1"">{x.ToLower()}</span>"); //yanyana ekleyecek append ile rolleri x burada rol oluyo
            });

            output.Content.SetHtmlContent(stringBuilder.ToString()); //bunu htmlde kullancaz tag helper ile
        }
    }
}
