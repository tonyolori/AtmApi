using Application.Common.Models;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Slack;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<Result>();


        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Slack(configuration.GetRequiredSection("ConnectionStrings:Slack").Get<string>())
            .WriteTo.Console()
            .CreateLogger();
        //services.AddScoped(provider =>
        //{
        //    var configuration = provider.GetRequiredService<IConfiguration>();
        //    return new AuthHelper(configuration);
        //});

        return services;
    
    }
}

