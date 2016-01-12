using System.Collections.Generic;
using Assets.TD.scripts.Enums;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Хранит основную информацию об игре.
    /// </summary>
    public static class GameInfo
    {
        public static int Port { get; set; }
        public static string Host { get; set; }
        public static string ServerName { get; set; }
        public static int GameId { get; set; }

        public static int PlayerId { get; set; }
        public static string Key { get; set; }

        public static PlayerRole Role { get; set; }
        public static GameState GameState { get; set; }

        public static readonly Queue<string> ServerMessages = new Queue<string>();

        public static readonly GameMap Map = new GameMap();

        public static int CoinsAmount { get; set; }

        //todo: finish
        /*public static void Clean()
        {
            
        }*/
    }
}