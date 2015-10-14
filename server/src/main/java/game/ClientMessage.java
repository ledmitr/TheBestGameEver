package game;


/**
 * Created by Елена on 06.10.2015.
 */
public abstract class ClientMessage{

    String msg;
    Integer client;
    final MessageType messageType;

    protected ClientMessage(MessageType messageType) {
        this.messageType = messageType;
    }


    public enum MessageType{
        INIT_GAME,
        SEND_MAP,
        NEW_UNIT,
        POLE_STATE,
        GAME_END
    }

}
