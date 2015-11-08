package game;

import gserv.NetParser;
import org.json.simple.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.text.ParseException;
import java.util.LinkedList;
import java.util.Queue;
import java.util.Random;

/**
 * Created by Елена on 10.10.2015.
 */
public class TempConnection extends Thread{

    Socket sock;
    volatile boolean isReady = false;
    volatile boolean isLogged = false;
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

    public void run() {
        System.out.println("New client connected");
        waitForAutorization();

        // Waiting for game
        while(!isReady){}

        sendGameProperties();

    }

    private void waitForAutorization(){
        try {
            is = sock.getInputStream();
            os = sock.getOutputStream();

            Queue<JSONObject> receiveData = new LinkedList<JSONObject>();
            InputStream is = sock.getInputStream();

            NetParser parser = new NetParser(is, receiveData);
            while(receiveData.isEmpty()) {
                parser.goParse();
            }
            JSONObject data = receiveData.remove();
            String action = (String)data.get("action");
            if (!action.isEmpty()) {
                if (action.equals("login")) {
                    JSONObject tmp = (JSONObject) data.get("content");
                    String email = (String) tmp.get("email");
                    String password = (String) tmp.get("md5_password");
                    isLogged = true;
                    System.out.println("client loginned as "+email);
                }
            }
            else {
                // TODO: add error json processing
                throw new ParseException("Incorrect message", 0);
            }

        } catch (IOException e) {
            e.printStackTrace();
        } catch (ParseException e) {
            e.printStackTrace();
        }

    }

    private void sendGameProperties(){
        try {
            if (createdPort != null) {
                System.out.println("Send port number to client");
                JSONObject obj = new JSONObject();
                obj.put("action", "connect_to_game");

                JSONObject content = new JSONObject();
                // TODO: replace with id from database
                Random random = new Random();
                content.put("port", createdPort);
                // TODO: replace with host name
                content.put("host", "localhost");
                content.put("secret_key", key);
                obj.put("content", content);
                String response = obj.toString().concat("!end");
                os.write(response.getBytes());
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
