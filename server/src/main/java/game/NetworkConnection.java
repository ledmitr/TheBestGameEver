package game;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.*;

//import org.json.simple.JSONObject;

/**
 * Created by Елена on 06.10.2015.
 */
public class NetworkConnection extends Thread {

    private ServerSocket mainSocket;
    private byte[] inBuf = new byte[256];
    private byte[] outBuf = new byte[256];

    NetworkConnection(){
        System.out.println("Connection started");

        try {
            mainSocket = new ServerSocket(8000);
            System.out.println("Main socket opened");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }


    @Override
    public void run() {

        while(true){

            TempConnection.isReady = false;
            try {
                new TempConnection(mainSocket.accept());
                new TempConnection(mainSocket.accept());
            } catch (IOException e) {
                e.printStackTrace();
            }
            TempConnection.isReady = true;
        }

    }

}
