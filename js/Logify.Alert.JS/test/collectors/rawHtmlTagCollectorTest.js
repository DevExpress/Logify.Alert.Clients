import scriptsCollector from "../../src/collectors/scriptsCollector.js";
import metasCollector from "../../src/collectors/metasCollector";

var assert = require('chai').assert;

describe('rawHtmlTagCollector tests', function () {

    it('scriptsCollector process test', function () {
        let collector = new scriptsCollector();
        let reportData = {};
        collector.process(getWin(), reportData);
        checkData(reportData, "scripts");
    });

    it('metasCollector process test', function () {
        let collector = new metasCollector();
        let reportData = {};
        collector.process(getWin(), reportData);
        checkData(reportData, "metas");
    });

    function getWin() {
        let win = {};
        win.document = {};
        win.document.getElementsByTagName = function (tagName) {
            if (tagName === 'script' || tagName === 'meta') {
                let result = [];
                let scr = {"innerHTML": "", "outerHTML": "1"};
                result.push(scr);
                scr = {"innerHTML": "1", "outerHTML": "2"};
                result.push(scr);
                scr = {"innerHTML": "", "outerHTML": "3"};
                result.push(scr);
                return result;
            }
        };

        return win;
    }

    function checkData(reportData, dataPropertyName) {
        assert.equal(2, reportData[dataPropertyName].length);
        assert.equal("1", reportData[dataPropertyName][0]);
        assert.equal("3", reportData[dataPropertyName][1]);
    }
});