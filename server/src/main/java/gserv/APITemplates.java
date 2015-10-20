package gserv;

import org.json.simple.JSONObject;

/**
 * Created by bodrik on 20.10.15.
 */
public class APITemplates {
    public static JSONObject build(String action, int code, String content) {
        JSONObject jsonObj = new JSONObject();
        jsonObj.put("action", action);
        jsonObj.put("code", code);
        jsonObj.put("content", content);
        return jsonObj;
    }

    public static JSONObject build(String action, int code, JSONObject content) {
        JSONObject jsonObj = new JSONObject();
        jsonObj.put("action", action);
        jsonObj.put("code", code);
        jsonObj.put("content", content);
        return jsonObj;
    }
}
