namespace AspNetCoreIdentity.Core.ViewModels
{
    public class UserViewModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }  //null olabilir veritabanında baktık

        //kullanıcı fotoğrafını Member/Index sayfasında kullanıcı bilgileri bölümünde gösterebilmek için property eklemesi yapıyoruz.
        public string? PictureUrl { get; set; } //null olabilir her kullanıcının fotoğrafı olmak zorunda değil default picture atıcaz
    }
}