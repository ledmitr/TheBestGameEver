package game;

import gserv.extra.LogException;

import java.io.IOException;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        LogException.saveToLog("Main server started", "HOST: " + (String)args[0] + " PORT:" + (String)args[1]);
        NetworkConnection netThread = new NetworkConnection((String)args[0], (String)args[1], (String)args[2]);
        netThread.start();
        while(true){
            try {
                char letter = (char) System.in.read();
                switch(letter){
                    case 'q':
                        System.exit(0);
                    default:
                }
            } catch (IOException e) {
                LogException.saveToLog(e.getMessage(), e.getStackTrace().toString());
            }
        }
    }
}
