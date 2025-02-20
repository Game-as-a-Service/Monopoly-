﻿using Microsoft.AspNetCore.SignalR;
using Monopoly.DomainLayer.Domain.Events;
using Monopoly.InterfaceAdapterLayer.Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Monopoly.InterfaceAdapterLayer.Server.Hubs.Monopoly.EventHandlers;

public class ChessMovedEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) : MonopolyEventHandlerBase<ChessMovedEvent>
{
    protected override Task HandleSpecificEvent(ChessMovedEvent e)
    {
        return hubContext.Clients.All.ChessMovedEvent(new ChessMovedEventArgs
        {
            PlayerId = e.PlayerId,
            BlockId = e.BlockId,
            Direction = e.Direction,
            RemainingSteps = e.RemainingSteps
        });
    }
}