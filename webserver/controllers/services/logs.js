/**
 * Created by bodrik on 09.11.15.
 * Контроллер для вывода таблицы логов
 */

exports.index = function(req, res, ext, limit) {
    var adress ='postgres://' +
        ext.constants.PG_USER +
        ':' + ext.constants.PG_PASSWORD +
        '@' + ext.constants.PG_HOST +
        ':' + ext.constants.PG_PORT +
        '/' + ext.constants.PG_DATABASE;
    var client = new ext.pg.Client(adress);
    var q = null;
    client.connect();
    if (/^[0-9]*$/i.test(limit)) {
        q = client.query("SELECT * FROM logs ORDER BY date_event DESC LIMIT " + limit);
    } else {
        q = client.query("SELECT * FROM logs ORDER BY date_event DESC");
    }
    q.on('row', function (row, result) {
        result.addRow(row);
    });
    q.on('end', function (result) {
        res.render('showlogs.jade', { 'logs' : result.rows });
        client.end();
    });
}