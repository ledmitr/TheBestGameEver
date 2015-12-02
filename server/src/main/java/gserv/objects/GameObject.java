package gserv.objects;

/**
 * Общий абстрактный класс игровых обектов
 */
public abstract class GameObject {

    public static final int COORD_X = 0;
    public static final int COORD_Y = 1;
    public static final int DEFENDER = 1;
    public static final int ATTACKER = 0;

    public static final int DIRECTION_LEFT = 0;
    public static final int DIRECTION_RIGHT = 1;
    public static final int DIRECTION_TOP = 2;
    public static final int DIRECTION_DOWN = 3;

    public int type;
    public String name;
    protected int[] position;
    public int gold;
    public int hitpoint;
    public int owner;
    public int direction;

    {
        position = new int[2];
        owner = -1;
        gold = 0;
        position[COORD_X] = 0;
        position[COORD_Y] = 0;
        hitpoint = 0;
        name = "Unnamed";
        type = 0;
        direction = DIRECTION_RIGHT;
    }

    public void setPosition(int x, int y)
    {
        position[COORD_X] = x;
        position[COORD_Y] = y;
    }

    public int[] getPosition()
    {
        return position;
    }
}
