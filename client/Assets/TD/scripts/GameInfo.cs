using Assets.TD.scripts.Enums;

namespace Assets.TD.scripts
{
    public static class GameInfo
    {
        public static int Port;
        public static string Host;

        public static int PlayerId;
        public static string Key;

        public static PlayerRole Role;

        public static GameState GameState { get; set; }
    }
}