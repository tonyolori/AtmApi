using Application.Common.Models;

namespace Application.Interfaces;
public interface IEmailService
{
    Task<Result> SendEmailAsync(string toEmail, string FirstName);
}
