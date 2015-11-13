package game;

import gserv.extra.LogException;

import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;

//import org.json.simple.JSONObject;

/**
 * Created by Елена on 06.10.2015.
 */
public class NetworkConnection extends Thread {

    private ServerSocket mainSocket;
    private byte[] inBuf = new byte[256];
    private byte[] outBuf = new byte[256];

    private String outerHost;

    NetworkConnection(String host, String port, String outerHost){
        LogException.saveToLog("Connection started", "main server");
        this.outerHost = outerHost;
        try {
            System.out.println(Integer.parseInt(port));
            mainSocket = new ServerSocket(Integer.parseInt(port), 0, InetAddress.getByName(host));
            LogException.saveToLog("Main socket opened", "main server");
        } catch (IOException e) {
            LogException.saveToLog(e.getMessage(), e.getStackTrace().toString());
            e.printStackTrace();
        }
    }


    @Override
    public void run() {

        ConnectionManager manager = new ConnectionManager(outerHost);
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
