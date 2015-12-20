﻿namespace Assets.TD.scripts
{
    public abstract class Message
    {
        public string action { get; set; }
    }

// Класс, хранящий поля блока json (запрос клиента на авторизацию):
    public class Head_ReqToServer_Login : Message
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
    public class Head_RespFromServer_Login : Message
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
    public class Head_ReqFromServer_ConnectToGame : Message
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
    public class Head_ReqToServer_HandShake : Message
    {
        public Content content { get; set; }
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
    public class Head_RespFromServer_HandShake : Message
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

    public class AddUnitRequestToServer : Message
    {
        public Content content { get; set; }

        public class Content
        {
            public int position_x { get; set; }
            public int position_y { get; set; }
            public int type_unit { get; set; }
        }
    }

    public class PrepareToStart : Message
    {
        public Content content { get; set; }

        public class Content
        {
            public int you_role { get; set; }
            public int map_width { get; set; }
            public int map_height { get; set; }
            public int[][] map { get; set; }
        }
    }

    public class PrepareToStartResponse : Message
    {
        public int code { get; set; }
        public string content { get; set; }
    }

    public class GameToStart : Message
    {
        public string content { get; set; }
    }

    public class StagePlanning : Message
    {
        public Content content { get; set; }
        public class Content
        {
            public int time { get; set; }
            public string message { get; set; }
        }
    }

    public class StageSimulate : Message
    {
        public string content { get; set; }
    }

    public class StageFinish : Message
    {
        public string content { get; set; }
    }
}