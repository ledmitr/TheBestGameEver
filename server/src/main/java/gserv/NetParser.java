package gserv;

import gserv.extra.LogException;
import org.json.simple.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.util.Queue;

import static gserv.Helper.tryReadJSON;

/**
 * Класс обеспечивает парсинг сообщений из сети от клиентов, помещая прочитанное в очередь.
 * Пример вызова из кода:
 *
 * Queue<JSONObject> receiveData;
 * InputStream is = sock.getInputStream();
 * NetParser parser = new NetParser(is, receiveData);
 * while (true) {
 *      parser.goParse();
 * }
 *
 * Пример того, как достать данные из очереди:
 * JSONObject message = receiveData.poll(); - достаём сообщение из очереди
 * Чтобы понять как происходит обработка этих данных, следует обратиться к документации
 * по библиотеке simple json.
 * Пример обработки данных json реализован в функции handler файла GameServer в пакете gserv
 */
public class NetParser {
    /**
     * Определяет последовательность символов, которая говорит об
     * окончании приёма целого пакета данных
     */
    protected static final String END_OF_RECEIVE_DATA = "!end";

    /**
     * Буффер данных, которые пока ещё не прошли успешную обработку на json строку
     */
    private String buffer;

    /**
     * Очередь данных, полученных от клиента скапливается здесь
     */
    private Queue<JSONObject> receiveData;

    /**
     * Входящий поток от сокета клиента
     */
    private InputStream is;

    /**
     * Буфер для данных полученных от сокета за один цикл
     */
    private byte buff[];

    /**
     * Вспомогательный буфер для облегчения парсинга
     */
    private String partsBuffer[];

    /**
     * Конструктор парсера
     *
     * @param arg_is ссылка на объект InputStream
     * @param arg_rd ссылка на объект очереди Queue<JSONObject>
     */
    public NetParser(InputStream arg_is, Queue<JSONObject> arg_rd) {
        is = arg_is;
        receiveData = arg_rd;
        buffer = "";
        buff = new byte[64 * 1024];
    }

    /**
     * Запускаем процесс парсинга
     *
     * @throws IOException вылетает, когда возникают проблемы с входным потоком
     */
    public void goParse() throws IOException {
        buffer = buffer.concat(new String(buff, 0, is.read(buff)));
        //Проверяем, пришли ли данные полностью, разделяем буфер делителем END_OF_RECEIVE_DATA
        while (buffer.contains(END_OF_RECEIVE_DATA)) {
            partsBuffer = buffer.split(END_OF_RECEIVE_DATA, 2);
            buffer = partsBuffer[1];
            JSONObject data = Helper.tryReadJSON(partsBuffer[0]);
            if (data == null) {
                continue;
            }
            receiveData.add(data);
        }
    }
}
