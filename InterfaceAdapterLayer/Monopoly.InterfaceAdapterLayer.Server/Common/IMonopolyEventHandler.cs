using Monopoly.DomainLayer.Common;

namespace Monopoly.InterfaceAdapterLayer.Server.Common;

internal interface IMonopolyEventHandler
{
    public Type EventType { get; }
    Task HandleAsync(DomainEvent e);
}