using System.Net.Mail;
using System.Net;

namespace AutenticaAPI;

public class EmailSender: IEmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config)
    {
        _config = config;
    }
    public async Task<bool>  EnviaEmail(Email email)
    {
        try
        {

            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_config.GetSection("EmailInfo").GetValue<string>("FromAddress"), "Contato Site litejob.com.br")
            };
            foreach(string address in email.To)
            {
                mail.To.Add(new MailAddress(address));
            }

            mail.Subject = email.Subject;
            mail.Body = email.Body;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(_config.GetSection("EmailInfo").GetValue<string>("Host"), int.Parse(_config.GetSection("EmailInfo").GetValue<string>("Port"))))
            {
                smtp.Credentials = new NetworkCredential(_config.GetSection("EmailInfo").GetValue<string>("FromAddress"), _config.GetSection("EmailInfo").GetValue<string>("Password"));
                smtp.EnableSsl = false;
                smtp.Send(mail);
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
