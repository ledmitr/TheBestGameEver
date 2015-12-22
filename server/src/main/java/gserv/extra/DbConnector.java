package gserv.extra;

import java.sql.*;

/**
 * Created by bodrik on 08.11.15.
 * Класс для связи с субд postgres
 * Выполнено в стиле singleton
 */
public class DbConnector {

    /**
     * Подключён ли к бд
     */
    private static boolean isConnected = false;

    /**
     * Драйвер соединения
     */
    private static Connection dbcon = null;

    /**
     * URL к базе состоит из протокола:подпротокола://[хоста]:[порта_СУБД]/[БД]
     */
    private static String postgre_url = "jdbc:postgresql://23.251.139.4:5432/tbge_db";

    /**
     * Имя пользователя бд
     */
    private static String postgre_user = "postgres";

    /**
     * Пароль пользователя
     */
    private static String postgre_password = "111";

    /**
     * Инициализируем статику
     */
    static {
        try {
            Class.forName("org.postgresql.Driver");
            dbcon = DriverManager.getConnection(postgre_url, postgre_user, postgre_password);
            isConnected = true;
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Получаем instance обекта соединения с бд
     * @return переменая для работы с бд
     * @throws Exception если невозможно получить инстанс
     */
    public static Statement getStatement() throws Exception{
        if (!isConnected) {
            throw new Exception("Don't connect to database");
        }
        return dbcon.createStatement();
    }
}
