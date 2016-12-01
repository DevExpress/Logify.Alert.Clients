import platformEnvironmentCollector from "../../src/collectors/platformEnvironmentCollector.js";
var assert = require('chai').assert;
var os = require('os');

describe('platformEnvironmentCollector tests', function() {

    it('process test', function () {
        let reportData = new Object();

        let collector = new platformEnvironmentCollector();
        collector.process(reportData);

        assert.equal(os.platform(), reportData.platformEnvironment.platform);
        assert.equal(os.cpus().length, reportData.platformEnvironment.cpus.length);
        assert.equal(os.arch(), reportData.platformEnvironment.architecture);
        assert.notEqual(undefined, reportData.platformEnvironment.freeMemory);
        assert.equal(os.totalmem(), reportData.platformEnvironment.totalMemory);
        assert.equal(os.release(), reportData.platformEnvironment.release);
    });
});