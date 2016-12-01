import nodeCollector from "../../src/collectors/nodeCollector.js";
var assert = require('chai').assert;
var os = require('os');

describe('nodeCollector tests', function() {
    it('check all error data test', function () {
        let collector = createCollector();
        let errorObject = new Object();
        errorObject.message = "mocErrorMsg";
        errorObject.stack = "mocStack";
        collector.handleException(errorObject);
        checkErrorData(collector);
    });

    it('check all rejection data with error reason test', function () {
        let collector = createCollector();
        let reasonObject = new Object();
        reasonObject.message = "mocErrorMsg";
        reasonObject.stack = "mocStack";
        collector.handleRejection(reasonObject);
        checkRejectionData(collector, true, true);
    });

    it('check rejection data with error reason without stack test', function () {
        let collector = createCollector();
        let reasonObject = new Object();
        reasonObject.message = "mocErrorMsg";
        collector.handleRejection(reasonObject);
        checkRejectionData(collector, true, false);
    });

    it('check whole rejection data with object reason test', function () {
        let collector = createCollector();
        let reasonObject = "mocReason";
        collector.handleRejection(reasonObject);
        checkRejectionData(collector, false, false);
    });

    function checkErrorData(collector) {
        assert.equal(3, collector.collectors.length);

        assert.equal(undefined, collector.reportData.rejection);

        assert.equal("mocErrorMsg", collector.reportData.error.message);
        assert.equal("mocStack", collector.reportData.error.stack);

        checkCommonData(collector);
    }

    function checkRejectionData(collector, hasMessage, hasStack) {
        assert.equal(3, collector.collectors.length);
        assert.equal(undefined, collector.reportData.error);
        
        if (hasMessage) {
            assert.notEqual(undefined, collector.reportData.rejection.error);
            assert.equal("mocErrorMsg", collector.reportData.rejection.error.message);
            if(hasStack) {
                assert.equal("mocStack", collector.reportData.rejection.error.stack);
            } else {
                assert.equal(undefined, collector.reportData.rejection.error.stack);
            }
        }
        else {
            assert.equal("mocReason", collector.reportData.rejection.reason);
        }

        checkCommonData(collector);
    }

    function checkCommonData(collector) {
        assert.equal("mocAppName", collector.reportData.logifyApp.name);
        assert.equal("mocAppVersion", collector.reportData.logifyApp.version);
        assert.equal("mocUserId", collector.reportData.logifyApp.userId);

        assert.equal("15.2.11", collector.reportData.clientVersion);
        assert.equal("node.js", collector.reportData.devPlatform);
        assert.equal("1.0", collector.reportData.logifyProtocolVersion);

        assert.equal(process.versions, collector.reportData.nodeEnvironmentVersions);

        assert.equal(os.platform(), collector.reportData.platformEnvironment.platform);
        assert.equal(os.cpus().length, collector.reportData.platformEnvironment.cpus.length);
        assert.equal(os.arch(), collector.reportData.platformEnvironment.architecture);
        assert.notEqual(undefined, collector.reportData.platformEnvironment.freeMemory);
        assert.equal(os.totalmem(), collector.reportData.platformEnvironment.totalMemory);
        assert.equal(os.release(), collector.reportData.platformEnvironment.release);
    }
    function createCollector() {
        let collector = new nodeCollector();
        collector.applicationName = "mocAppName";
        collector.applicationVersion = "mocAppVersion";
        collector.userId = "mocUserId";

        return collector;
    }
});
