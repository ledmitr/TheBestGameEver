package game;

import java.io.IOException;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.Callable;

/**
 * Created by Елена on 10.10.2015.
 */
public class TempConnection implements Runnable{

    Socket sock;
    volatile static boolean isReady = false;
    volatile static Integer createdPort;
    volatile static String key;

    TempConnection(Socket sock){
        this.sock = sock;
    }

    public void run(){
        Integer createdPort = null;
        while(!isReady){}

        OutputStream os = null;
        try {
<<<<<<< HEAD
            os = sock.getOutputStream();
            if (createdPort != null) {
                os.write(createdPort.byteValue());
                os.write(key.getBytes());
            }
=======
            ServerSocket gameSocket = new ServerSocket();
            createdPort = gameSocket.getLocalPort();
            //ClientMessage message = new ClientMessage(createdPort);
>>>>>>> 0b036322d70d002245ad907f0bf46d548a71c212
        } catch (IOException e) {
            e.printStackTrace();
        }

    }

    public static void setPort(int port){
        createdPort = port;
    }

    public static void generateKey(){
        key = "abc";
    }
}
