package gserv;

import gserv.GameServer;

import java.net.InetAddress;
import java.net.ServerSocket;

/**
 * Точка для запуска сервера.
 * Это временный класс создан для запуска автономного сервера на двоих игроков.
 * В дальнейшем этот класс будет удалён
 */
public class PointEntry {
    public static void main(String[] args) throws Exception {
        //Запускаем новый сервер в новом потоке
        Thread game1 = new Thread(new GameServer(new ServerSocket(1995, 0, InetAddress.getByName(args[0].toString())), 21, "H34MS23"));
        game1.start();
        System.out.println("Main stream has complited!" + args[0]);
    }
}
