package game;

import gserv.NetParser;
import org.json.simple.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.Queue;
import java.util.concurrent.CountDownLatch;

/**
 * Created by Елена on 10.10.2015.
 */
public class TempConnection extends Thread{

    Socket sock;
    volatile boolean isReady = false;
    volatile Integer createdPort;
    volatile String key;
    private byte[] inBuf = new byte[256];
    private byte[] outBuf = new byte[256];
    InputStream is;
    OutputStream os;

    TempConnection(Socket sock){
        this.sock = sock;
        this.start();
    }

    public void run(){
        try {
            is = sock.getInputStream();
            os = sock.getOutputStream();

            Queue<JSONObject> reciveData = null;
            InputStream is = sock.getInputStream();
            NetParser parser = new NetParser(is, reciveData);
           // while (true) {
                parser.goParse();
            //}
            System.out.print(reciveData.poll().toString());

        } catch (IOException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }


        while(!isReady){}


        try {
            if (createdPort != null) {
                // TODO: change to sending json message
                System.out.println("Send port number to client");
                os.write(createdPort.byteValue());
            } else {
                throw new IOException("New port number is not available");
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        try {
            System.out.println("Close temporary connection");
            sock.close();
        } catch (IOException e) {
            e.printStackTrace();
        }

    }

    void setReady(Integer port, String key){
        this.createdPort = port;
        this.key = key;
        this.isReady = true;
    }
}
