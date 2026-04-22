using System.Net;
using System.Net.Mail;
using Application.Email;
using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging; 

namespace Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger) : IEmailService
{
    public async Task<bool> SendEmail(string to, string subject, string body)
    {
        
        if (string.IsNullOrWhiteSpace(to))
        {
            logger.LogWarning("Attempted to send email to an empty address.");
            return false;
        }

        try
        {
          
            using var smtpClient = new SmtpClient(options.Value.SmtpServer, options.Value.Port);
            smtpClient.Credentials = new NetworkCredential(options.Value.Email, options.Value.Password);
            smtpClient.EnableSsl = true;

           
            var mail = new MailMessage(options.Value.Email, to, subject, body);
            mail.IsBodyHtml = true;

            
            await smtpClient.SendMailAsync(mail);
            logger.LogInformation("Email successfully sent to {Recipient} with subject: {Subject}", to, subject);
            
            return true;
        }
        catch (SmtpException smtpEx)
        {
            logger.LogError(smtpEx, "SMTP error occurred while sending email to {Recipient}", to);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while sending email to {Recipient}", to);
            return false;
        }
    }
}