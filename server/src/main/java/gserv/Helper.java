package gserv;

import org.json.simple.*;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;

/**
 * Класс с разнообразными полезными функциями
 */
public class Helper {
    /**
     * Функция извлекает из json строки json объект
     *
     * @param message json строка
     * @return json объект
     */
    public static JSONObject tryReadJSON(String message) {
        try {
            JSONParser parser = new JSONParser();
            JSONObject jsonObj = null;
            jsonObj = (JSONObject) parser.parse(message);
            return jsonObj;
        } catch (ParseException e) {
            System.out.println("Catch ParseException");
            return null;
        }
    }
}
