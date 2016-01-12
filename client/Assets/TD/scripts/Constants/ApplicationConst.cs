namespace Assets.TD.scripts.Constants
{
    /// <summary>
    /// Данные для приложения.
    /// </summary>
    public static class ApplicationConst
    {
        public const string ServerAddress = "23.251.139.4";
        public const int ServerPort = 2121;

        public const string MountainTag = "Mountain";
        public const string TowerTag = "Tower";
        public const string FortressTag = "Fortress";
        public const string SelectableTag = "Selectable";
        public const string TentTag = "Tent";
        public const string KnightTag = "Knight";
        public const string LandTag = "Land";
        public const string RoadTag = "Road";

        public const string CreateTowerHint = "\uf25a TAP ON THE LAND TO PLACE A TOWER";
        public const string CreateKnightHint = "\uf25a TAP ON A TENT WHERE KNIGHT APPEARS FROM";
        public const string GameFinishedStr = "GAME FINISHED";

        public const int StartCoinsAmount = 1000;
        public const int TowerCost = 300;
        public const int KnightCost = 100;

        public const string GameSearchHeading = "GAME SEARCH";
        public const string LoadingHeading = "LOADING";
        public const string ConnectingHeading = "CONNECTING";
        public const string ErrorHeading = "ERROR";
    }
}
