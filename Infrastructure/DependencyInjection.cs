using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IDataContext, ApplicationDbContext>();
        services.AddScoped<IAuthHelper, AuthHelper>();
        services.AddScoped<ISecretHasher, SecretHasher>();

        services.AddSingleton<IEmailSender>(provider =>
        {
            var smtpServer = "smtp.gmail.com";
            var port = 587;
            var fromEmail = configuration.GetRequiredSection("EmailSettings:Email").Get<string>();
            var password = configuration.GetRequiredSection("EmailSettings:Password").Get<string>();

            return new EmailSender(smtpServer, port, fromEmail, password);
        });




        return services;

    }
}
