package gserv;

import gserv.GameServer;

/**
 * Точка для запуска сервера.
 * Это временный класс создан для запуска автономного сервера на двоих игроков.
 * В дальнейшем этот класс будет удалён
 */
public class PointEntry {
    public static void main(String[] args) {
        //Запускаем новый сервер в новом потоке
        Thread game1 = new Thread(new GameServer(1993, 21, "H34MS23"));
        game1.start();
        System.out.println("Main stream has complited!");
    }
}
