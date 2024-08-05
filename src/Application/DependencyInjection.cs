using Application.Common.Models;
using Application.Users.Commands;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Slack;
using System.Reflection;
using Application.Users;
namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<Result>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Slack(configuration.GetRequiredSection("ConnectionStrings:Slack").Get<string>())
            .WriteTo.Console()
            .CreateLogger();

        return services;

    }
}

