package gserv.objects;

/**
 * Класс солдата первого уровня атакующей комманды
 * type = 11
 */
public class SoldierAttacker extends GameObject
{
    /**
     * Следы
     */
    private int tracks[];

    public SoldierAttacker(int sid, int x, int y, int own, int hp) {
        position = new int[2];
        tracks = new int[2];
        owner = own;
        gold = 10;
        position[COORD_X] = x;
        position[COORD_Y] = y;
        tracks[COORD_X] = x;
        tracks[COORD_Y] = y;
        hitpoint = hp;
        name = "Soldier";
        type = 11;
        id = sid;
    }

    public int[] getTracks() {
        return tracks;
    }

    public void setTracks() {
        tracks[COORD_X] = position[COORD_X];
        tracks[COORD_Y] = position[COORD_Y];
    }

@Override
    public void setPosition(int x, int y)
    {
        setTracks();
        super.setPosition(x, y);
    }
}
