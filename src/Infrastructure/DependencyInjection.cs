using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISecretHasherService, SecretHasherService>();

        services.AddSingleton<IEmailService>(provider =>
        {
            return new EmailService(configuration);
        });




        return services;

    }
}
