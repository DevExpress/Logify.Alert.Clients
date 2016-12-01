import osVersionCollector from "../../src/collectors/osVersionCollector.js";
var assert = require('chai').assert;

describe('osVersionCollector tests', function() {

    it('process test', function () {
        let collector = new osVersionCollector();
        let win = new Object();
        win.navigator = new Object();
        win.navigator.platform = "mocPlatform";

        let reportData = new Object();
        
        collector.process(win, reportData);

        assert.equal("mocPlatform", reportData.os.platform);
    });
});