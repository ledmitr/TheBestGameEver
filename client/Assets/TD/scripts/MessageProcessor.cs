using System;
using System.Linq;
using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class MessageProcessor : MonoBehaviour
    {
        public ConnectToServer ConnectToServer = null;
        public UnitManager UnitManager = null;
        public UIManager UIManager = null;

        private void Start()
        {
            Debug.Assert(ConnectToServer != null);
            Debug.Assert(UnitManager != null);
            Debug.Assert(UIManager != null);
        }

        private void Update()
        {   
            var messages = GameInfo.ServerMessages.ToList();
            if (!messages.Any())
                return;
            GameInfo.ServerMessages.Clear();
            Debug.Log(string.Format("Count messages to process: {0}", messages.Count));
            foreach (var message in messages)
            {
                string messageAction = "";
                try
                {
                    var parsedObject = JObject.Parse(message);
                    messageAction = (string)parsedObject["action"];
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                Debug.Log(string.Format("processing action: {0}", messageAction));
                switch (messageAction)
                {
                    case Actions.HandShake:
                        ProcessHandShake(message);
                        break;
                    case Actions.PrepareToStart:
                        ProcessPrepareToStart(message);
                        break;
                    case Actions.GameToStart:
                        ProcessGameToStart(message);
                        break;
                    case Actions.StagePlanning:
                        ProcessStagePlanning(message);
                        break;
                    case Actions.StageSimulate:
                        ProcessStageSimulate(message);
                        break;
                    case Actions.StageFinish:
                        ProcessStageFinish(message);
                        break;
                    case Actions.ActualData:
                        ProcessActualData(message);
                        break;
                    case Actions.Error:
                        ProcessError();
                        break;
                    case "":
                        Debug.LogError("Incorrect message action");
                        break;
                }
                Debug.Log(GameInfo.GameState);
            }
        }

        private void ProcessHandShake(string responseData)
        {
            // Производим десериализацию.
            Head_RespFromServer_HandShake respFromServer =
                JsonConvert.DeserializeObject<Head_RespFromServer_HandShake>(responseData);
            GameInfo.ServerName = respFromServer.content.server_name;
            GameInfo.GameId = respFromServer.content.game_id;
            Debug.Log(respFromServer);
            if (GameInfo.GameState == GameState.Connected)
                GameInfo.GameState = GameState.HandShakeDone;

            UIManager.SetLoadingDetails("HANDSHAKE DONE");
        }

        private void ProcessGameToStart(string responseData)
        {
            var gameToStartMsg = JsonConvert.DeserializeObject<GameToStart>(responseData);
            Debug.Log(gameToStartMsg.content);
            GameInfo.GameState = GameState.Preparing;
            UIManager.SetLoadingDetails("GAME IS PREPARING");
        }

        private void ProcessPrepareToStart(string responseData)
        {
            var prepareToStartMsg = JsonConvert.DeserializeObject<PrepareToStart>(responseData);

            GameInfo.Role = (PlayerRole)prepareToStartMsg.content.you_role;
            Debug.Log(string.Format("Server sent you a role: {0}", (GameInfo.Role == PlayerRole.Attacker ? "Attacker" : "Defender")));

            if (prepareToStartMsg.content.map_height > 0 && prepareToStartMsg.content.map_width > 0)
            {
                GameInfo.Map.Map = prepareToStartMsg.content.map;
                GameInfo.Map.Height = prepareToStartMsg.content.map_height;
                GameInfo.Map.Width = prepareToStartMsg.content.map_width;
                GameInfo.CoinsAmount = ApplicationConst.StartCoinsAmount;

                UnitManager.InitCubeArray(GameInfo.Map.Height, GameInfo.Map.Width);
                UIManager.SetLoadingDetails("MAP CREATION");
                StartCoroutine(UnitManager.InstantinateMap(GameInfo.Map));
            }
            else
            {
                Debug.LogError("Map dimensions recieved from server are not positive");
            }

            var message = new PrepareToStartResponse
            {
                action = Actions.PrepareToStart,
                code = 0,
                content = "RECEIVED"
            };
            ConnectToServer.SendMessageToServer(message);
        }

        private void ProcessStagePlanning(string responseData)
        {
            var stagePlanningMsg = JsonConvert.DeserializeObject<StagePlanning>(responseData);
            Debug.Log(stagePlanningMsg.content.message);
            Debug.Log(stagePlanningMsg.content.time);
            UIManager.SetUiActive();
            UIManager.UpdateGoToMainUnitButton(GameInfo.Role);
            UIManager.StatBar.SetActive(false);
            UIManager.PreparingStartBar.SetActive(true);
            UIManager.SetPreparingTime(stagePlanningMsg.content.time);
            GameInfo.GameState = GameState.Planning;
        }

        private void ProcessStageSimulate(string responseData)
        {
            var stageSimulateMsg = JsonConvert.DeserializeObject<StageSimulate>(responseData);
            Debug.Log(stageSimulateMsg.content);
            UIManager.PreparingStartBar.SetActive(false);
            UIManager.StatBar.SetActive(true);
            GameInfo.GameState = GameState.Playing;
        }

        private bool _isCoinsActualized = false;
        private void ProcessActualData(string message)
        {
            Debug.Log(string.Format("Server from message:{0}", message));
            var actualData = JsonConvert.DeserializeObject<ActualData>(message);
            GameInfo.KilledAmount = actualData.content[(int)PlayerRole.Attacker].is_dead;
            UnitManager.UpdateUnits(actualData);

            if (!_isCoinsActualized)
            {
                if (GameInfo.Role == PlayerRole.Defender)
                    GameInfo.CoinsAmount = ApplicationConst.StartCoinsAmount - UnitManager.GetTowers().Count * ApplicationConst.TowerCost;
                else
                    GameInfo.CoinsAmount = ApplicationConst.StartCoinsAmount - UnitManager.GetKnights().Count * ApplicationConst.KnightCost;
                _isCoinsActualized = true;
            }
        }

        private void ProcessStageFinish(string responseData)
        {
            var stageFinishMsg = JsonConvert.DeserializeObject<StageFinish>(responseData);
            Debug.Log(stageFinishMsg.content);
            GameInfo.GameState = GameState.Finished;
            UIManager.ProcessFinish();
            ConnectToServer.FinishGame();
        }

        private void ProcessError()
        {

        }
    }
}