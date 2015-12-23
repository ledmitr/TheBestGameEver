package gserv.objects;

/**
 * Класс солдата первого уровня атакующей комманды
 * type = 11
 */
public class SoldierAttacker extends GameObject
{
    public SoldierAttacker(int sid, int x, int y, int own, int hp) {
        position = new int[2];
        owner = own;
        gold = 10;
        position[COORD_X] = x;
        position[COORD_Y] = y;
        hitpoint = hp;
        name = "Soldier";
        type = 11;
        id = sid;
    }
}
