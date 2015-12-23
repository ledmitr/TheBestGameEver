namespace Assets.TD.scripts.Constants
{
    /// <summary>
    /// Имена типов сообщений от сервера.
    /// </summary>
    public static class Actions
    {
        public const string HandShake = "handshake";
        public const string ConnectToGame = "connect_to_game";
        public const string Login = "login";
        public const string Message = "message";
        public const string PrepareToStart = "prepare_to_start";
        public const string GameToStart = "game_to_start";
        public const string StagePlanning = "stage_planning";
        public const string StageSimulate = "stage_simulate";
        public const string StageFinish = "stage_finish";
        public const string AddUnit = "add_unit";
        public const string ActualData = "actual_data";
    }
}