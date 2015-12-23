package gserv.objects;

/**
 * Created by bodrik on 23.12.15.
 */
public class TowerDefender extends GameObject{

    public TowerDefender(int sid, int x, int y, int own, int hp) {
        position = new int[2];
        owner = own;
        gold = 10;
        position[COORD_X] = x;
        position[COORD_Y] = y;
        hitpoint = hp;
        name = "Tower";
        type = 21;
        id = sid;
    }

}
