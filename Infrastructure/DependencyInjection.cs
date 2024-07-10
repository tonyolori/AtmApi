using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddScoped<IDataContext, ApplicationDbContext>();

        return services;

    }
}
