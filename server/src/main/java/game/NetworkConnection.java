package game;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.concurrent.*;

import org.json.simple.JSONObject;

/**
 * Created by Елена on 06.10.2015.
 */
public class NetworkConnection extends Thread {

    private ServerSocket mainSocket;
    private byte[] inBuf = new byte[256];
    private byte[] outBuf = new byte[256];

    NetworkConnection() {
        System.out.println("Connection started");

        try {
            mainSocket = new ServerSocket(8000);
            System.out.println("Main socket opened");
        } catch (IOException e) {
            e.printStackTrace();
        }


    }

    @Override
    public void run() {


        JSONObject resultJson = new JSONObject();

        resultJson.put("name", "foo");
        resultJson.put("num", new Integer(100));
        resultJson.put("is_vip", new Boolean(true));
        resultJson.put("nickname", null);
        System.out.print(resultJson.toString());

    }

    private void mWaitForTwoClients() {
        int connectionsCounter = 0;
        Socket[] clientsSocket = null;
        while (connectionsCounter < 2) {
            System.out.println("Waiting for clients...");
            clientsSocket[connectionsCounter++] = null;
            ExecutorService es = Executors.newFixedThreadPool(2);
            Future f1 = es.submit(new Callable() {
                /**
                 * Computes a result, or throws an exception if unable to do so.
                 *
                 * @return computed result
                 * @throws Exception if unable to compute a result
                 */
                public Socket call() throws Exception {
                    return mWaitForClient();
                }
            });

            while (!f1.isDone()) {

            }
            try {
                System.out.println("task has been completed : " + f1.get());

            } catch (InterruptedException ie) {
                ie.printStackTrace(System.err);

            } catch (ExecutionException ee) {
                ee.printStackTrace(System.err);

            }
            es.shutdown();
        }
    }

    private Socket mWaitForClient() {
        Socket soc = null;
        try {
            soc = mainSocket.accept();
            InputStream is = soc.getInputStream();
            OutputStream os = soc.getOutputStream();
        } catch (IOException e) {
            e.printStackTrace();
        }
        return soc;
    }

}
