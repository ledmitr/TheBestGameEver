using System;
using System.Net.Sockets;
using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class ConnectToServer : MonoBehaviour {
        // Экземпляр класса TCP Client.
        private TcpClient _client = null;
        // Экземпляр класса Socket.
        private Socket _socket = null;
        private NetworkStream _stream = null;
        private string _nameServer = "";
        private int _idGame = 0;
        private string _msgFromServer = "";
        private const string EndJsonStr = "!end";
        private const char EndJson = '!';

        // Use this for initialization
        void Start () {
            // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
            _client = new TcpClient(GameInfo.Host, GameInfo.Port);
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
            if (_socket.Connected) 
            {
                if (_stream.DataAvailable)
                {
                    byte[] buffer = new byte[256];
                    Int32 bytes = _stream.Read(buffer, 0, buffer.Length);
                    // Преобразуем в строку.
                    string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);

                    Debug.Log(String.Format("Readed {0} symbols: {1}", bytes, responseData));

                    if (responseData.Contains(EndJsonStr))
                    {
                        string[] splitData = responseData.Split(EndJson);
                        responseData = splitData[0];
                    }
                    else Debug.Log("Плохой пакет: " + responseData);

                    string messageAction = "";
                    try
                    {
                        var parsedObject = JObject.Parse(responseData);
                        messageAction = (parsedObject["action"]).ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        return;
                    }

                    switch (messageAction)
                    {
                        case Actions.HandShake:
                            ProcessHandShake(responseData);
                            break;
                        case Actions.PrepareToStart:
                            ProcessPrepareToStart(responseData);
                            break;
                        case Actions.GameToStart:
                            ProcessGameToStart(responseData);
                            break;
                        case Actions.StagePlanning:
                            ProcessStagePlanning(responseData);
                            break;
                        case Actions.StageSimulate:
                            ProcessStageSimulate(responseData);
                            break;
                        case Actions.StageFinish:
                            ProcessStageFinish(responseData);
                            break;
                    }
                    Debug.Log(GameInfo.GameState);
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
            _nameServer = respFromServer.content.server_name;
            _idGame = respFromServer.content.game_id;
            _msgFromServer = respFromServer.content.message;
            Debug.Log(_nameServer);
            Debug.Log(_idGame);
            Debug.Log(_msgFromServer);

            if (GameInfo.GameState == GameState.Connected)
                GameInfo.GameState = GameState.HandShakeDone;
        }

        private void SendMessageToServer(object objectToSend)
        {
            if (_socket.Connected && _stream.CanWrite)
            {
                var serializedObject = JsonConvert.SerializeObject(objectToSend);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(serializedObject);
                _stream.Write(data, 0, data.Length);
            }
        }

        public void SendAddUnitRequest(UnitType unitType, Vector3 targetTowerPosition)
        {
            var addTowerRequest = new AddUnitRequestToServer
            {
                action = Actions.AddUnit,
                content = new AddUnitRequestToServer.Content
                {
                    type_unit = (int)unitType,
                    position_x = (int)targetTowerPosition.x,
                    position_y = (int)targetTowerPosition.y
                }
            };
            SendMessageToServer(addTowerRequest);
        }
    }
}
