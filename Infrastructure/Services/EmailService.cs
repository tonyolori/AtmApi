using Application.Common.Models;
using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using Serilog;
using Application.Interfaces;

namespace Infrastructure.Services;
public class EmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;
    private readonly string _password;

    public EmailSender(string smtpServer, int port, string fromEmail, string password)
    {
        _fromEmail = fromEmail;
        _password = password;

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
            var mailMessage = new MailMessage(_fromEmail, toEmail, subject, body);
            using (var smtpClient = new SmtpClient(_smtpClient.Host, _smtpClient.Port))
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
