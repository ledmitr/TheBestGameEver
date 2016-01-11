﻿namespace Assets.TD.scripts.Constants
{
    /// <summary>
    /// Данные для приложения.
    /// </summary>
    public static class ApplicationConst
    {
        public const string ServerAddress = "23.251.139.4";
        public const int ServerPort = 2121;

        public const string FieldTag = "Field";
        public const string TowerTag = "Tower";
        public const string FortressTag = "Fortress";
        public const string SelectableTag = "Selectable";
        public const string TentTag = "Tent";
        public const string KnightTag = "Knight";
        public const string LandTag = "Land";
        public const string RoadTag = "Road";

        public const string CreateTowerHint = "\uf25a TAP ON THE LAND TO PLACE A TOWER";
        public const string CreateKnightHint = "\uf25a TAP ON A TENT WHERE KNIGHT APPEARS FROM";

        public const int StartHealth = 100;
        public const int DamageAmount = 10;

        public const string GameSearchHeading = "GAME SEARCH";
        public const string LoadingHeading = "LOADING";
        public const string ConnectingHeading = "CONNECTING";
        public const string ErrorHeading = "ERROR";
    }
}
