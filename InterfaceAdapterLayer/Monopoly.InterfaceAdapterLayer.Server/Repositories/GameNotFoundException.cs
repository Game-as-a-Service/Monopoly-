﻿namespace Monopoly.InterfaceAdapterLayer.Server.Repositories
{
    [Serializable]
    internal class GameNotFoundException : Exception
    {
        public GameNotFoundException(string? id) : base($"Game id {{{id}}} not found!")
        {
        }
    }
}