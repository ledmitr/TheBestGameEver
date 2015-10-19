package gserv;

import java.io.*;
import java.net.*;
import java.util.*;

import org.json.simple.JSONObject;
import gserv.NetCon;

/**
 * Created by bodrik on 19.10.15.
 */
public class GameServer implements Runnable {
    protected volatile int port;
    protected volatile int game_id;
    private NetCon[] clients;
    private ServerSocket wait_socket;

    GameServer(int p, int g) {
        port = p;
        game_id = g;
        clients = new NetCon[2];
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
                    System.out.println(data.get("action"));
                    System.out.println(data.get("code"));
                    System.out.println(data.get("data"));
                }
            }
            //Thread.currentThread().sleep(1000);
        }
    }

    private void newClient(int number, Socket sc) {
        clients[number] = new NetCon(sc);
        clients[number].start();
    }
}
