import scriptsCollector from "../../src/collectors/scriptsCollector.js";
var assert = require('chai').assert;

describe('scriptsCollector tests', function() {

    it('process test', function () {
        let reportData = new Object();

        let collector = new scriptsCollector();
        let win = new Object();
        win.document = new Object();
        win.document.getElementsByTagName =  function (tagName) {
            if(tagName == 'script') {
                var result = [];
                var scr = { "innerHTML":"", "outerHTML":"1" };
                result.push(scr);
                scr = { "innerHTML":"1", "outerHTML":"2" };
                result.push(scr);
                scr = { "innerHTML":"", "outerHTML":"3" };
                result.push(scr);
                return result;
            }
        };

        collector.process(win, reportData);

        assert.equal(2, reportData.scripts.length);
        assert.equal("1", reportData.scripts[0]);
        assert.equal("3", reportData.scripts[1]);
    });
});