package game;

import java.io.IOException;
import java.io.OutputStream;
import java.net.Socket;
import java.util.concurrent.CountDownLatch;

/**
 * Created by Елена on 10.10.2015.
 */
public class TempConnection extends Thread{

    Socket sock;
    private CountDownLatch latch;
    volatile boolean isReady = false;
    volatile Integer createdPort;
    volatile String key;

    TempConnection(Socket sock){
        //this.latch = latch;
        this.sock = sock;
        this.start();
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
        //latch.countDown();

    }

    void setReady(Integer port, String key){
        this.createdPort = port;
        this.key = key;
        this.isReady = true;
    }
}
