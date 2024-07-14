using System.Reflection;
using Monopoly.ApplicationLayer.Application.Common;
using Monopoly.ApplicationLayer.Application.MonopolyUsecases.Queries.Interfaces;
using Monopoly.ApplicationLayer.Application.ReadyRoomUsecases.Commands;
using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.Domain;
using Monopoly.DomainLayer.ReadyRoom;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using Monopoly.InterfaceAdapterLayer.Server.Presenters;
using Monopoly.InterfaceAdapterLayer.Server.Repositories;
using Monopoly.InterfaceAdapterLayer.Server.Repositories.Inquiries;

namespace Monopoly.InterfaceAdapterLayer.Server;

public static class DependencyInjection
{
    public static void AddMonopolyServer(this IServiceCollection services)
    {
        services.AddSingleton(typeof(FakeInMemoryDatabase<>), typeof(FakeInMemoryDatabase<>));
        services.AddTransient<IRepository<MonopolyAggregate>, MonopolyRepository>()
            .AddTransient<IRepository<ReadyRoomAggregate>, ReadyRoomRepository>()
            .AddSingleton<IEventBus<DomainEvent>, MonopolyEventBus>()
            .AddTransient(typeof(DefaultPresenter<>), typeof(DefaultPresenter<>));
        services.AddInquiries();
        services.AddSignalREventHandlers();
    }

    private static void AddSignalREventHandlers(this IServiceCollection services)
    {
        var handlers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.IsAssignableTo(typeof(IMonopolyEventHandler)))
            .ToList();
        foreach (var handler in handlers)
        {
            services.AddSingleton(typeof(IMonopolyEventHandler), handler);
        }
    }

    private static void AddInquiries(this IServiceCollection services)
    {
        services.AddTransient<IGameExistenceInquiry, GameExistenceInquiry>();
    }
}