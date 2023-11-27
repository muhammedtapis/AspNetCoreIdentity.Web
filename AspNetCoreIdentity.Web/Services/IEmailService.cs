namespace AspNetCoreIdentity.Web.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string resetPasswordEmailLink, string toEmail); //gönderilecek link ve kime gönderilceği parametre alıyo
        Task SendResetPasswordEmailWithMessage(string message,string resetPasswordEmailLink ,string toEmail);
    }
}
