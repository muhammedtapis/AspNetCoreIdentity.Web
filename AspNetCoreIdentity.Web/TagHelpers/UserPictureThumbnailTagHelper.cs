using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCoreIdentity.Web.TagHelpers
{
    public class UserPictureThumbnailTagHelper:TagHelper
    {
        //bu taghelperi kullanıcı bilgileri gösterdiğimiz html dosyasında if else kodu yazıp htmli business kodla doldurmamak için yazıyoruz.
        public string? PictureUrl { get; set; } // Member/Index.cshtml sayfasındaki @Model.PictureUrl yerine geçecek.
        //img tagı oluşturacağız.

        //oluşturduktan sonra VIEWIMPORT tarafında eklemen lazım yoksa cshtmlde erişemezsin.
        //framework bunun taghelper olduğunu sınıfın sonundaki TagHelper dan anlıyor.
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img"; //bu taghelperın çıktısı img olcak
            
            if(string.IsNullOrEmpty(PictureUrl) ) //eğer null ise
            {
                output.Attributes.SetAttribute("src", "/userpictures/default_user_picture.png");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userpictures/{PictureUrl}"); //eğer modelden fotoğraf geliyosa kendi fotosunu göster
            }
            base.Process(context, output);  
        }
    }
}
