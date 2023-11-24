namespace AspNetCoreIdentity.Web.OptionsModels
{
    //bu sınıfı oluşturmamızın sebebi email kullanmak için appsettings dosyasında tanımlamalara yaptık onlara erişimi tip güvenli şekilde sağlamak.
    public class EmailSettings
    {    
        public string Host { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
