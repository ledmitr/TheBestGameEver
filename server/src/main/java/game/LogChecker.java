package game;

import java.util.Iterator;

/**
 * Created by Елена on 08.11.2015.
 */
public class LogChecker extends Thread{

    ConnectionManager manager;

    public LogChecker(ConnectionManager manager) {
        this.manager = manager;
    }

    public void run(){
        while(true){
            for (TempConnection connection : manager.connections){
                if (connection.isLogged){
                    manager.loggedConnections.add(connection);
                    manager.connections.remove(connection);
                }
            }

        }
    }
}
