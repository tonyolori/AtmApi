using Microsoft.Extensions.DependencyInjection;

namespace Contracts;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;

    }
}
