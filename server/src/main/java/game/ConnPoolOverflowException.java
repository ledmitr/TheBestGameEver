package game;

/**
 * Created by Елена on 10.10.2015.
 */
public class ConnPoolOverflowException extends Exception{

    public String toString(){
        String str = "Temporary Connection pool overflow";
        return str;
    }
}
