using System.Reflection;
using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries.Interfaces;
using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using Monopoly.InterfaceAdapterLayer.Server.Presenters;
using Monopoly.InterfaceAdapterLayer.Server.Repositories;
using Monopoly.InterfaceAdapterLayer.Server.Repositories.Inquiries;

namespace Monopoly.InterfaceAdapterLayer.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyServer(this IServiceCollection services)
    {
        services.AddSingleton<IReadyRoomRepository, InMemoryReadyRoomRepository>();
        services.AddSingleton(typeof(FakeInMemoryDatabase<>), typeof(FakeInMemoryDatabase<>));
        services.AddSingleton<IRepository<MonopolyAggregate>, MonopolyRepository>()
            .AddSingleton<IEventBus<DomainEvent>, MonopolyEventBus>()
            .AddTransient(typeof(SignalrDefaultPresenter<>), typeof(SignalrDefaultPresenter<>))
            .AddTransient(typeof(DefaultPresenter<>), typeof(DefaultPresenter<>));
        services.AddInquiries();
        services.AddSignalREventHandlers();
        return services;
    }

    private static IServiceCollection AddSignalREventHandlers(this IServiceCollection services)
    {
        var handlers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                          && t.IsAssignableTo(typeof(IMonopolyEventHandler)))
            .ToList();
        foreach (var handler in handlers)
        {
            services.AddSingleton(typeof(IMonopolyEventHandler), handler);
        }
        return services;
    }
    
    private static void AddInquiries(this IServiceCollection services)
    {
        services.AddTransient<IGameExistenceInquiry, GameExistenceInquiry>();
    }
}