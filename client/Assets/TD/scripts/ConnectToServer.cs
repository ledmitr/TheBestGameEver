using UnityEngine;
using System.Net.Sockets;
using System;
using Assets.TD.scripts;
using Newtonsoft.Json;

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
        _client = new TcpClient(SaveVar.Host, SaveVar.Port);
        // "Привязываем" сокет к нашему экземпляру соединения с сервером.
        _socket = _client.Client;
        if (_socket.Connected)
        {
            // Получаем поток для чтения и записи данных.
            _stream = _client.GetStream();

            // Производим сериализацию Json пакета.
            Head_ReqToServer_HandShake handshake = new Head_ReqToServer_HandShake()
            {
                action = Actions.HandShake,
                content = new Head_ReqToServer_HandShake.Content()
                {
                    user_id = SaveVar.Player_id,
                    secret_key = SaveVar.Key
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
                byte[] buffer = new Byte[256];
                Int32 bytes = _stream.Read(buffer, 0, buffer.Length);
                // Преобразуем в строку.
                string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
                if (responseData.Contains(EndJsonStr))
                {
                    string[] splitData = responseData.Split(EndJson);
                    responseData = splitData[0];
                }
                else Debug.Log("Плохой пакет: " + responseData);
                // Происзводим десериализацию.
                Head_RespFromServer_HandShake respFromServer = JsonConvert.DeserializeObject<Head_RespFromServer_HandShake>(responseData);
                // Если поле action равно login, то используем login_resp, иначе используем другой класс десериализации.
                if (respFromServer.action == Actions.HandShake)
                {
                    _nameServer = respFromServer.content.server_name;
                    _idGame = respFromServer.content.game_id;
                    _msgFromServer = respFromServer.content.message;
                    Debug.Log(_nameServer);
                    Debug.Log(_idGame);
                    Debug.Log(_msgFromServer);
                }
            }
        }
	}

    public void SendMessageToServer(object objectToSend)
    {
        if (_socket.Connected && _stream.CanWrite)
        {
            var serializedObject = JsonConvert.SerializeObject(objectToSend);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(serializedObject);
            _stream.Write(data, 0, data.Length);
        }
    }
}
