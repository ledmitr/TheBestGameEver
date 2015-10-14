package game;

/**
 * Created by Елена on 10.10.2015.
 */
public class NoOpponentException extends Exception{
    public String toString(){
        String str = "No opponent connected";
        return str;
    }
}
