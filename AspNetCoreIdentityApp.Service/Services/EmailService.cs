using AspNetCoreIdentity.Core.OptionsModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentity.Service.Services
{
    public class EmailService : IEmailService
    {
        //appsettingsdevelopmentta ayar yapcaz daha sonra kullanabilmek için burada readonly options çağıracağız constructorda belirticez

        private readonly EmailSettings _emailSettings;

        //daha sonra frameworke dicez ki herhangi bir classın constructorunda IOptions<EmailSettings> kullandığımda bana EmailSettingsten nesne örneği ver
        //bunu da program.cs te builder.services.configure<EmailSettings> ile vericez

        //CTOR
        public EmailService(IOptions<EmailSettings> emailOptions) //buradaki emailoptions.value değeri bize zaten EmailSettings döndüğü için readonlyde sadece emailsettings tanımlasak yeterli
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string toEmail)
        {
            var smtpClient = new SmtpClient();
            smtpClient.Host = _emailSettings.Host!; //hostu belirledik appsettings sayfasından geliyo bilgi bu emailSettings sınıfı ordaki bilgilerle maplendi.
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //network kanalı gönderilcek method
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587; //bu port encrypted mail transmissions için.
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password); //mail kullanıcı adı ve şifre bu şifreyi mailde özel kod oluşturmuştuk o.
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email!); //mesajın gönderileceği adres
            mailMessage.To.Add(toEmail); //mesajın gideceği adres
            mailMessage.Subject = "Localhost | Şifre Sıfırlama Linki";
            mailMessage.Body = @$"
                         <h4>Şifreninizi yenilemek için aşağıdaki linke tıklayınız.</h4>
                         <p><a style=""color:"" href='{resetPasswordEmailLink}'>Click for Feedback</a></p>
                         ";

            mailMessage.IsBodyHtml = true; //yukarda html kullandık onu true set ettik.

            await smtpClient.SendMailAsync(mailMessage);

            //en son bu servisi sisteme vermen lazım. program.cs te
        }

        public async Task SendResetPasswordEmailWithMessage(string message, string resetPasswordEmailLink, string toEmail)
        {
            var smtpClient = new SmtpClient();
            smtpClient.Host = _emailSettings.Host!; //hostu belirledik appsettings sayfasından geliyo bilgi bu emailSettings sınıfı ordaki bilgilerle maplendi.
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; //network kanalı gönderilcek method
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587; //bu port encrypted mail transmissions için.
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password); //mail kullanıcı adı ve şifre bu şifreyi mailde özel kod oluşturmuştuk o.
            smtpClient.EnableSsl = true;
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email!); //mesajın gönderileceği adres
            mailMessage.To.Add(toEmail); //mesajın gideceği adres
            mailMessage.Subject = "Fıstık | Diyara Mamysheva";

            mailMessage.Body = @$"
            <p>{message}</p>
            <p><a href='{resetPasswordEmailLink}'>Click</a></p>";

            mailMessage.IsBodyHtml = true; //yukarda html kullandık onu true set ettik.
            await smtpClient.SendMailAsync(mailMessage);

            //en son bu servisi sisteme vermen lazım. program.cs te
        }
    }
}