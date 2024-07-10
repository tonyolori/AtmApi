using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddScoped<IDataContext, ApplicationDbContext>();
        services.AddScoped<IAuthHelper, AuthHelper>();
        services.AddScoped<ISecretHasher, SecretHasher>();

        return services;

    }
}
