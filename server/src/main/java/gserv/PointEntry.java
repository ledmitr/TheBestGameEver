package gserv;

import gserv.GameServer;

/**
 * Created by bodrik on 19.10.15.
 */
public class PointEntry {
    public static void main(String[] args) {
        Thread game1 = new Thread(new GameServer(1993, 21));
        game1.start();
        System.out.println("Main stream has complited!");
    }
}
