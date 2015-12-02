package gserv;

import gserv.extra.LogException;
import gserv.objects.*;
import org.json.simple.*;

import java.io.*;
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

    /**
     * Ширина карты
     */
    public static final int MAP_WIDTH = 5;

    /**
     * Высота карты
     */
    public static final int MAP_HEIGHT = 5;

    /**
     * Коллекция обектов атакующего игрока
     */
    protected volatile GameObject[] attackerObjects;

    /**
     * Коллекция обектов защищающегося игрока
     */
    protected volatile GameObject[] defenderObjects;

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

    public SimulateGame(int[][] gm, Client[] argClients) {
        gameMap = gm;
        clients = argClients;
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
}
