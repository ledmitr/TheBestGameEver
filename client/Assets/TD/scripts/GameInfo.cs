using System.Collections.Generic;
using Assets.TD.scripts.Enums;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Хранит основную информацию об игре.
    /// </summary>
    public static class GameInfo
    {
        public static int Port;
        public static string Host;
        public static string ServerName { get; set; }
        public static int GameId { get; set; }

        public static int PlayerId;
        public static string Key;

        public static PlayerRole Role;
        public static GameState GameState { get; set; }

        public static readonly Queue<string> ServerMessages = new Queue<string>();

        public static readonly GameMap Map = new GameMap();
    }
}