import logifyInfoCollector from "../../src/collectors/logifyInfoCollector.js";
var assert = require('chai').assert;

describe('logifyInfoCollector tests', function() {

    it('process test', function () {
        let collector = new logifyInfoCollector();
        
        let reportData = new Object();

        collector.process(null, reportData);

        assert.equal("15.2.11", reportData.clientVersion);
        assert.equal("js", reportData.devPlatform);
        assert.equal("1.0", reportData.logifyProtocolVersion);
    });
});