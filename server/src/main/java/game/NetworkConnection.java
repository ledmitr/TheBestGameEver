package game;

import java.io.IOException;
import java.net.ServerSocket;

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

        ConnectionManager manager = new ConnectionManager();
        manager.start();
        while(true){

            try {
                TempConnection tempConnection = new TempConnection(mainSocket.accept());
                manager.addConnection(tempConnection);
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

}
