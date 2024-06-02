using Monopoly.DomainLayer.Common;
using Monopoly.DomainLayer.ReadyRoom.Common;

namespace Monopoly.DomainLayer.ReadyRoom.Events;

public sealed record PlayerRoleSelectedEvent(string PlayerId, string RoleId) : DomainEvent;