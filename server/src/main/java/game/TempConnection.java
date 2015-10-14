package game;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.Callable;

/**
 * Created by Елена on 10.10.2015.
 */
public class TempConnection implements Callable{

    Socket sock;
    volatile static boolean isReady = false;

    TempConnection(Socket sock){
        this.sock = sock;
    }

    public Integer call(){
        Integer createdPort = null;
        while(!isReady){}
        try {
            ServerSocket gameSocket = new ServerSocket();
            createdPort = gameSocket.getLocalPort();
            ClientMessage message = new ClientMessage(createdPort);
        } catch (IOException e) {
            e.printStackTrace();
        }

        return createdPort;


    }
}
