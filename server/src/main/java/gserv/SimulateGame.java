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
     * Количество атакующих юнитов, которые должны попасть к башне для победы атакующего.
     */
    public static final int MAX_HEALTH_TOWER = 10;

    /**
     * Ширина карты
     */
    public static final int MAP_WIDTH = 100;

    /**
     * Высота карты
     */
    public static final int MAP_HEIGHT = 100;

    /**
     * Коллекция обектов атакующего игрока
     */
    protected volatile ArrayList<GameObject> attackerObjects;

    /**
     * Коллекция обектов защищающегося игрока
     */
    protected volatile ArrayList<GameObject> defenderObjects;

    protected int attackerDeads;

    protected int defenderDeads;

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
        attackerDeads = 0;
        defenderDeads = 0;
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
                clients[i].sendData(APITemplates.build("prepare_to_start", 0, new_content[i]));
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
            File fl = new File("/home/bodrik/main.map");
            FileInputStream reader = null;
            if (fl.exists()) {
                reader = new FileInputStream("/home/bodrik/main.map");
            } else {
                reader = new FileInputStream("/home/born_s13666/main.map");
            }
            int buf, i = 0, j = 0;
            while((buf=reader.read())!=-1){
                if ( buf >= '0' && buf <= '9') {
                    if (i == MAP_WIDTH) {
                        i = 0;
                        j++;
                    }
                    if (j == MAP_HEIGHT) break;
                    gameMap[j][i++] = (char)buf - '0';
                }
            }
        }
        catch(IOException ex){
            LogException.saveToLog(ex.getMessage(), ex.getStackTrace().toString());
        }
    }

    private JSONObject getFreshData(ArrayList<GameObject> obj)
    {
        JSONObject content = new JSONObject();
        JSONArray units = new JSONArray();
        Iterator objIterator = obj.iterator();
        while (objIterator.hasNext()) {
            GameObject currentUnit = (GameObject)objIterator.next();
            JSONObject unit = new JSONObject();
            unit.put("id_unit", currentUnit.id);
            unit.put("type_unit", currentUnit.type);
            unit.put("hit_point", currentUnit.hitpoint);
            int pos[] = currentUnit.getPosition();
            unit.put("position_x", pos[GameObject.COORD_X]);
            unit.put("position_y", pos[GameObject.COORD_Y]);
            unit.put("direction", currentUnit.direction);
            units.add(unit);
        }
        content.put("units", units);
        return content;
    }

    private void gotoStagePlanning(int time)
    {
        LogException.saveToLog("Current stage is planning. It's completed from " + time + " seconds.", "GAME IS RUNNING");
        JSONObject new_content = new JSONObject();
        new_content.put("time", time);
        new_content.put("message", "Current stage of game has been changed on 'planning'.");
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("stage_planning", 0, new_content));
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
        exitPoint:
        while (!attackerObjects.isEmpty()) {
            try {
                //Эмулируем передвижения юнитов
                Iterator attackerIterator = attackerObjects.iterator();
                while (attackerIterator.hasNext()) {
                    SoldierAttacker soldier = (SoldierAttacker)attackerIterator.next();
                    int posX = soldier.getPosition()[GameObject.COORD_X];
                    int posY = soldier.getPosition()[GameObject.COORD_Y];
                    int trackX = soldier.getTracks()[GameObject.COORD_X];
                    int trackY = soldier.getTracks()[GameObject.COORD_Y];
                    //Просто передвигаем юнита
                    if ((gameMap[posY][posX + 1] == 0 || gameMap[posY][posX + 1] == 8) && !(trackX == posX + 1 && trackY == posY)) {
                        soldier.direction = 1;
                        soldier.setPosition(posX + 1, posY);
                        if (gameMap[posY][posX] == 5) {
                            gameMap[posY][posX] = 0;
                        }
                        if (gameMap[posY][posX + 1] == 0 ) {
                            gameMap[posY][posX + 1] = 5;
                        }
                    } else {
                        if ((gameMap[posY + 1][posX] == 0 || gameMap[posY + 1][posX] == 8) && !(trackX == posX && trackY == posY + 1)) {
                            soldier.direction = 3;
                            soldier.setPosition(posX, posY + 1);
                            if (gameMap[posY][posX] == 5) {
                                gameMap[posY][posX] = 0;
                            }
                            if (gameMap[posY + 1][posX] == 0) {
                                gameMap[posY + 1][posX] = 5;
                            }
                        } else {
                            if ((gameMap[posY - 1][posX] == 0 || gameMap[posY - 1][posX] == 8) && !(trackX == posX && trackY == posY - 1)) {
                                soldier.direction = 2;
                                soldier.setPosition(posX, posY - 1);
                                if (gameMap[posY][posX] == 5) {
                                    gameMap[posY][posX] = 0;
                                }
                                if (gameMap[posY - 1][posX] == 0) {
                                    gameMap[posY - 1][posX] = 5;
                                }
                            } else {
                                if (!(trackX == posX - 1 && trackY == posY)) {
                                    soldier.direction = 0;
                                    soldier.setPosition(posX - 1, posY);
                                    if (gameMap[posY][posX] == 5) {
                                        gameMap[posY][posX] = 0;
                                    }
                                    if (gameMap[posY][posX - 1] == 0) {
                                        gameMap[posY][posX - 1] = 5;
                                    }
                                }
                            }
                        }
                    }
                    posX = soldier.getPosition()[GameObject.COORD_X];
                    posY = soldier.getPosition()[GameObject.COORD_Y];
                    // Если солдат дошёл до главного тавера
                    if (gameMap[posY][posX] == 8) {
                        defenderDeads++;
                        attackerIterator.remove();
                    } else {
                        //Если здоровье солдата на нуле, убиваем его к чертям
                        if (soldier.hitpoint == 0) {
                            attackerDeads++;
                            attackerIterator.remove();
                        }
                    }
                    if (defenderDeads >= MAX_HEALTH_TOWER) {
                        break exitPoint;
                    }
/*                    Iterator defenderIterator = defenderObjects.iterator();
                    while (defenderIterator.hasNext()) {

                    }*/
                }
                //Эмулируем обстрел башен
                Iterator defenderIterator = defenderObjects.iterator();
                nextTower:
                while (defenderIterator.hasNext()) {
                    TowerDefender tower = (TowerDefender)defenderIterator.next();
                    int posX = tower.getPosition()[GameObject.COORD_X];
                    int posY = tower.getPosition()[GameObject.COORD_Y];
                    Iterator soldiersIterator = attackerObjects.iterator();
                    nextSoldier:
                    while (soldiersIterator.hasNext()) {
                        GameObject soldier = (GameObject) soldiersIterator.next();
                        int posXSoldier = soldier.getPosition()[GameObject.COORD_X];
                        int posYSoldier = soldier.getPosition()[GameObject.COORD_Y];
                        for (int i = 0; i <= tower.attackRange; i++) {
                            for (int j = 0; j <= tower.attackRange; j++) {
                                if (soldier.hitpoint > 0) {
                                    if ((posX == posXSoldier - i && posY == posYSoldier - j) ||
                                        (posX == posXSoldier - i && posY == posYSoldier + j) ||
                                        (posX == posXSoldier + i && posY == posYSoldier - j) ||
                                        (posX == posXSoldier + i && posY == posYSoldier + j)
                                    ) {
                                        soldier.hitpoint--;
                                        if (soldier.hitpoint == 0) {
                                            defenderDeads++;
                                        }
                                        continue nextTower;
                                    }
                                } else {
                                    continue nextSoldier;
                                }
                            }
                        }
                    }
                }
                Thread.sleep(SIMULATE_DELAY);
            } catch (Exception e) {
                LogException.saveToLog(e.getMessage(), e.getStackTrace().toString());
            }
            JSONArray new_content = new JSONArray();
            JSONObject attacker_content = getFreshData(attackerObjects);
            attacker_content.put("is_dead", attackerDeads);
            new_content.add(attacker_content);
            JSONObject defender_content = getFreshData(defenderObjects);
            defender_content.put("tower_health", MAX_HEALTH_TOWER - defenderDeads);
            new_content.add(defender_content);
            for (int i = 0; i < 2; i++) {
                clients[i].sendData(APITemplates.build("actual_data", 0, new_content));
            }
        }
    }

    private void gotoStageFinish()
    {
        int whoWin = 0; // 0 - победили атакующие, 1 - защищающиеся
        if (MAX_HEALTH_TOWER <= defenderDeads) {
            whoWin = 1;
        }
        LogException.saveToLog("Current stage is simulate.", "GAME IS RUNNING");
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("stage_finish", whoWin, "Current stage of game has been changed on 'finish'."));
        }
    }

    private void proccesGame()
    {
        clients[0].setStatus(Client.STATUS_IN_GAME);
        clients[1].setStatus(Client.STATUS_IN_GAME);
        for (int i = 0; i < 2; i++) {
            clients[i].sendData(APITemplates.build("game_to_start", 0, "Game has been started! Please wait next instructions."));
        }
/*        while(true) {
            //Первый этап, перехоим в режим планирования
            gotoStagePlanning(10);
            //Первый этап, переходим в режим симуляции
            gotoStageSimulate();
        }*/
/*        //Второй этап, перехоим в режим планирования
        gotoStagePlanning(10);
        //Второй этап, переходим в режим симуляции
        gotoStageSimulate();*/
        //Третий этап, перехоим в режим планирования
        gotoStagePlanning(40);
        //Третий этап, переходим в режим симуляции
        gotoStageSimulate();
        //Конец игры
        //Первый этап, переходим в режим симуляции
        gotoStageFinish();
    }
}
