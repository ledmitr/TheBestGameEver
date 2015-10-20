package gserv;

import java.net.*;

import org.json.simple.JSONObject;

/**
 * Created by bodrik on 19.10.15.
 */
public class GameServer implements Runnable {
    private static final String SERVER_NAME = "TheBestGameEver";
    protected volatile int port;
    protected volatile int game_id;
    protected volatile String secret_key;
    private Client[] clients;
    private ServerSocket wait_socket;

    GameServer(int p, int g, String s) {
        port = p;
        game_id = g;
        secret_key = s;
        clients = new Client[2];
    }

    public void run() {
        try {
            wait_socket = new ServerSocket(port, 0, InetAddress.getByName("localhost"));
            System.out.println("Game server with id = " + game_id + " has been started!");
            for (int i = 0; i < 2; i++) {
                newClient(i, wait_socket.accept());
            }
            startGame();
            System.out.println("Game server with id = " + game_id + " has been stoped!");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void startGame() throws Exception {
        JSONObject data;
        while (true) {
            for (int i = 0; i < 2; i++) {
                if (clients[i].getState() == Thread.State.TERMINATED) {
                    System.out.println("Client #" + (i + 1) + " was disconnected. The server goes into standby mode.");
                    newClient(i, wait_socket.accept());
                }
                data = clients[i].tryGetReciveData();
                if (data != null) {
                    handler(i, data);
                }
            }
            //Thread.currentThread().sleep(1000);
        }
    }

    private void newClient(int number, Socket sc) {
        clients[number] = new Client(sc);
        clients[number].start();
    }

    private void handler(int numClient, JSONObject data) {
        int code = 0;
        JSONObject new_content = new JSONObject();
        //Здесь мы знакомимся с клиентами
        if (data.get("action").toString().equals("handshake")) {
            try {
                JSONObject content = (JSONObject) data.get("content");
                if (clients[numClient].isLogged()) {
                    code = -2;
                    throw new Error("You have already logged on this server");
                }
                if (!secret_key.equals(content.get("secretkey"))) {
                    code = -1;
                    throw new Error("You have invalid secret key.");
                }
                new_content.put("servername", SERVER_NAME);
                new_content.put("gameid", game_id);
                clients[numClient].name = content.get("name").toString();
                clients[numClient].setStatus(Client.STATUS_LOGGED);
                throw new Error("Hello " + clients[numClient].name + "! You have successfully logged on this server");
            } catch (Error err) {
                new_content.put("message", err.getMessage());
            }
            clients[numClient].sendData(APITemplates.build("handshake", code, new_content));
            return;
        }
        if (!clients[numClient].isLogged()) {
            new_content.put("message", "You need logged on this server before you can do anything.");
            clients[numClient].sendData(APITemplates.build(data.get("action").toString(), -100, new_content));
            return;
        }
        //Ниже обрабатываются запросы уже для представившихся и одобренных клиентов
        if (data.get("action").toString().equals("post")) {
            if (numClient == 1) {
                clients[0].sendData(APITemplates.build("post", 0, data.get("content").toString()));
            } else {
                clients[1].sendData(APITemplates.build("post", 0, data.get("content").toString()));
            }
            return;
        }
    }
}
