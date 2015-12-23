using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Assets.TD.scripts.Constants;
using Newtonsoft.Json;
using UnityEngine;

// Класс главного меню.
namespace Assets.TD.scripts
{
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
            if(_flagStart)
            {
                if (_socket != null && _socket.Connected)
                {
                    if (_stream.DataAvailable)
                    {
                        while (_socket.Connected)
                        {
                            // Буфер для хранения принятого массива bytes.
                            Byte[] buffer = new Byte[256];
                            // Читаем пакет ответа сервера. 
                            Int32 bytes = _stream.Read(buffer, 0, buffer.Length);
                            // Преобразуем в строку.
                            string responseData = Encoding.ASCII.GetString(buffer, 0, bytes);
                            // !!! возможно здесь таится ошибка
                            if (responseData.Contains(EndJsonStr))
                            {
                                string [] splitData = responseData.Split(EndJson);
                                responseData = splitData[0];
                            }
                            else Debug.Log("Плохой пакет: " + responseData);
                            // Происзводим десериализацию.
                            Head_RespFromServer_Login loginResponse = JsonConvert.DeserializeObject<Head_RespFromServer_Login>(responseData);
                            // Если поле action равно login, то используем login_resp, иначе используем другой класс десериализации.
                            if (loginResponse.action == Actions.Login)
                            {
                                // Делаем надпись GameSearch активной, а Connecting убираем.
                                ConnectingSplashScreen.SetActive(false);
                                GameSearchSplashScreen.SetActive(true);
                                GameInfo.PlayerId = loginResponse.content.your_id;
                                _msgFromServer = loginResponse.content.message;
                                Debug.Log(GameInfo.PlayerId);
                                Debug.Log(_msgFromServer);
                            }
                            if (loginResponse.action == Actions.ConnectToGame)
                            {
                                Head_ReqFromServer_ConnectToGame connectResponse = JsonConvert.DeserializeObject<Head_ReqFromServer_ConnectToGame>(responseData);
                                GameInfo.Port = connectResponse.content.port;
                                GameInfo.Host = connectResponse.content.host;
                                GameInfo.Key = connectResponse.content.secret_key;
                                Debug.Log(GameInfo.Port);
                                Debug.Log(GameInfo.Host);
                                Debug.Log(GameInfo.Key);
                                // Закрывем соединение и поток.
                                _socket.Close();
                                _client.Close();
                                _stream.Close();
                                // Делаем надпись GameSearch неактивной, а Loading включаем.
                                GameSearchSplashScreen.SetActive(false);
                                LoadingSplashScreen.SetActive(true);
                                Application.LoadLevel(start);
                            }
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
        public bool IsStart;
        // Строковая переменная, хранящая название сцены Start.
        public string start;
        // Логическая переменная, отвечающая за нажатие кнопки Statistics.
        public bool IsStatistics;
        // Строковая переменная, хранящая название сцены Statistics.
        public string statistics;
        // Логическая переменная, отвечающая за нажатие кнопки Settings.
        public bool IsSettings;
        // Логическая переменная, отвечающая за нажатие кнопки Exit.
        public bool IsExit;
        // Строковая переменная, содержащая IP-адрес сервера.
        private const string Server = ApplicationConst.ServerAddress;
        // Целочисленная переменная, содержащая номер порта.
        private const Int32 Port = ApplicationConst.ServerPort;
        // Пустой экземпляр класса TCP Client.
        private TcpClient _client = null;
        // Пустой экземпляр класса Socket.
        private Socket _socket = null;
        // Пустой экземпляр класса NetworkStream.
        private NetworkStream _stream = null;
        // Полученное сообщение от сервера
        private string _msgFromServer = "";
        private bool _flagStart = false;
        public const string EndJsonStr = "!end";
        public const char EndJson = '!';

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
            if(IsStart)
            {
                _flagStart = true;
                // Делаем надпись CONNECTING активной.
                ConnectingSplashScreen.SetActive(true);
                // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
                _client = new TcpClient(Server, Port);
                // "Привязываем" сокет к нашему экземпляру соединения с сервером.
                _socket = _client.Client;
                // Если соединение установлено, то...
                if (_socket.Connected)
                {
                    // Получаем поток для чтения и записи данных.
                    _stream = _client.GetStream();

                    // Производим сериализацию Json пакета.
                    Head_ReqToServer_Login login = new Head_ReqToServer_Login()
                    {
                        action = Actions.Login,
                        content = new Head_ReqToServer_Login.Content()
                        {
#if UNITY_EDITOR
                            email = "unity-editor@mail.ru",
#else
                            email = "mobile-device@mail.ru",
#endif
                            md5_password = "698d51a19d8a121ce581499d7b701668"
                        }
                    };
                    // С помощью JsonConvert производим сериализацию структуры выше.
                    string json = JsonConvert.SerializeObject(login, Formatting.Indented)+"!end";
                    // Переводим наше сообщение в ASCII, а затем в массив Byte.
                    Byte[] data = Encoding.ASCII.GetBytes(json);
                    // Отправляем сообщение нашему серверу. 
                    _stream.Write(data, 0, data.Length);
                }
            }
            if(IsStatistics)
            {
                Application.LoadLevel(statistics);
            }
            if(IsSettings)
            {
                OnMouseExit();
                MainMenuItems.SetActive(false);
                SettingsMenuItems.SetActive(true);
            }
            if(IsExit)
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
}