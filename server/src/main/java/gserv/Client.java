package gserv;

import gserv.NetCon;
import org.json.simple.JSONObject;

import java.net.Socket;
import java.util.LinkedList;

/**
 * Created by bodrik on 20.10.15.
 */
public class Client extends NetCon {
    public static int STATUS_NOT_LOGGED = 0;
    public static int STATUS_LOGGED = 1;
    public static int STATUS_READY_TO_START = 2;
    public static int STATUS_IN_GAME = 3;
    public static int STATUS_PAUSE = 4;
    public static int STATUS_GAME_OVER = 5;

    private int status;
    public String name;

    Client(Socket sc) {
        super(sc);
        status = STATUS_NOT_LOGGED;
    }

    public void setStatus(int st) {
        status = st;
    }

    public boolean isLogged() {
        if (status == STATUS_NOT_LOGGED) {
            return false;
        }
        return true;
    }
}
