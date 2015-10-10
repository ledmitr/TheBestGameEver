package game;

import java.util.concurrent.ConcurrentHashMap;

/**
 * Created by Елена on 10.10.2015.
 */
public class ConnectionManager {
    private ConcurrentHashMap<String, TempConnection> connectionPool;
    private boolean isFull = false;

    public void addConnection(String connName, TempConnection conn) throws ConnPoolOverflowException {
        int currentPoolSize = connectionPool.size();
        if (currentPoolSize >= 2) {
            throw new ConnPoolOverflowException();
        }
        else{
            connectionPool.put(connName, conn);
        }

        if (connectionPool.size() == 2){
            redirectConnections();
            clearPool();
        }
    }

    private void clearPool(){
        connectionPool.clear();
        isFull = false;
    }

    private void redirectConnections(){
        clearPool();
    }
}
