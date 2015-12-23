using System;
using System.IO;
using System.Net.Sockets;
using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Подключается к серверу и осуществляет взаимодействие с ним.
    /// </summary>
    public class ConnectToServer : MonoBehaviour {
        // Экземпляр класса TCP Client.
        private TcpClient _client = null;
        // Экземпляр класса Socket.
        private Socket _socket = null;
        private NetworkStream _stream = null;
        private const string EndJsonStr = "!end";

        // Use this for initialization
        void Start () {
            // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
            _client = new TcpClient(ApplicationConst.ServerAddress, GameInfo.Port);
            // "Привязываем" сокет к нашему экземпляру соединения с сервером.
            _socket = _client.Client;
            if (_socket.Connected)
            {
                GameInfo.GameState = GameState.Connected;
                // Получаем поток для чтения и записи данных.
                _stream = _client.GetStream();

                // Производим сериализацию Json пакета.
                Head_ReqToServer_HandShake handshake = new Head_ReqToServer_HandShake
                {
                    action = Actions.HandShake,
                    content = new Head_ReqToServer_HandShake.Content
                    {
                        user_id = GameInfo.PlayerId,
                        secret_key = GameInfo.Key
                    }
                };
                // С помощью JsonConvert производим сериализацию структуры выше.
                string json = JsonConvert.SerializeObject(handshake, Formatting.Indented)+"!end";
                // Переводим наше сообщение в ASCII, а затем в массив Byte.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                // Отправляем сообщение нашему серверу. 
                _stream.Write(data, 0, data.Length);
            }
        }
	
        // Update is called once per frame
        void Update () {
            if (_socket != null && _socket.Connected) 
            {
                if (_stream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    Int32 bytes = _stream.Read(buffer, 0, buffer.Length);
                    // Преобразуем в строку.
                    string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);

                    Debug.Log(String.Format("Readed {0} symbols: {1}", bytes, responseData));

                    using (var file = File.OpenWrite("serverOutput.txt"))
                    {
                        file.Write(buffer, 0, bytes);
                    }

                    string[] messages;
                    if (responseData.Contains(EndJsonStr))
                    {
                        messages = responseData.Split(new[] {EndJsonStr}, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        Debug.Log("Плохой пакет: " + responseData);
                        return;
                    }

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
                            //return;
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
                }
            }
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
                GameInfo.GameState = GameState.Playing;
        }

        private void ProcessStagePlanning(string responseData)
        {
            var stagePlanningMsg = JsonConvert.DeserializeObject<StagePlanning>(responseData);
            Debug.Log(stagePlanningMsg.content.message);
            Debug.Log(stagePlanningMsg.content.time);
            if (GameInfo.GameState == GameState.Preparing)
                GameInfo.GameState = GameState.Planning;
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
            GameInfo.Role = (PlayerRole) prepareToStartMsg.content.you_role;
            Debug.Log("Server sent you a role: " + (GameInfo.Role == PlayerRole.Attacker ? "Attacker" : "Defender"));
            //todo: parse map, save it.

            var message = new PrepareToStartResponse
            {
                action = Actions.PrepareToStart,
                code = 0,
                content = "RECEIVED"
            };
            SendMessageToServer(message);
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

        private void SendMessageToServer(object objectToSend)
        {
            if (_socket.Connected && _stream.CanWrite)
            {
                var serializedObject = JsonConvert.SerializeObject(objectToSend);
                Debug.Log("Message to server: " + serializedObject);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(serializedObject);
                _stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Запрашивает у сервера создание юнита.
        /// </summary>
        /// <param name="unitType">Тип юнита (башня или рыцарь).</param>
        /// <param name="position">Позиция юнита.</param>
        public void SendAddUnitRequest(UnitType unitType, Vector3 position)
        {
            var addTowerRequest = new AddUnitRequestToServer
            {
                action = Actions.AddUnit,
                content = new AddUnitRequestToServer.Content
                {
                    type_unit = (int)unitType,
                    position_x = (int)position.x,
                    position_y = (int)position.y
                }
            };
            SendMessageToServer(addTowerRequest);
            //return true;
        }

        private void ProcessActualData(string message)
        {
            Debug.Log("Server from message:" + message);
            var actualData = JsonConvert.DeserializeObject<ActualData>(message);
            foreach (var actualDataContentItem in actualData.content)
            {
                foreach (var actualDataUnit in actualDataContentItem.units)
                {
                    UnitManager.UpdateUnit(actualDataUnit);
                }
            }
        }

        public UnitManager UnitManager;
    }
}
