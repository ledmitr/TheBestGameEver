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
    ConcurrentLinkedDeque<TempConnection> loggedConnections;
    LogChecker logChecker;

    public void run(){
        while(true){
            if (loggedConnections.size()>=2){
                System.out.println("Create game socket");
                ServerSocket gameSocket = null;
                try {
                    gameSocket = new ServerSocket(0);
                } catch (IOException e) {
                    e.printStackTrace();
                }
                int createdPort = gameSocket.getLocalPort();

                TempConnection first = loggedConnections.removeFirst();
                String key = generateKey();
                first.setReady(createdPort, key);
                TempConnection second = loggedConnections.removeFirst();
                second.setReady(createdPort, key);

                System.out.println("Start game thread");
                new Thread(new GameServer(gameSocket, 123, key)).start();
            }
        }
    }

    ConnectionManager() {
        connections = new ConcurrentLinkedDeque<TempConnection>();
        loggedConnections = new ConcurrentLinkedDeque<TempConnection>();
        logChecker = new LogChecker(this);
        logChecker.start();
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

