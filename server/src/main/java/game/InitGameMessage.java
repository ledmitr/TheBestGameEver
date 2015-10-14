package game;

/**
 * Created by Елена on 14.10.2015.
 */
public class InitGameMessage extends ClientMessage{
    InitGameMessage(){
        super(MessageType.INIT_GAME);
    }


}
