using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Assets.TD.scripts.Constants;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    /// <summary>
    ///     Класс, описывающий поведение главного меню
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        public GameObject MainMenuItems;
        public GameObject SettingsMenuItems;

        public GameObject SplashScreen;
        public Text SplashScreenHeading;
        public Text SplashScreenDetails;

        private void Start()
        {
            SettingsMenuItems.SetActive(false);
            SplashScreen.SetActive(false);
            QualitySettings.SetQualityLevel(0);
        }

        private void Update()
        {
            if (_flagStart)
            {
                try
                {
                    if (_socket != null && _socket.Connected)
                    {
                        if (_stream.DataAvailable)
                        {
                            while (_socket.Connected)
                            {
                                // Буфер для хранения принятого массива bytes.
                                var buffer = new byte[256];
                                // Читаем пакет ответа сервера. 
                                var bytes = _stream.Read(buffer, 0, buffer.Length);
                                // Преобразуем в строку.
                                var responseData = Encoding.ASCII.GetString(buffer, 0, bytes);
                                // !!! возможно здесь таится ошибка
                                if (responseData.Contains(EndJsonStr))
                                {
                                    var splitData = responseData.Split(new[] {EndJsonStr}, StringSplitOptions.RemoveEmptyEntries);
                                    responseData = splitData[0];
                                }
                                else
                                {
                                    Debug.Log("Плохой пакет: " + responseData);
                                }

                                // Производим десериализацию.
                                var loginResponse = JsonConvert.DeserializeObject<Head_RespFromServer_Login>(responseData);
                                // Если поле action равно login, то используем login_resp, иначе используем другой класс десериализации.
                                if (loginResponse.action == Actions.Login)
                                {
                                    // Делаем надпись GameSearch активной, а Connecting убираем.
                                    SplashScreen.SetActive(false);
                                    SplashScreenHeading.text = ApplicationConst.GameSearchHeading;
                                    GameInfo.PlayerId = loginResponse.content.your_id;
                                    _msgFromServer = loginResponse.content.message;
                                    Debug.Log(GameInfo.PlayerId);
                                    Debug.Log(_msgFromServer);
                                }
                                else if (loginResponse.action == Actions.ConnectToGame)
                                {
                                    var connectResponse = JsonConvert.DeserializeObject<Head_ReqFromServer_ConnectToGame>(responseData);
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
                                    // Loading включаем.
                                    SplashScreenHeading.text = ApplicationConst.LoadingHeading;
                                    SplashScreenDetails.text = "please wait while game is loading";
                                    Application.LoadLevel(start);
                                }
                            }
                        }
                    }
                    else
                    {
                        SplashScreenHeading.text = ApplicationConst.ErrorHeading;
                        SplashScreenDetails.text = "connection to server is absent";
                    }
                }
                catch (Exception ex)
                {
                    SplashScreenHeading.text = ApplicationConst.ErrorHeading;
                    SplashScreenDetails.text += "exception occured: " + ex.Message;
                    Debug.Log(ex);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

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
        private const int Port = ApplicationConst.ServerPort;
        // Пустой экземпляр класса TCP Client.
        private TcpClient _client;
        // Пустой экземпляр класса Socket.
        private Socket _socket;
        // Пустой экземпляр класса NetworkStream.
        private NetworkStream _stream;
        // Полученное сообщение от сервера
        private string _msgFromServer = "";
        private bool _flagStart;
        public const string EndJsonStr = "!end";

        // Метод, описывающий действия при наведение курсора на объект.
        private void OnMouseEnter()
        {
            // В данном случае, мы получаем объект и меняем его цвет на серый:
            GetComponent<Renderer>().material.color = Color.gray;
        }

        // Метод, описывающий действия при отпускании курсора с объекта.
        private void OnMouseExit()
        {
            GetComponent<Renderer>().material.color = Color.white;
        }

        // Метод, описывающий действия при нажатии на объект. Если это 
        // объект с флагом is_start, то делается переход на сцену с именем
        // start. Аналогично происходит и с другими блоками if.
        private void OnMouseUp()
        {
            if (IsStart)
            {
                _flagStart = true;
                // Делаем надпись CONNECTING активной.
                SplashScreen.SetActive(true);
                SplashScreenHeading.text = ApplicationConst.ConnectingHeading;
                SplashScreenDetails.text = "creating server connection...\n";
                try
                {
                    // Создаем экземпляр класса TcpClient и пытаемся подключится к серверу.
                    _client = new TcpClient(Server, Port);
                    // "Привязываем" сокет к нашему экземпляру соединения с сервером.
                    _socket = _client.Client;
                    // Если соединение установлено, то...
                    if (_socket.Connected)
                    {
                        SplashScreenDetails.text += "connected.\nbuilding request message to server...\n";

                        // Получаем поток для чтения и записи данных.
                        _stream = _client.GetStream();

                        // Производим сериализацию Json пакета.
                        var login = new Head_ReqToServer_Login
                        {
                            action = Actions.Login,
                            content = new Head_ReqToServer_Login.Content
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
                        var json = JsonConvert.SerializeObject(login, Formatting.Indented) + EndJsonStr;
                        // Переводим наше сообщение в ASCII, а затем в массив Byte.
                        var data = Encoding.ASCII.GetBytes(json);
                        // Отправляем сообщение нашему серверу. 
                        _stream.Write(data, 0, data.Length);

                        SplashScreenDetails.text += "sended to server.\n";
                    }
                }
                catch (Exception ex)
                {
                    SplashScreenHeading.text = ApplicationConst.ErrorHeading;
                    SplashScreenDetails.text += "exception occured: " + ex.Message + "\n";
                    Debug.Log(ex);
                }
            }
            else if (IsStatistics)
            {
                Application.LoadLevel(statistics);
            }
            else if (IsSettings)
            {
                OnMouseExit();
                MainMenuItems.SetActive(false);
                SettingsMenuItems.SetActive(true);
            }
            else if (IsExit)
            {
                Application.Quit();
            }
        }

        // Метод для печати результата в файл.
        private static void PrintText(FileStream fs, string value)
        {
            var info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
}