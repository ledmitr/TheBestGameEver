/**
 * Created by bodrik on 10.11.15.
 */

exports.index = function(req, res, ext) {
    res.render('agent.jade', ext);
}