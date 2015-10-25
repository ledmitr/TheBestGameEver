package game;

import gserv.GameServer;

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
                TempConnection first = new TempConnection(mainSocket.accept());
                TempConnection second = new TempConnection(mainSocket.accept());
                new Thread(first).start();
                new Thread(second).start();
            } catch (IOException e) {
                e.printStackTrace();
            }

            ServerSocket gameSocket = null;
            try {
                gameSocket = new ServerSocket(0);
                int createdPort = gameSocket.getLocalPort();
                TempConnection.setPort(createdPort);
            } catch (IOException e) {
                e.printStackTrace();
            }
            TempConnection.generateKey();

            TempConnection.isReady = true;

            // TODO: REMOVE IT by latch or smth else
            try {
                sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

            Thread gameThread = new Thread(new GameServer(gameSocket, 123, TempConnection.key));
            gameThread.start();

        }

    }

}
