import tagsCollector from "../../src/collectors/tagsCollector.js";
var assert = require('chai').assert;

describe('tagsCollector tests', function() {

    it('process with data test', function () {
        let mocOwner = new Object();
        mocOwner.tags = "mocTags";
        
        let collector = new tagsCollector(mocOwner);
        
        let reportData = new Object();

        collector.process(null, reportData);

        assert.equal("mocTags", reportData.tags);
    });

    it('process without data test', function () {
        let mocOwner = new Object();
        let collector = new tagsCollector(mocOwner);

        let reportData = new Object();

        collector.process(null, reportData);

        assert.equal(undefined, reportData.tags);
    });
});