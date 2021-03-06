package gserv;

import gserv.extra.LogException;
import org.json.simple.JSONObject;
import java.util.*;
import gserv.objects.*;

import java.net.ServerSocket;
import java.net.Socket;

/**
 * Класс, который собственно и является игровым сервером для двух клиентов
 * Запускать класс следует в отдельном потоке, передавая сокет,
 * индетификтор игры и секретный ключ в конструктор
 */
public class GameServer implements Runnable {

    private static final String SERVER_NAME = "TheBestGameEver";

    protected volatile int unitCounter;

    /**
     * Коллекция обектов атакующего игрока
     */
    protected volatile ArrayList<GameObject> attackerObjects;

    /**
     * Коллекция обектов защищающегося игрока
     */
    protected volatile ArrayList<GameObject> defenderObjects;

    /**
     * Игровая карта
     */
    protected volatile int[][] gameMap;

    /**
     * Игровой tread
     */
    private SimulateGame gameThread;

    /**
     * Номер сетевого порта
     */
    protected volatile int port;

    /**
     * Индетификатор игры на сервере выше
     */
    protected volatile int game_id;

    /**
     * Секретный ключ, по которому будет производиться авторизация
     */
    protected volatile String secret_key;

    /**
     * Уникальные объекты для каждого игрока
     *
     * @see gserv.Client
     */
    private Client[] clients;

    /**
     * Открытый общий сокет для прослушивания входящих клиентов
     */
    private ServerSocket wait_socket;

    /**
     * Конструктор
     * @param ws сокет для прослушивания
     * @param g индетификатор игры
     * @param s секретный ключ
     */
    public GameServer(ServerSocket ws, int g, String s) {
        wait_socket = ws;
        game_id = g;
        secret_key = s;
        clients = new Client[2];
        gameThread = null;
        unitCounter = 0;
    }

    /**
     * Точка входа нового потока, аналог функции main
     * Здесь происходит начальная инициализация подключений клиентов
     */
    public void run() {
        System.out.println("Симуляция игры запущена");
        try {
            LogException.saveToLog("Game server with id = " + game_id + " has been started!", "");
            for (int i = 0; i < 2; i++) {
                newClient(i, wait_socket.accept());
            }
            proccessGame();
            LogException.saveToLog("Game server with id = " + game_id + " has been stoped!", "");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Данная функция запускает процесс прослушивания подключений и обработку сообщений от клиентов,
     * а также отслеживает рабочее состояние клиентов
     *
     * @throws Exception
     */
    private void proccessGame() throws Exception {
        JSONObject data;
        while (true) {
            for (int i = 0; i < 2; i++) {
                if (clients[i].getState() == Thread.State.TERMINATED) {
                    LogException.saveToLog("Client #" + (i + 1) + " was disconnected. The server goes into standby mode.", "");
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

    /**
     * Создаёт новое объектное представление клиента
     * @param number номер клиента от 0 до 1
     * @param sc сокет нового клиента
     */
    private void newClient(int number, Socket sc) {
        clients[number] = new Client(sc);
        clients[number].start();
    }

    /**
     * Метод обрабатывает полученные данные от клиентов, выполняет требуемые инструкции и отправляет ответ
     * @param numClient номер клиента от 0 до 1
     * @param data json.simple объект сформированный по ходу чтения входящих данных
     */
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
                if (!secret_key.equals(content.get("secret_key"))) {
                    code = -1;
                    throw new Error("You have invalid secret key.");
                }
                new_content.put("server_name", SERVER_NAME);
                new_content.put("game_id", game_id);
                clients[numClient].userId = Integer.valueOf(content.get("user_id").toString());
                clients[numClient].setStatus(Client.STATUS_LOGGED);
                System.out.println("User #" + clients[numClient].userId + " has logged succeful!");
                throw new Error("Hello! You have successfully logged on this server");
            } catch (Error err) {
                new_content.put("message", err.getMessage());
            }
            clients[numClient].sendData(APITemplates.build("handshake", code, new_content));
            //Если оба пользователя прошли авторизацию, то запускаем игровой процесс
            if (clients[0].isLogged() && clients[1].isLogged() && gameThread == null) {
                gameMap = new int[SimulateGame.MAP_HEIGHT + 1][];
                for (int i=0; i<gameMap.length; i++) {
                    gameMap[i] = new int[SimulateGame.MAP_WIDTH + 1];
                    for (int j=0; j<gameMap[i].length; j++) {
                        gameMap[i][j] = 0;
                    }
                }
                attackerObjects = new ArrayList<GameObject>();
                defenderObjects = new ArrayList<GameObject>();
                gameThread = new SimulateGame(gameMap, clients, attackerObjects, defenderObjects);
                gameThread.start();
            }
            return;
        }
        if (!clients[numClient].isLogged()) {
            new_content.put("message", "You need logged on this server before you can do anything.");
            clients[numClient].sendData(APITemplates.build(data.get("action").toString(), -100, new_content));
            return;
        }
        //Ниже обрабатываются запросы уже для представившихся и одобренных клиентов

        /**
         * Меняем статусы клиентов на готовы к запуску
         */
        if (data.get("action").toString().equals("prepare_to_start")) {
            if (data.get("code").toString().equals("0")) {
                clients[numClient].setStatus(Client.STATUS_READY_TO_START);
                LogException.saveToLog("Client with id=" + clients[numClient].userId + " ready to start game!", "Подготовка к игре");
            }
        }

        if (data.get("action").toString().equals("add_unit")) {
            JSONObject content = (JSONObject) data.get("content");
            int posX = new Integer(content.get("position_x").toString());
            int posY = new Integer(content.get("position_y").toString());
            if (content.get("type_unit").toString().equals("11")) { //Если тип юнита солдат
                if (gameMap[posY][posX] == 9) {
                    attackerObjects.add(new SoldierAttacker(unitCounter++, posX, posY, clients[numClient].role, 5));
                } else {
                    System.out.print(gameMap);
                    clients[numClient].sendData(APITemplates.build("Error", 1, "Unit can't to created."));
                }
            }
            if (content.get("type_unit").toString().equals("21")) { //Если тип юнита башня
                if (gameMap[posY][posX] == 1) {
                    defenderObjects.add(new TowerDefender(unitCounter++, posX, posY, clients[numClient].role, 5));
                } else {
                    clients[numClient].sendData(APITemplates.build("Error", 1, "Unit can't to created."));
                }
            }
        }

        /**
         * Обмен сообщениями
         */
        if (data.get("action").toString().equals("message")) {
            if (numClient == 1) {
                clients[0].sendData(APITemplates.build("message", 0, data.get("content").toString()));
            } else {
                clients[1].sendData(APITemplates.build("message", 0, data.get("content").toString()));
            }
            return;
        }
    }
}
