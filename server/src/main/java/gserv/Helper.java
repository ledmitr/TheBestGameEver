package gserv;

import org.json.simple.*;
import org.json.simple.parser.JSONParser;

/**
 * Created by bodrik on 19.10.15.
 */
public class Helper {
    public static JSONObject tryReadJSON(String message) {
        try {
            JSONParser parser = new JSONParser();
            JSONObject jsonObj = (JSONObject) parser.parse(message);
            return jsonObj;
        } catch (Exception e) {
            return null;
        }
    }
}
