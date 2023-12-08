using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AspNetCoreIdentity.Core.PermissionRoot;

namespace AspNetCoreIdentity.Repository.Seeds
{
    public static class PermissionSeed
    {//permission sınıfındaki dataları birleştirmek için
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole"); //rol var mı
            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole"); //rol var mı
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole"); //rol var mı

            if (!hasBasicRole)//bu rol yoksa
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" }); //yoksa bu rolu oluştur

                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;  //oluşturduğun bu role ulaştık

                await AddReadPermission(roleManager, basicRole);
            }

            if (!hasAdvancedRole)//bu rol yoksa
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" }); //yoksa bu rolu oluştur

                var basicRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;  //oluşturduğun bu role ulaştık

                await AddReadPermission(roleManager, basicRole);
                await AddUpdateAndCreatePermission(roleManager, basicRole);
            }

            if (!hasAdminRole)//bu rol yoksa
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" }); //yoksa bu rolu oluştur

                var basicRole = (await roleManager.FindByNameAsync("AdminRole"))!;  //oluşturduğun bu role ulaştık

                await AddReadPermission(roleManager, basicRole);
                await AddUpdateAndCreatePermission(roleManager, basicRole);
                await AddDeletePermission(roleManager, basicRole);
            }
        }

        public static async Task AddReadPermission(RoleManager<AppRole> roleManager, AppRole role)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Read)); //claim oluşturup ekledik rolü

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Read));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Read));
        }

        public static async Task AddUpdateAndCreatePermission(RoleManager<AppRole> roleManager, AppRole role)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Create)); //claim oluşturup ekledik

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Create));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Create));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Update)); //claim oluşturup ekledik

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Update));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Update));
        }

        public static async Task AddDeletePermission(RoleManager<AppRole> roleManager, AppRole role)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Stock.Delete)); //claim oluşturup ekledik

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Order.Delete));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permission.Catalog.Delete));
        }
    }
}