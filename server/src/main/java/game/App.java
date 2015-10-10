package game;

import java.io.IOException;
import java.net.UnknownHostException;

/**
 * Hello world!
 *
 */
public class App 
{
    public static void main( String[] args )
    {
        System.out.println("Server started");
        NetworkConnection netThread = null;
        try {
            netThread = new NetworkConnection();
        } catch (UnknownHostException e) {
            e.printStackTrace();
        }
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
                e.printStackTrace();
            }
        }
    }
}
