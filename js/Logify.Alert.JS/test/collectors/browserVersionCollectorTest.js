import browserVersionCollector from "../../src/collectors/browserVersionCollector.js";
var assert = require('chai').assert;

describe('browserVersionCollector tests', function() {
    
    it('process test', function () {
        let reportData = new Object();

        let collector = new browserVersionCollector();
        let win = new Object();
        win.navigator = new Object();
        win.navigator.appCodeName = "mocAppCodeName";
        win.navigator.userAgent = "mocUserAgent";
        win.navigator.appName = "mocAppName";
        win.navigator.appVersion = "mocAppVersion";
        win.navigator.language = "mocLanguage";

        collector.process(win, reportData);

        assert.equal("mocAppCodeName", reportData.browser.appCodeName);
        assert.equal("mocUserAgent", reportData.browser.userAgent);
        assert.equal("mocAppName", reportData.browser.appName);
        assert.equal("mocAppVersion", reportData.browser.appVersion);
        assert.equal("mocLanguage", reportData.browser.language);
    });
});
