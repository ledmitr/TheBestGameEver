package game;

import java.io.IOException;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Random;
import java.util.concurrent.Callable;
import java.util.concurrent.CountDownLatch;

/**
 * Created by Елена on 10.10.2015.
 */
public class TempConnection extends Thread{

    Socket sock;
    private CountDownLatch latch;
    volatile static boolean isReady = false;
    volatile static Integer createdPort;
    volatile static String key;

    TempConnection(CountDownLatch latch, Socket sock){
        this.latch = latch;
        this.sock = sock;
    }

    public void run(){
        while(!isReady){}

        OutputStream os = null;
        try {
            os = sock.getOutputStream();
            if (createdPort != null) {
                // TODO: change to sending json message
                System.out.println("Send port number to client");
                os.write(createdPort.byteValue());
            } else {
                throw new IOException();
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
        latch.countDown();

    }

    public static void setPort(int port){
        createdPort = port;
    }

    public static void generateKey(){
            Random random = new Random();
            String allowedChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] tmpKey = new char[8];
            for (int i = 0; i < 8; i++) {
                tmpKey[i] = allowedChars.charAt(random.nextInt(allowedChars.length()));
            }
        key = String.valueOf(tmpKey);
    }
}
