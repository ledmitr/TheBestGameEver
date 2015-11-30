using UnityEngine;
using Assets.TD.scripts;
using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using Newtonsoft.Json;
// Класс главного меню.
public class MainMenu : MonoBehaviour {

    public GameObject MainMenuItems;
    public GameObject SettingsMenuItems;
    public void Start()
    {
        SettingsMenuItems.SetActive(false);
        LoadingSplashScreen.SetActive(false);
        GameSearchSplashScreen.SetActive(false);
        ConnectingSplashScreen.SetActive(false);
        QualitySettings.SetQualityLevel(0);
    }

    public void Update() {
        if(flag_start==true)
        {
            if (socket.Connected == true)
            {
                if (stream.DataAvailable == true)
                {
                    while (socket.Connected == true)
                    {
                        // Буфер для хранения принятого массива bytes.
                        Byte[] buffer = new Byte[256];
                        // Читаем пакет ответа сервера. 
                        Int32 bytes = stream.Read(buffer, 0, buffer.Length);
                        // Преобразуем в строку.
                        string responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);
                        // !!! возможно здесь таится ошибка
                        if (responseData.Contains(END_JSON)==true)
                        {
                            string [] splitData = responseData.Split(End_json);
                            responseData = splitData[0];
                        }
                        else Debug.Log("Плохой пакет: " + responseData);
                        // Происзводим десериализацию.
                        Head_RespFromServer_Login login_resp = JsonConvert.DeserializeObject<Head_RespFromServer_Login>(responseData);
                        // Если поле action равно login, то используем login_resp, иначе используем другой класс десериализации.
                        if (login_resp.action == "login")
                        {
                            // Делаем надпись GameSearch активной, а Connecting убираем.
                            ConnectingSplashScreen.SetActive(false);
                            GameSearchSplashScreen.SetActive(true);
                            SaveVar.Player_id = login_resp.content.your_id;
                            msg_from_server = login_resp.content.message;
                            Debug.Log(SaveVar.Player_id);
                            Debug.Log(msg_from_server);
                        };
                        if (login_resp.action == "connect_to_game")
                        {
                            Head_ReqFromServer_ConnectToGame connect_resp = JsonConvert.DeserializeObject<Head_ReqFromServer_ConnectToGame>(responseData);
                            SaveVar.Port = connect_resp.content.port;
                            SaveVar.Host = connect_resp.content.host;
                            SaveVar.Key = connect_resp.content.secret_key;
                            Debug.Log(SaveVar.Port);
                            Debug.Log(SaveVar.Host);
                            Debug.Log(SaveVar.Key);
                            // Закрывем соединение и поток.
                            socket.Close();
                            client.Close();
                            stream.Close();
                            // Делаем надпись GameSearch неактивной, а Loading включаем.
                            GameSearchSplashScreen.SetActive(false);
                            LoadingSplashScreen.SetActive(true);
                            Application.LoadLevel(start);
                        };

                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public GameObject ConnectingSplashScreen;
    public GameObject GameSearchSplashScreen;
    public GameObject LoadingSplashScreen;
    // Логическая переменная, отвечающая за нажатие кнопки Start.
	public bool is_start;
	// Строковая переменная, хранящая название сцены Start.
	public string start;
	// Логическая переменная, отвечающая за нажатие кнопки Statistics.
	public bool is_statistics;
	// Строковая переменная, хранящая название сцены Statistics.
	public string statistics;
	// Логическая переменная, отвечающая за нажатие кнопки Settings.
	public bool is_settings;
	// Логическая переменная, отвечающая за нажатие кнопки Exit.
	public bool is_exit;
    // Строковая переменная, содержащая IP-адрес сервера.
    private string server = "104.155.17.158";
    // Целочисленная переменная, содержащая номер порта.
    Int32 port = 2121;
    // Пустой экземпляр класса TCP Client.
    TcpClient client = null;
    // Пустой экземпляр класса Socket.
    Socket socket = null;
    // Пустой экземпляр класса NetworkStream.
    NetworkStream stream = null;
    // Полученное сообщение от сервера
    string msg_from_server = "";
    bool flag_start = false;
    public string END_JSON = "!end";
    public char End_json = '!';

	// Метод, описывающий действия при наведение курсора на объект.
	void OnMouseEnter() {
		// В данном случае, мы получаем объект и меняем его цвет на серый:
		GetComponent<Renderer>().material.color = Color.gray;
	}

	// Метод, описывающий действия при отпускании курсора с объекта.
	void OnMouseExit() {
		GetComponent<Renderer>().material.color = Color.white;
	}

	// Метод, описывающий действия при нажатии на объект. Если это 
	// объект с флагом is_start, то делается переход на сцену с именем
	// start. Аналогично происходит и с другими блоками if.
	    void OnMouseUp() {
		    if(is_start)
		    {
                flag_start = true;
                // Делаем надпись CONNECTING активной.
                ConnectingSplashScreen.SetActive(true);
                // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
                client = new TcpClient(server, port);
                // "Привязываем" сокет к нашему экземпляру соединения с сервером.
                socket = client.Client;
                // Если соединение установлено, то...
                if (socket.Connected == true)
                {
                    // Получаем поток для чтения и записи данных.
                    stream = client.GetStream();

                    // Производим сериализацию Json пакета.
                    Head_ReqToServer_Login login = new Head_ReqToServer_Login()
                    {
                        action = "login",
                        content = new Head_ReqToServer_Login.Content()
                        {
                            email = "blablabla@mail.ru",
                            md5_password = "698d51a19d8a121ce581499d7b701668"
                        }
                    };
                    // С помощью JsonConvert производим сериализацию структуры выше.
                    string json = JsonConvert.SerializeObject(login, Formatting.Indented)+"!end";
                    // Переводим наше сообщение в ASCII, а затем в массив Byte.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(json);
                    // Отправляем сообщение нашему серверу. 
                    stream.Write(data, 0, data.Length);
                }
		    }
		if(is_statistics)
		{
			Application.LoadLevel(statistics);
		}
		if(is_settings)
		{
            OnMouseExit();
            MainMenuItems.SetActive(false);
            SettingsMenuItems.SetActive(true);
		}
		if(is_exit)
		{
			Application.Quit();
		}
	}

    // Метод для печати результата в файл.
    private static void PrintText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
   
}