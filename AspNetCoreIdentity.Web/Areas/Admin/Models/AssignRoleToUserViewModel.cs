namespace AspNetCoreIdentity.Web.Areas.Admin.Models
{
    public class AssignRoleToUserViewModel
    {
        //buradaki bilgileri sayfaya dolduracağımız için oluşturduk bu modeli
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;

        //bu rol kullanıcıda var mı yok mu onun kontrolü için bool değer
        public bool Exist { get; set; }
    }
}
