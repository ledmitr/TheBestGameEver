package gserv;

import java.io.*;
import java.net.*;
import java.util.*;

import org.json.simple.JSONObject;
import gserv.Helper;

/**
 * Класс обеспечивает связь с клиентом по tcp/ip протоколу.
 * Обеспечивает приём данных и сохренние их в очередь, а также
 * позволяет совершать передачу данных клиентам.
 * Класс выполняется в отдельном потоке.
 */
public class NetCon extends Thread {
    /**
     * Сокет для связи с клиентом
     */
    protected Socket sock;

    /**
     * Буфер принятых данных
     */
    protected String buffer;

    /**
     * Буферная очередь принятых данных
     */
    protected Queue<JSONObject> reciveData;

    /**
     * Конструктор класса
     *
     * @param sc сокет клиента
     */
    NetCon(Socket sc) {
        super();
        buffer = "";
        sock = sc;
        reciveData = new LinkedList<JSONObject>();
    }

    /**
     * Точка входа при создании потока.
     * Здесь считываются данные из буфера сокета.
     * После чего, целые корректные json данные помещаются в буферную очередь.
     */
    public void run()
    {
        try {
            System.out.println("Client has started!");
            InputStream is = sock.getInputStream();
            NetParser parser = new NetParser(is, reciveData);
            while (true) {
                parser.goParse();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Функция читает данные из буферной очереди, если это возможно
     *
     * @return json объект или null, если очередь пуста
     */
    public JSONObject tryGetReciveData()
    {
        return reciveData.poll();
    }

    /**
     * Осуществляет отправку строковых данных клиенту
     *
     * @param str строка для передачи
     * @return true в случае успеха и false в противном
     */
    public boolean sendData(String str)
    {
        try {
            str = str.concat("!end");
            OutputStream os = sock.getOutputStream();
            os.write(str.getBytes());
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    /**
     * Осущетвляет отправку строковых данных, но перед этим конвертирует json объект в строку
     *
     * @param obj json объект
     * @return true в случае успеха и false в противном
     */
    public boolean sendData(JSONObject obj)
    {
        return sendData(obj.toJSONString());
    }
}
