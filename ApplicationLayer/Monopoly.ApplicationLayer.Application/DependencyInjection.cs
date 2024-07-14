using Microsoft.Extensions.DependencyInjection;
using Monopoly.ApplicationLayer.Application.Common;

namespace Monopoly.ApplicationLayer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyApplication(this IServiceCollection services)
    {
        // 依賴注入 Use Cases
        return services.AddUseCases();
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var usecaseType = typeof(Usecase<,>);
        var usecaseType2 = typeof(Usecase<>);

        var types = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, BaseType.IsGenericType: true})
            .Where(t => t.BaseType?.GetGenericTypeDefinition() == usecaseType || t.BaseType?.GetGenericTypeDefinition() == usecaseType2);
        foreach (var type in types)
        {
            services.AddTransient(type);
        }

        return services;
    }
}