import siteDataCollector from "../../src/collectors/siteDataCollector.js";
var assert = require('chai').assert;

describe('siteDataCollector tests', function() {

    it('process test with data', function () {
        let reportData = new Object();
        let owner = new Object();
        owner.collectLocalStorage = true;
        owner.collectSessionStorage = true;
        owner.collectCookies = true;

        let collector = new siteDataCollector(owner);
        let win = new Object();
        win.document = new Object();
        win.document.cookie = "mocCookie";
        
        win.localStorage = "mocLocalStorage";
        win.sessionStorage = "mocSessionStorage";

        collector.process(win, reportData);

        assert.equal("mocCookie", reportData.siteData.cookie);
        assert.equal("mocLocalStorage", reportData.siteData.localStorage);
        assert.equal("mocSessionStorage", reportData.siteData.sessionStorage);

        owner.collectLocalStorage = false;
        collector = new siteDataCollector(owner);
        reportData = new Object();
        collector.process(win, reportData);
        assert.equal("mocCookie", reportData.siteData.cookie);
        assert.equal("mocSessionStorage", reportData.siteData.sessionStorage);
        assert.equal(undefined, reportData.siteData.localStorage);

        owner.collectSessionStorage = false;
        collector = new siteDataCollector(owner);
        reportData = new Object();
        collector.process(win, reportData);
        assert.equal("mocCookie", reportData.siteData.cookie);
        assert.equal(undefined, reportData.siteData.sessionStorage);
        assert.equal(undefined, reportData.siteData.localStorage);

        owner.collectCookies = false;
        collector = new siteDataCollector(owner);
        reportData = new Object();
        collector.process(win, reportData);
        assert.equal(undefined, reportData.siteData.cookie);
        assert.equal(undefined, reportData.siteData.sessionStorage);
        assert.equal(undefined, reportData.siteData.localStorage);
    });
});