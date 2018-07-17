import customDataCollector from "../../src/collectors/customDataCollector.js";
var assert = require('chai').assert;

describe('customDataCollector tests', function() {

    it('process with data test', function () {
        let mocOwner = new Object();
        mocOwner.customData = "mocCustomData";
        
        let collector = new customDataCollector(mocOwner);
        
        let reportData = new Object();

        collector.process(null, reportData);

        assert.equal("mocCustomData", reportData.customData);
    });

    it('process without data test', function () {
        let mocOwner = new Object();
        let collector = new customDataCollector(mocOwner);
        let reportData = new Object();
        collector.process(null, reportData);
        assert.equal(0, Object.keys(reportData.customData).length);
    });

    it('process with merged custom data test', function () {
        let mocOwner = new Object();
        let collector = new customDataCollector(mocOwner);
        let reportData = new Object();
        collector.process(null, reportData, "test");
        assert.notEqual({ additionalData: 'test' }, reportData.customData);
        collector.process(null, reportData, { myData: 'asd' });
        assert.notEqual({ myData: 'asd' }, reportData.customData);
        collector.process(null, reportData);
        assert.equal(0, Object.keys(reportData.customData).length);
    });
});