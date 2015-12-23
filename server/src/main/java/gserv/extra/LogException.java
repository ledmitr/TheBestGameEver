package gserv.extra;

/**
 * Created by bodrik on 04.11.15.
 * Класс вызова исключений с последующим сохранением его в лог бд
 */
public class LogException extends Exception {

    /**
     * Базовый конструктор
     */
    public LogException() {
        super();
        saveToLog("Exception: undefined", this.getStackTrace().toString());
    }

    /**
     * Расширеный конструктор
     * @param message сообщение об ошибке
     */
    public LogException(String message) {
        super(message);
        saveToLog("Exception: " + message, this.getStackTrace().toString());
    }

    /**
     * Функция для сохранения записи события в лог бд
     * @param message сообщение об ошибки
     * @param extra дополнительные данные
     */
    public static void saveToLog(String message, String extra) {
        try {
            System.out.println("LOG: " + message);
            DbConnector.getStatement().executeUpdate(
                    "INSERT INTO logs (content, date_event, extra) " +
                            "VALUES ('" + message + "', now()::timestamp, '" + extra + "')"
            );
        } catch (Exception e) {
            System.out.println("Record to database is not provide.");
        }
    }
}
