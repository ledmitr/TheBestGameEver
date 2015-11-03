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

            CountDownLatch latch = new CountDownLatch(2);
            TempConnection.isReady = false;
            try {
                new TempConnection(latch, mainSocket.accept()).start();
                System.out.println("First client connected");
                new TempConnection(latch, mainSocket.accept()).start();
                System.out.println("Second client connected");
            } catch (IOException e) {
                e.printStackTrace();
            }

            ServerSocket gameSocket = null;
            try {
                System.out.println("Create game socket");
                gameSocket = new ServerSocket(0);
                int createdPort = gameSocket.getLocalPort();
                TempConnection.setPort(createdPort);
            } catch (IOException e) {
                e.printStackTrace();
            }
            TempConnection.generateKey();

            TempConnection.isReady = true;

            try {
                latch.await();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            System.out.println("Start game thread");
            new Thread(new GameServer(gameSocket, 123, TempConnection.key)).start();
        }
    }

    public void waitForTwoClients(){

    }
}
