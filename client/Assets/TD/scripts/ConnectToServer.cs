using UnityEngine;
using System.Net.Sockets;
using System;
using Assets.TD.scripts;
using Newtonsoft.Json;

public class ConnectToServer : MonoBehaviour {
    // Экземпляр класса TCP Client.
    TcpClient _client = null;
    // Экземпляр класса Socket.
    Socket _socket = null;
    NetworkStream _stream = null;
    string _nameServer = "";
    int _idGame = 0;
    string _msgFromServer = "";

	// Use this for initialization
	void Start () {
        // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
        _client = new TcpClient(SaveVar.Host, SaveVar.Port);
        // "Привязываем" сокет к нашему экземпляру соединения с сервером.
        _socket = _client.Client;
        if (_socket.Connected)
        {
            // Получаем поток для чтения и записи данных.
            _stream = _client.GetStream();

            // Производим сериализацию Json пакета.
            Head_ReqToServer_HandShake handshake = new Head_ReqToServer_HandShake
            {
                action = Actions.HandShake,
                content = new Head_ReqToServer_HandShake.Content
                {
                    user_id = SaveVar.Player_id,
                    secret_key = SaveVar.Key
                }
            };
            // С помощью JsonConvert производим сериализацию структуры выше.
            string json = JsonConvert.SerializeObject(handshake, Formatting.Indented);
            // Переводим наше сообщение в ASCII, а затем в массив Byte.
            byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
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
                // Происзводим десериализацию.
                Head_RespFromServer_HandShake responseFromServer = JsonConvert.DeserializeObject<Head_RespFromServer_HandShake>(responseData);
                // Если поле action равно login, то используем login_resp, иначе используем другой класс десериализации.
                if (responseFromServer.action == Actions.HandShake)
                {
                    _nameServer = responseFromServer.content.server_name;
                    _idGame = responseFromServer.content.game_id;
                    _msgFromServer = responseFromServer.content.message;
                    Debug.Log(_nameServer);
                    Debug.Log(_idGame);
                    Debug.Log(_msgFromServer);
                }
            }
        }
	}
}
