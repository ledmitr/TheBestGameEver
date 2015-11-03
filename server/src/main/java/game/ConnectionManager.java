package game;

import gserv.GameServer;

import java.io.IOException;
import java.net.ServerSocket;
import java.util.*;
import java.util.concurrent.*;

/**
 * Created by Елена on 03.11.2015.
 */
public class ConnectionManager extends Thread{
    ConcurrentLinkedDeque<TempConnection> connections;

    public void run(){
        while(true){
            if (connections.size()>=2){
                System.out.println("Create game socket");
                ServerSocket gameSocket = null;
                try {
                    gameSocket = new ServerSocket(0);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                int createdPort = gameSocket.getLocalPort();

                TempConnection first = connections.removeFirst();
                String key = generateKey();
                first.setReady(createdPort, key);
                TempConnection second = connections.removeFirst();
                second.setReady(createdPort, key);

                System.out.println("Start game thread");
                new Thread(new GameServer(gameSocket, 123, key)).start();
            }
        }
    }

    ConnectionManager() {
        connections = new ConcurrentLinkedDeque<TempConnection>();
    }

    void addConnection(TempConnection connection){
        connections.addLast(connection);
    }
    String generateKey(){
        Random random = new Random();
        String allowedChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] tmpKey = new char[8];
        for (int i = 0; i < 8; i++) {
            tmpKey[i] = allowedChars.charAt(random.nextInt(allowedChars.length()));
        }
        return String.valueOf(tmpKey);
    }


}

