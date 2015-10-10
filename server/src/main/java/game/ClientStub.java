package game;

import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;

/**
 * Created by Елена on 10.10.2015.
 */
public class ClientStub {

    public void connectToServer(InetAddress addr, int port) throws IOException {
        Socket soc = new Socket(addr, port);
    }

    public void sendMessage(ClientMessage message) {

    }

    public ClientMessage readMessage() {
        ClientMessage msg = null;
        return msg;
    }
}
