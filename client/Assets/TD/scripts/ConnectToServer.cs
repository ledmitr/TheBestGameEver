using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using Assets.TD.scripts.Utils.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Подключается к серверу и осуществляет взаимодействие с ним.
    /// </summary>
    public class ConnectToServer : MonoBehaviour {
        private const string EndStr = "!end";
        private const string EndJsonStr = "}!end";

        // Экземпляр класса TCP Client.
        private TcpClient _client = null;
        // Экземпляр класса Socket.
        private Socket _socket = null;
        private NetworkStream _stream = null;

        private void Start() {
            // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
            _client = new TcpClient(ApplicationConst.ServerAddress, GameInfo.Port);
            // "Привязываем" сокет к нашему экземпляру соединения с сервером.
            _socket = _client.Client;
            if (_socket.Connected)
            {
                GameInfo.GameState = GameState.Connected;
                // Получаем поток для чтения и записи данных.
                _stream = _client.GetStream();

                //Debug.Log("server connected.\n handshake message recieved");

                // Производим сериализацию Json пакета.
                Head_ReqToServer_HandShake handshake = new Head_ReqToServer_HandShake {
                    action = Actions.HandShake,
                    content = new Head_ReqToServer_HandShake.Content
                    {
                        user_id = GameInfo.PlayerId,
                        secret_key = GameInfo.Key
                    }
                };
                SendMessageToServer(handshake);
                //Debug.Log("handshake message sent");
            }
        }

        private string _notFinishedMessage = null;
	
        // Update is called once per frame
        private void Update()
        {
            if (_socket != null && _socket.Connected && _stream.DataAvailable)
            {
                //Debug.Log("start reading message");
                var response = ReadFromStream(_stream);
                Debug.Log("FROM server: " + response);

                if (!string.IsNullOrEmpty(_notFinishedMessage))
                {
                    response = _notFinishedMessage + response;
                    _notFinishedMessage = string.Empty;
                }

                if (!response.Contains(EndJsonStr))
                {
                    _notFinishedMessage = response;
                }
                else
                {
                    var messages = response.Split(new[] {EndStr}, StringSplitOptions.RemoveEmptyEntries);
                    if (messages.Length > 1 && !messages.Last().EndsWith(EndJsonStr))
                    {
                        _notFinishedMessage = messages.Last();
                        messages = messages.Take(messages.Length - 1).ToArray();
                    }
                    GameInfo.ServerMessages.AddRange(messages);
                }
            }
        }

        // unused
        private static string ReadFromStream(Stream stream)
        {
            byte[] buffer = new byte[1024];
            Int32 bytes = stream.Read(buffer, 0, buffer.Length);
            // Преобразуем в строку.
            string responseData = Encoding.ASCII.GetString(buffer, 0, bytes);

            //Debug.Log(String.Format("Read {0} symbols: {1}", bytes, responseData));
            return responseData;
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <exception cref="System.IO.IOException">Thrown if any of the underlying IO calls fail</exception>
        private static string ReadFully(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                byte[] data = new byte[1024];
                int numBytesRead;
                while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
                {
                    ms.Write(data, 0, numBytesRead);
                    //Debug.Log(numBytesRead + " bytes read from server");
                }
                var str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                return str;
            }
        }

        public void SendMessageToServer(object objectToSend)
        {
            if (_socket.Connected && _stream.CanWrite)
            {
                var serializedObject = JsonConvert.SerializeObject(objectToSend) + EndStr;
                Debug.Log("TO server: " + serializedObject);
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
        }
    }
}
