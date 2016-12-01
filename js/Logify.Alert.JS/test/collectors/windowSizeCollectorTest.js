import windowSizeCollector from "../../src/collectors/windowSizeCollector.js";
var assert = require('chai').assert;

describe('windowSizeCollector tests', function() {

    it('process test', function () {
        let collector = new windowSizeCollector();
        let win = new Object();
        win.innerHeight = "mocHeight";
        win.innerWidth = "mocWidth";

        let reportData = new Object();

        collector.process(win, reportData);

        assert.equal("mocHeight", reportData.window.height);
        assert.equal("mocWidth", reportData.window.width);
    });
});