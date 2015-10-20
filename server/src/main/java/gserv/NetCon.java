package gserv;

import java.io.*;
import java.net.*;
import java.util.*;

import org.json.simple.JSONObject;
import gserv.Helper;
import sun.misc.Regexp;

/**
 * Created by bodrik on 19.10.15.
 */
public class NetCon extends Thread {
    protected static final String END_OF_RECIVE_DATA = "!end";
    protected Socket sock;
    protected String buffer;
    protected Queue<JSONObject> reciveData;

    NetCon(Socket sc) {
        super();
        buffer = "";
        sock = sc;
        reciveData = new LinkedList<JSONObject>();
    }

    public void run() {
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

    public JSONObject tryGetReciveData() {
        return reciveData.poll();
    }

    public boolean sendData(String str) {
        try {
            str = str.concat("!end");
            OutputStream os = sock.getOutputStream();
            os.write(str.getBytes());
        } catch (Exception e) {
            return false;
        }
        return true;
    }

    public boolean sendData(JSONObject obj) {
        return sendData(obj.toJSONString());
    }
}
