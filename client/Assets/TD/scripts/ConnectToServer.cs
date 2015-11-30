using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using Newtonsoft.Json;

public class ConnectToServer : MonoBehaviour {
    // Экземпляр класса TCP Client.
    TcpClient client = null;
    // Экземпляр класса Socket.
    Socket socket = null;
    public NetworkStream stream = null;
    string NameServer = "";
    int id_game = 0;
    string msg_from_server = "";
    public string END_JSON = "!end";
    public char End_json = '!';

	// Use this for initialization
	void Start () {
        // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
        client = new TcpClient(SaveVar.Host, SaveVar.Port);
        // "Привязываем" сокет к нашему экземпляру соединения с сервером.
        socket = client.Client;
        if (socket.Connected == true)
        {
            // Получаем поток для чтения и записи данных.
            stream = client.GetStream();

            // Производим сериализацию Json пакета.
            Head_ReqToServer_HandShake handshake = new Head_ReqToServer_HandShake()
            {
                action = "handshake",
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
            stream.Write(data, 0, data.Length);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (socket.Connected) 
        {
            if (stream.DataAvailable == true)
            {
                byte[] buffer = new Byte[256];
                Int32 bytes = stream.Read(buffer, 0, buffer.Length);
                // Преобразуем в строку.
                string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
                if (responseData.Contains(END_JSON) == true)
                {
                    string[] splitData = responseData.Split(End_json);
                    responseData = splitData[0];
                }
                else Debug.Log("Плохой пакет: " + responseData);
                // Происзводим десериализацию.
                Head_RespFromServer_HandShake resp_from_server = JsonConvert.DeserializeObject<Head_RespFromServer_HandShake>(responseData);
                // Если поле action равно login, то используем login_resp, иначе используем другой класс десериализации.
                if (resp_from_server.action == "handshake")
                {
                    NameServer = resp_from_server.content.server_name;
                    id_game = resp_from_server.content.game_id;
                    msg_from_server = resp_from_server.content.message;
                    Debug.Log(NameServer);
                    Debug.Log(id_game);
                    Debug.Log(msg_from_server);
                }
            }
        }
	}
}
