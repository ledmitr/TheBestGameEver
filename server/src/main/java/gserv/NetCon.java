package gserv;

import java.io.*;
import java.net.*;

/**
 * Created by bodrik on 19.10.15.
 */
public class NetCon implements Runnable {
    private Socket sock;
    private String buffer;

    NetCon(Socket sc) {
        buffer = "";
        sock = sc;
    }

    public void run() {
        try {
            System.out.println("Client has started!");
            InputStream is = sock.getInputStream();
            byte buff[] = new byte[64 * 1024];
            while (true) {
                buffer = new String(buff, 0, is.read(buff));
                if (buffer.length() > 0) {
                    System.out.println(buffer);
                    buffer = "";
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
