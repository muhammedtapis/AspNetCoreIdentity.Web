namespace AspNetCoreIdentity.Web.OptionsModels
{
    //bu sınıfı oluşturmamızın sebebi email kullanmak için appsettings dosyasında tanımlamalara yaptık onlara erişimi tip güvenli şekilde sağlamak.
    public class EmailSettings
    {    
        public string? Host { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
