import customDataCollector from "../../src/collectors/customDataCollector.js";
var assert = require('chai').assert;

describe('customDataCollector tests', function() {

    it('process with data test', function () {
        let collector = new customDataCollector("mocCustomData");

        let reportData = new Object();

        collector.process(reportData);

        assert.equal("mocCustomData", reportData.customData);
    });

    it('process without data test', function () {
        let collector = new customDataCollector();

        let reportData = new Object();

        collector.process(reportData);

        assert.equal(undefined, reportData.customData);
    });
});