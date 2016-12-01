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

        let reportData = new Object();
        reportData.Result = "";

        collector.process(reportData);

        assert.equal("11111", reportData.Result);
    });
});

class mocInternalCollector extends collectorBase {
    process(report) {
        super.process(report);

        report.Result = report.Result + "1";
    }
}
