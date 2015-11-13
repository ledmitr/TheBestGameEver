/**
 * Зависимости
 */
var express = require('express');
var controllers = require('./controllers');
var fs = module.exports = require('fs');
var app = module.exports = express.createServer();

/**
 * Переменная расшерения, содержащяя инструменты для пересылки в контроллер
 */
var ext = module.exports = {
    'ws': require('ws'),
    'pg': require('pg'),
    'constants': {
        'SERVER_NAME': 'TheBestGameEverWEB',
        'SERVER_HOST': '104.155.17.75',
        'CONTROLLERS_DIR': __dirname + '/controllers',
        'WEB_SERVER_PORT': 80,
        'WEB_SOCKET_PORT': 8081,
        'PG_USER': 'postgres',
        'PG_PASSWORD': '111',
        'PG_HOST': '104.155.17.75',
        'PG_PORT': '5432',
        'PG_DATABASE': 'tbge_db',
        'LOG_TIME_UPDATE': 1000
    }
}

/**
 * Конфигурирование
 */
app.configure(function(){
    app.set('server_name', 'TheBestGameEverWEB');
    app.set('env', 'development');
    app.set('views', __dirname + '/views');
    app.set('view engine', 'jade');
    app.disable('view cache');
    app.use(express.bodyParser());
    app.use(express.static(__dirname + '/public'));
});

app.configure('development', function(){
    app.use(express.errorHandler({ dumpExceptions: true, showStack: true }));
});

app.configure('production', function(){
    app.use(express.errorHandler());
});

/**
 * Роутинг
 */
app.get(/^(\/.*)$/i, function (req, res, next) {
    var purl = req.params[0].split('/');
    var predictChunk = [''];
    var counter = 0;
    for (var i in purl) {
        if (purl[i] == '') {
            continue;
        }
        predictChunk[counter + 1] = (predictChunk[counter] + '/' + purl[i]).toLowerCase();
        counter++;
    }
    predictChunk.reverse();
    //Определение и вызов контроллера
    for (var i in predictChunk) {
        if (fs.existsSync(ext.constants.CONTROLLERS_DIR + predictChunk[i] + '.js')) {
            var args = ("req, res, ext, '" + predictChunk[0].replace(predictChunk[i], '').replace(/\/{1}/ig, "', '") + "'").replace(/(,\s)?'{2}/ig, '');
            var foreval = "require('./controllers" + predictChunk[i] + "').index(" + args + ");";
            console.log(foreval);
            eval(foreval);
            return;
        }
    }
    next();
});

app.get('/', controllers.index);

/**
 * Обработка ошибок и исключений
 */
/*app.error(function(err, req, res, next) {
    if (err instanceof Error) {
        console.log("Error: " + err.message);
        res.send("Ошибка сервера!");
    } else {
        next(err);
    }
});*/

/**
 * Старт web сервера
 */
app.listen(ext.constants.WEB_SERVER_PORT, function(){
    console.log(ext.constants.SERVER_NAME + " server listening on port %d in %s mode", app.address().port, app.settings.env);
});

/**
 * Старт web сокета
 */
var webSockClients = [];
var wsid = 0;
var webSockServer = new ext.ws.Server({
    'port': ext.constants.WEB_SOCKET_PORT
});
webSockServer.on('connection', function(ws) {
    webSockClients[++wsid] = ws;
    console.log("На веб сокете новый клиент #%d", wsid);
    var json = JSON.stringify({
        'action': 'hello',
        'data': {
            'SERVERNAME': ext.constants.SERVER_NAME,
            'SERVERHOST': ext.constants.SERVER_HOST,
        }
    });
    ws.send(json);
    ws.on('message', function(message) {
        console.log("Получено сообщение %s", message);
    });
    ws.on('close', function(message) {
        console.log('Клиент #%d отключился от web сокета.', wsid);
        delete webSockClients[wsid];
    });
});
/**
 * Запускаем таймер и читаем логи
 */
var lastRecordId = null;
var pgAdress ='postgres://' +
    ext.constants.PG_USER +
    ':' + ext.constants.PG_PASSWORD +
    '@' + ext.constants.PG_HOST +
    ':' + ext.constants.PG_PORT +
    '/' + ext.constants.PG_DATABASE;

var pgClient = new ext.pg.Client(pgAdress);
pgClient.connect();

setInterval(function() {
    var q = null;
    try {
        if (lastRecordId == null) {
            q = pgClient.query("SELECT * FROM logs ORDER BY id DESC LIMIT 100");
        } else {
            q = pgClient.query(
                "SELECT * FROM logs " +
                "WHERE id > " + lastRecordId + " " +
                "ORDER BY id DESC LIMIT 100"
            );
        }
    } catch(e) {
        console.log('Ошибка при обращении к бд.');
        return;
    }
    q.on('row', function (row, result) {
        result.addRow(row);
    });
    q.on('end', function (result) {
        if (result.rowCount < 1) {
            return;
        }
        lastRecordId = result.rows[0].id;
        var rows = result.rows.reverse();
        var json = JSON.stringify({
            'action': 'new-event',
            'data': rows
        });
        for (var id in webSockClients) {
            try { //Переделать этот момент
                webSockClients[id].send(json);
            } catch(e) {
                delete webSockClients[id];
            }
        }
        console.log('Log has been update last in %s', rows[0].date_event);
    });
}, ext.constants.LOG_TIME_UPDATE);

