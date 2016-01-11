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
            var messages = GameInfo.ServerMessages;
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
                }
                Debug.Log(GameInfo.GameState);
            }
            GameInfo.ServerMessages.Clear();
        }

        private void ProcessStageFinish(string responseData)
        {
            var stageFinishMsg = JsonConvert.DeserializeObject<StageFinish>(responseData);
            Debug.Log(stageFinishMsg.content);
            if (GameInfo.GameState == GameState.Playing)
                GameInfo.GameState = GameState.Finished;
        }

        private void ProcessStageSimulate(string responseData)
        {
            var stageSimulateMsg = JsonConvert.DeserializeObject<StageSimulate>(responseData);
            Debug.Log(stageSimulateMsg.content);
            if (GameInfo.GameState == GameState.Planning)
            {
                UIManager.StatBar.SetActive(true);
                UIManager.PreparingStartBar.SetActive(false);
                GameInfo.GameState = GameState.Playing;
            }
        }

        private void ProcessStagePlanning(string responseData)
        {
            var stagePlanningMsg = JsonConvert.DeserializeObject<StagePlanning>(responseData);
            Debug.Log(stagePlanningMsg.content.message);
            Debug.Log(stagePlanningMsg.content.time);
            if (GameInfo.GameState == GameState.Preparing)
            {
                UIManager.StatBar.SetActive(false);
                UIManager.PreparingStartBar.SetActive(true);
                UIManager.SetPreparingTime(stagePlanningMsg.content.time);
                GameInfo.GameState = GameState.Planning;
            }
        }

        private void ProcessGameToStart(string responseData)
        {
            var gameToStartMsg = JsonConvert.DeserializeObject<GameToStart>(responseData);
            Debug.Log(gameToStartMsg.content);
            if (GameInfo.GameState == GameState.HandShakeDone)
                GameInfo.GameState = GameState.Preparing;
        }

        private void ProcessPrepareToStart(string responseData)
        {
            var prepareToStartMsg = JsonConvert.DeserializeObject<PrepareToStart>(responseData);
            GameInfo.Role = (PlayerRole)prepareToStartMsg.content.you_role;
            Debug.Log("Server sent you a role: " + (GameInfo.Role == PlayerRole.Attacker ? "Attacker" : "Defender"));

            Debug.Log("server sent you a map");
            Debug.Log(prepareToStartMsg);
            
            if (prepareToStartMsg.content.map_height > 0 && prepareToStartMsg.content.map_width > 0)
            {
                GameInfo.Map.Map = prepareToStartMsg.content.map;
                GameInfo.Map.Height = prepareToStartMsg.content.map_height;
                GameInfo.Map.Width = prepareToStartMsg.content.map_width;
                UnitManager.InitCubeArray(GameInfo.Map.Height, GameInfo.Map.Width);
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
        }

        private void ProcessActualData(string message)
        {
            Debug.Log("Server from message:" + message);
            var actualData = JsonConvert.DeserializeObject<ActualData>(message);
            UnitManager.UpdateUnits(actualData);
        }
    }
}