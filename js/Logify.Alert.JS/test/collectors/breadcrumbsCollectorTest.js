import breadcrumbsCollector from "../../src/collectors/breadcrumbsCollector.js";

var assert = require('chai').assert;

describe('breadcrumbsCollector tests', function() {

    it('process with data test', function () {
        let mocOwner = new Object();
        mocOwner._breadcrumbs = ["mocBreadcrumb"];
        
        let collector = new breadcrumbsCollector(mocOwner);
        
        let reportData = new Object();

        collector.process(null, reportData);

        assert.equal(1, reportData.breadcrumbs.length);
        assert.equal("mocBreadcrumb", reportData.breadcrumbs[0]);
    });

    it('process without data test', function () {
        let mocOwner = new Object();
        let collector = new breadcrumbsCollector(mocOwner);

        let reportData = new Object();

        collector.process(null, reportData);

        assert.equal(undefined, reportData.breadcrumbs);
    });
});