package gserv;

import gserv.NetCon;
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
     * Имя клиента/игрока
     */
    public String name;

    /**
     * Конструктор
     *
     * @param sc сокет клиента
     */
    Client(Socket sc) {
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
}