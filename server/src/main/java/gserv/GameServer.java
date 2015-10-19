package gserv;

import java.io.*;
import java.net.*;

import gserv.NetCon;

/**
 * Created by bodrik on 19.10.15.
 */
public class GameServer implements Runnable {
    public volatile int port;
    public volatile int game_id;
    private Thread[] clients;

    GameServer(int p, int g) {
        port = p;
        game_id = g;
        clients = new Thread[2];
    }

    public void run() {
        try {
            ServerSocket wait_socket = new ServerSocket(port, 0, InetAddress.getByName("localhost"));
            System.out.println("Game server with id = " + game_id + " has been started!");
            clients[0] = new Thread(new NetCon(wait_socket.accept()));
            clients[0].start();
            clients[1] = new Thread(new NetCon(wait_socket.accept()));
            clients[1].start();
            System.out.println("Game server with id = " + game_id + " has been stoped!");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
