using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentity.Core.Models;

namespace AspNetCoreIdentity.Repository.Models
{
    //kullanıcı bilgileri
    public class AppUser : IdentityUser
    {
        //kullanıcı ilk üye olduğunda bu bilgileri istemiycez iye olduktan sonra userEdit sayfasından güncelleyebilcek.
        public string? City { get; set; }

        public string? Picture { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }  //genderi oluşturduğumuz gender enum tipine verdik.
    }
}