using Application.Results;

namespace Application.Interfaces.Services;

public interface IEmailService
{
    
    Task<bool>SendEmail(string to,string subject,string body);
}
