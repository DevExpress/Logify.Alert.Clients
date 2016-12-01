import compositeCollector from "../../src/collectors/compositeCollector.js";
import collectorBase from "../../src/collectors/collectorBase.js";
var assert = require('chai').assert;

describe('compositeCollector tests', function() {

    it('process test', function () {
        let collector = new compositeCollector();

        let internalCollector = new mocInternalCollector();
        collector.collectors.push(internalCollector);
        internalCollector = new mocInternalCollector();
        collector.collectors.push(internalCollector);
        internalCollector = new mocInternalCollector();
        collector.collectors.push(internalCollector);
        internalCollector = new mocInternalCollector();
        collector.collectors.push(internalCollector);
        internalCollector = new mocInternalCollector();
        collector.collectors.push(internalCollector);

        let win = new Object();
        win.Result = "";
        
        let reportData = new Object();
        collector.process(win, reportData);

        assert.equal("11111", win.Result);
    });
});

class mocInternalCollector extends collectorBase {
    process(win, report) {
        super.process(win, report);

        win.Result = win.Result + "1";
    }
}
