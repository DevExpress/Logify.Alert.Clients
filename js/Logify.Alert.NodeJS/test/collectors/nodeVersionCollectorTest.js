import nodeVersionCollector from "../../src/collectors/nodeVersionCollector.js";
var assert = require('chai').assert;
var process = require('process');

describe('processVersionCollector tests', function() {

    it('process test', function () {
        let reportData = new Object();

        let collector = new nodeVersionCollector();
        collector.process(reportData);
        assert.equal(process.versions, reportData.nodeEnvironmentVersions);
    });
});