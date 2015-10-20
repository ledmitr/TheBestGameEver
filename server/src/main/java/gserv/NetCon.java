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
     * Определяет последовательность символов, которая говорит об
     * окончании приёма целого пакета данных
     */
    protected static final String END_OF_RECIVE_DATA = "!end";

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
            byte buff[] = new byte[64 * 1024];
            String partsBuffer[];
            while (true) {
                buffer = buffer.concat(new String(buff, 0, is.read(buff)));
                //Проверяем, пришли ли данные полностью, разделяем буфер делителем END_OF_RECIVE_DATA
                while (buffer.indexOf(END_OF_RECIVE_DATA) != -1) {
                    partsBuffer = buffer.split(END_OF_RECIVE_DATA, 2);
                    buffer = partsBuffer[1];
                    //Пытаемся прочитать json сообщение
                    JSONObject data = Helper.tryReadJSON(partsBuffer[0]);
                    if (data == null) {
                        continue;
                    }
                    //Добавляем данные в очередь приёма
                    reciveData.add(data);
                }
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
