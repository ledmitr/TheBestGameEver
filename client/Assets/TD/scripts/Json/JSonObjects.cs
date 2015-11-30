public abstract class RequestToServer
{
    public string action { get; set; }
}

// Класс, хранящий поля блока json (запрос клиента на авторизацию):
public class Head_ReqToServer_Login : RequestToServer
{
    public Content content { get; set; }
    // Класс, хранящий поля объекта content:
    // 1) email - электронный адрес 
    // 2) md5_password - md5 хэш пароля
    public class Content
    {
        public string email { get; set; }
        public string md5_password { get; set; }
    }
}

// Класс, хранящий поля блока json (ответ сервера на авторизацию клиента):
public class Head_RespFromServer_Login : RequestToServer
{
    public int code { get; set; }
    public Content content { get; set; }
    // Класс, хранящий поля объекта content:
    // 1) your_id - индетификатор игрока на сервере 
    // 2) message - сообщение с пояснением о выполненой операции 
    public class Content
    {
        public int your_id { get; set; }
        public string message { get; set; }
    }
}

// Класс, хранящий поля блока json (получение номера порта и секр. ключа):
public class Head_ReqFromServer_ConnectToGame : RequestToServer
{
    public Content content { get; set; }
    // Класс, хранящий поля объекта content:
    // 1) port - номер порта для подключения к игровому серверу  
    // 2) host - ip адрес или имя сервера
    // 3) secret_key - секретный ключ, для аунтетификации на игровом сервере
    public class Content
    {
        public int port { get; set; }
        public string host { get; set; }
        public string secret_key { get; set; }
    }
}

// Класс, хранящий поля блока json (подключение клиента к игровому серверу):
public class Head_ReqToServer_HandShake : RequestToServer
{
    public Content content{ get; set; }
    // Класс, хранящий поля объекта content:
    // 1) user_id - ваш индетификатор пользователя 
    // 2) secret_key - секретный ключ, полученный от фронт-сервера
    public class Content
    {
        public int user_id { get; set; }
        public string secret_key { get; set; }
    }
}

// Класс, хранящий поля блока json (успешное подключение к игровому серверу):
public class Head_RespFromServer_HandShake : RequestToServer
{
    public int code { get; set; }
    public Content content { get; set; }
    // Класс, хранящий поля объекта content:
    // 1) server_name - имя игрового сервера 
    // 2) game_id - индетификатор игры 
    // 3) message - описание статуса выполнения
    public class Content
    {
        public string server_name { get; set; }
        public int game_id { get; set; }
        public string message { get; set; }
    }
}
