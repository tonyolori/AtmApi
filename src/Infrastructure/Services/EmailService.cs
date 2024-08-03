using Application.Common.Models;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services;
public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;
    private readonly string _password;
    private readonly string smtpServer;
    private readonly int port;

    public EmailService(IConfiguration configuration)
    {
        IConfigurationSection emailConfig = configuration.GetSection("EmailSender");
        _fromEmail = emailConfig["Email"];
        _password = emailConfig["Password"];
        smtpServer = emailConfig["smtpServer"];


        if (!int.TryParse(emailConfig["port"], out port))
        {
            throw new InvalidOperationException("Invalid port value in configuration");
        }

        _smtpClient = new SmtpClient(smtpServer, port)
        {
            Credentials = new NetworkCredential(_fromEmail, _password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };
    }

    public async Task<Result> SendEmailAsync(string toEmail, string FistName)
    {
        string subject = EmailTemplate.GetSubject(FistName, "Atm Application");
        string body = EmailTemplate.GetBody(FistName, "Atm Application");
        try
        {
            MailMessage mailMessage = new MailMessage(_fromEmail, toEmail, subject, body);
            using (SmtpClient smtpClient = new SmtpClient(_smtpClient.Host, _smtpClient.Port))
            {
                smtpClient.Credentials = _smtpClient.Credentials;
                smtpClient.EnableSsl = _smtpClient.EnableSsl;
                await smtpClient.SendMailAsync(mailMessage);
            }
            return Result.Success($"Registration Email successfully sent to {toEmail}");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to send email to {toEmail}", ex);
        }
    }
}
