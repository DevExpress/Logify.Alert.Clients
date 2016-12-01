import screenSizeCollector from "../../src/collectors/screenSizeCollector.js";
var assert = require('chai').assert;

describe('screenSizeCollector tests', function() {

    it('process test', function () {
        let collector = new screenSizeCollector();
        let win = new Object();
        win.screen = new Object();
        win.screen.width = "mocWidth";
        win.screen.height = "mocHeight";

        let reportData = new Object();

        collector.process(win, reportData);

        assert.equal("mocWidth", reportData.screen.width);
        assert.equal("mocHeight", reportData.screen.height);
    });
});