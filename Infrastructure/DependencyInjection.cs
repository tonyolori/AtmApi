using Infrastructure.Data;
using Infrastructure.Database;
using Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDataContext, DataContext>();


        return services;

    }
}
