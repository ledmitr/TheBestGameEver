package gserv;

import org.json.simple.JSONObject;

/**
 * Класс для построения API сообщений по шаблонам
 */
public class APITemplates {

    /**
     * Строит стандартный API запрос/ответ, в качестве контента выступает строка
     *
     * @param action  название производимого действия
     * @param code    состояние действия
     * @param content строка с содержанием
     * @return собраный json объект
     */
    public static JSONObject build(String action, int code, String content) {
        JSONObject jsonObj = new JSONObject();
        jsonObj.put("action", action);
        jsonObj.put("code", code);
        jsonObj.put("content", content);
        return jsonObj;
    }

    /**
     * Строит стандартный API запрос/ответ, в качестве контента выступает json объект
     *
     * @param action  название производимого действия
     * @param code    состояние действия
     * @param content строка с содержанием
     * @return собраный json объект
     */
    public static JSONObject build(String action, int code, JSONObject content)
    {
        JSONObject jsonObj = new JSONObject();
        jsonObj.put("action", action);
        jsonObj.put("code", code);
        jsonObj.put("content", content);
        return jsonObj;
    }
}
