package gserv;

import gserv.extra.LogException;
import gserv.objects.*;
import org.json.simple.*;

import java.io.*;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.Random;

/**
 * Этот класс реализует процесс игры
 */
public class SimulateGame extends Thread
{
    public static final int STATE_PREPARING = 0;
    public static final int STATE_READY = 1;
    public static final int STATE_PLAY = 2;
    public static final int STATE_FINISH = 3;

    public static final int SIMULATE_DELAY = 1000; //ms

    /**
     * Ширина карты
     */
    public static final int MAP_WIDTH = 10;

    /**
     * Высота карты
     */
    public static final int MAP_HEIGHT = 10;

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
     * Игровой статус
     */
    private int gameStatus;

    /**
     * Уникальные объекты для каждого игрока
     *
     * @see gserv.Client
     */
    private Client[] clients;

    public SimulateGame(int[][] gm, Client[] argClients, ArrayList<GameObject> attacker, ArrayList<GameObject> defender) {
        gameMap = gm;
        clients = argClients;
        attackerObjects = attacker;
        defenderObjects = defender;
        gameStatus = STATE_PREPARING;
    }

    public int getGameStatus()
    {
        return gameStatus;
    }

    @Override
    public void run()
    {
        loadMap();
        allocationRandomRoles();
        sendPreparedData();
        while (!(clients[0].isReady() && clients[1].isReady())) {
            try {
                Thread.sleep(3000);
            } catch (Exception e) {
                LogException.saveToLog(e.getMessage(), e.getStackTrace().toString());
            }
        }
        proccesGame();
    }

    private void sendPreparedData() {
            JSONObject new_content[] = new JSONObject[2];
            JSONArray map = new JSONArray();
            for (int i = 0; i < MAP_HEIGHT; i++){
                JSONArray row = new JSONArray();
                for (int j = 0; j < MAP_WIDTH; j++){
                    row.add(j, gameMap[i][j]);
                }
                map.add(i, row);
            }
            for (int i = 0; i < 2; i++){
                new_content[i] = new JSONObject();
                new_content[i].put("you_role", clients[i].role);
                new_content[i].put("map_width", MAP_WIDTH);
                new_content[i].put("map_height", MAP_HEIGHT);
                new_content[i].put("map", map);
                clients[i].sendData(APITemplates.build("prepare_to_start", 0, new_content[i].toJSONString()));
            }
    }

    private void allocationRandomRoles()
    {
        Random rand = new Random();
        clients[0].role = rand.nextInt(1);
        if (clients[0].role == 1) {
            clients[1].role = 0;
        } else {
            clients[1].role = 1;
        }
    }

    private void loadMap()
    {
        try {
            FileReader reader = new FileReader(SimulateGame.class.getResource("/main.map").getFile());
            int buf, i = 0, j = 0;
            while((buf=reader.read())!=-1){
                if ( buf >= '0' && buf <= '9') {
                    if (i == MAP_HEIGHT) {
                        i = 0;
                        j++;
                    }
                    if (j == MAP_WIDTH) break;
                    gameMap[j][i++] = (char)buf - '0';
                }
            }
        }
        catch(IOException ex){
            LogException.saveToLog(ex.getMessage(), ex.getStackTrace().toString());
        }
    }

    private void gotoStagePlanning(int time)
    {
        LogException.saveToLog("Current stage is planning. It's completed from " + time + " seconds.", "GAME IS RUNNING");
        JSONObject new_content = new JSONObject();
        new_content.put("time", time);
        new_content.put("message", "Current stage of game has been changed on 'planning'.");
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("stage_planning", 0, new_content.toJSONString()));
        }
        //Ждём time секунд
        try {
            Thread.sleep(time * 1000);
        } catch (Exception e) {
            LogException.saveToLog(e.getMessage(), e.getStackTrace().toString());
        }
    }

    private void gotoStageSimulate()
    {
        LogException.saveToLog("Current stage is simulate.", "GAME IS RUNNING");
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("stage_simulate", 0, "Current stage of game has been changed on 'simulate'."));
        }
        while (!attackerObjects.isEmpty()) {
            try {
                Thread.sleep(SIMULATE_DELAY);
            } catch (Exception e) {
                LogException.saveToLog(e.getMessage(), e.getStackTrace().toString());
            }
/*            JSONObject new_content = new JSONObject();
            JSONArray container = new JSONArray();
            JSONObject attacker_content = new JSONObject();
            Iterator attackerIterator = attackerObjects.iterator();
            while (attackerIterator.hasNext()) {
                attackerIterator
            }
            JSONObject defender_content = new JSONObject();*/
            for (int i = 0; i < 2; i++) {
                clients[i].sendData(APITemplates.build("actual_data", 0, "test Simulate"));
            }
        }
    }

    private void gotoStageFinish()
    {
        LogException.saveToLog("Current stage is simulate.", "GAME IS RUNNING");
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("stage_finish", 0, "Current stage of game has been changed on 'finish'."));
        }
    }

    private void proccesGame()
    {
        clients[0].setStatus(Client.STATUS_IN_GAME);
        clients[1].setStatus(Client.STATUS_IN_GAME);
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("game_to_start", 0, "Game has been started! Please wait next instructions."));
        }
        //Первый этап, перехоим в режим планирования
        gotoStagePlanning(45);
        //Первый этап, переходим в режим симуляции
        gotoStageSimulate();
        //Второй этап, перехоим в режим планирования
        gotoStagePlanning(45);
        //Второй этап, переходим в режим симуляции
        gotoStageSimulate();
        //Третий этап, перехоим в режим планирования
        gotoStagePlanning(45);
        //Третий этап, переходим в режим симуляции
        gotoStageSimulate();
        //Конец игры
        //Первый этап, переходим в режим симуляции
        gotoStageFinish();
    }
}
