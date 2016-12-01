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

        assert.equal(undefined, reportData.customData);
    });
});