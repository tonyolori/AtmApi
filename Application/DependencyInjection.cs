using Authentication.Helpers;
using Authentication.Logic;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        
        services.AddValidatorsFromAssembly(assembly);


        ///
        services.AddScoped<UniqueAccountGenerator>();
        services.AddScoped<TransactionManager>();
        services.AddScoped(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            return new AuthHelper(configuration);
        });
        return services;
    
    }
}

