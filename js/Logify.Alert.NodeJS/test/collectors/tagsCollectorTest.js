import tagsCollector from "../../src/collectors/tagsCollector.js";
var assert = require('chai').assert;

describe('tagsCollector tests', function() {

    it('process with data test', function () {
        let collector = new tagsCollector("mocTags");

        let reportData = new Object();

        collector.process(reportData);

        assert.equal("mocTags", reportData.tags);
    });

    it('process without data test', function () {
        let collector = new tagsCollector();

        let reportData = new Object();

        collector.process(reportData);

        assert.equal(undefined, reportData.tags);
    });
});