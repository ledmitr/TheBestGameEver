package gserv;

import gserv.NetCon;
import gserv.extra.LogException;
import org.json.simple.JSONObject;
import java.net.Socket;
import java.util.LinkedList;

/**
 * Класс для представления клиента
 */
public class Client extends NetCon {
    /**
     * Всевозможные статусы клиента
     */
    public static int STATUS_NOT_LOGGED = 0;
    public static int STATUS_LOGGED = 1;
    public static int STATUS_READY_TO_START = 2;
    public static int STATUS_IN_GAME = 3;
    public static int STATUS_PAUSE = 4;
    public static int STATUS_GAME_OVER = 5;

    /**
     * Хранит текущий статус клиента
     */
    private int status;

    /**
     * Индетификтор пользователя на фронт-сервере
     */
    public int userId;

    /**
     * Конструктор
     *
     * @param sc сокет клиента
     */
    Client(Socket sc)
    {
        super(sc);
        status = STATUS_NOT_LOGGED;
    }

    /**
     * Установка статуса клиента
     *
     * @param st номер статуса
     */
    public void setStatus(int st)
    {
        status = st;
    }

    /**
     * Проверяет, прошёл ли клиент авторизацию на сервере
     *
     * @return true в случае если прошёл и false в противном
     */
    public boolean isLogged()
    {
        if (status == STATUS_NOT_LOGGED) {
            return false;
        }
        return true;
    }

    @Override
    public boolean sendData(String str)
    {
        if (super.sendData(str)) {
            LogException.saveToLog("The data was successfully sent to the client with userid " + userId, str);
            return true;
        }
        LogException.saveToLog("Unable to send data to the client with userid " + userId, str);
        return false;
    }

    @Override
    public JSONObject tryGetReciveData()
    {
        JSONObject jsobj = reciveData.poll();
        if (jsobj != null) {
            LogException.saveToLog("The data was successfully get from the client with userid " + userId, jsobj.toJSONString());
        }
        return jsobj;
    }
}
