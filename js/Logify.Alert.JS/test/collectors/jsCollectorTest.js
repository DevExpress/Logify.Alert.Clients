import jsCollector from "../../src/collectors/jsCollector.js";
var assert = require('chai').assert;

describe('jsCollector tests', function() {
    it('check collect rejection string test', function () {
        let collector = createCollector();
        let win = createMocWin();
        let doc = createMocDoc();
        collector._window = win;
        collector._document = doc;
        collector.collectRejectionData("mocReason");
        checkRejectionData(collector, null);
    });

    it('check collect rejection with error with stack test', function () {
        let collector = createCollector();
        let win = createMocWin();
        let doc = createMocDoc();
        collector._window = win;
        collector._document = doc;
        let err = new Object();
        err.message = "mocMessage";
        err.stack = "mocStack";
        collector.collectRejectionData(err);
        checkRejectionData(collector, err);
    });

    it('check collect rejection with error without stack test', function () {
        let collector = createCollector();
        let win = createMocWin();
        let doc = createMocDoc();
        collector._window = win;
        collector._document = doc;
        let err = new Object();
        err.message = "mocMessage";
        collector.collectRejectionData(err);
        checkRejectionData(collector, err);
    });
    
    it('check all data test', function () {
        let collector = createCollector();
        let win = createMocWin();
        collector._window = win;
        let errorObject = new Object();
        errorObject.stack = "mocStack";
        collector.collectErrorData("mocErrorMsg", "mocUrl", "mocLineNumber", "mocColumn", errorObject);
        checkData(collector, true);
    });

    it('check data without stack test', function () {
        let collector = createCollector();
        let win = createMocWin();
        collector._window = win;
        let errorObject = new Object();
        collector.collectErrorData("mocErrorMsg", "mocUrl", "mocLineNumber", "mocColumn", errorObject);
        checkData(collector, false);
    });

    it('check data without errorObj test', function () {
        let collector = createCollector();
        let win = createMocWin();
        collector._window = win;
        collector.collectErrorData("mocErrorMsg", "mocUrl", "mocLineNumber", "mocColumn", undefined);
        checkData(collector, false);
    });

    it('check app information without setting test', function () {
        let collector = createCollector();
        let win = createMocWin();
        collector._window = win;
        collector.collectErrorData("mocErrorMsg", "mocUrl", "mocLineNumber", "mocColumn", undefined);
        assert.equal("", collector.reportData.logifyApp.version);
        assert.equal("", collector.reportData.logifyApp.userId);
        assert.equal("mocUrl", collector.reportData.logifyApp.name);
    });

    it('check app information with setting test', function () {
        let collector = createCollector();
        let win = createMocWin();
        collector._window = win;
        collector.applicationName = "mocAppName";
        collector.applicationVersion = "mocVersion";
        collector.userId = "mocUserId";
        collector.collectErrorData("mocErrorMsg", "mocUrl", "mocLineNumber", "mocColumn", undefined);
        assert.equal("mocVersion", collector.reportData.logifyApp.version);
        assert.equal("mocAppName", collector.reportData.logifyApp.name);
        assert.equal("mocUserId", collector.reportData.logifyApp.userId);
    });

    function createMocWin() {
        let win = new Object();
        win.navigator = new Object();
        win.navigator.appCodeName = "mocAppCodeName";
        win.navigator.userAgent = "mocUserAgent";
        win.navigator.appName = "mocAppName";
        win.navigator.appVersion = "mocAppVersion";
        win.navigator.platform = "mocPlatform";

        win.screen = new Object();
        win.screen.width = "mocWidth";
        win.screen.height = "mocHeight";

        win.innerHeight = "mocHeight";
        win.innerWidth = "mocWidth";

        win.localStorage = "mocLocalStorage";
        win.sessionStorage = "mocSessionStorage";

        win.document = new Object();
        win.document.cookie = "mocCookie";

        return win;
    }
    
    function createCollector() {
        let owner = new Object();
        owner.collectLocalStorage = true;
        owner.collectSessionStorage = true;
        owner.collectCookies = true;
        owner.customData = "mocCustomData";
        return new jsCollector(owner, true);
    }

    function createMocDoc() {
        return new mocDoc();
    }

    function checkRejectionData(collector, error) {
        if(error == null) {
            assert.equal("mocReason", collector.reportData.rejection.reason);
        } else {
            assert.equal(error.message, collector.reportData.rejection.error.message);
            if(error.stack != undefined) {
                assert.equal(error.stack, collector.reportData.rejection.error.stack);
            } else {
                assert.equal(undefined, collector.reportData.rejection.error.stack);
            }
        }
        assert.equal("mocScript", collector.reportData.logifyApp.name);
    }
    
    function checkData(collector, hasStack) {
        assert.equal(7, collector.collectors.length);

        assert.equal("mocErrorMsg", collector.reportData.error.message);
        assert.equal("mocUrl", collector.reportData.error.url);
        assert.equal("mocLineNumber", collector.reportData.error.lineNumber);
        assert.equal("mocColumn", collector.reportData.error.column);
        if(hasStack) {
            assert.equal("mocStack", collector.reportData.error.stack);
        } else {
            assert.equal(undefined, collector.reportData.error.stack);
        }

        assert.equal("mocLocalStorage", collector.reportData.siteData.localStorage);
        assert.equal("mocSessionStorage", collector.reportData.siteData.sessionStorage);
        assert.equal("mocCookie", collector.reportData.siteData.cookie);

        assert.equal("mocAppCodeName", collector.reportData.browser.appCodeName);
        assert.equal("mocUserAgent", collector.reportData.browser.userAgent);
        assert.equal("mocAppName", collector.reportData.browser.appName);
        assert.equal("mocAppVersion", collector.reportData.browser.appVersion);

        assert.equal("mocPlatform", collector.reportData.os.platform);

        assert.equal("mocWidth", collector.reportData.screen.width);
        assert.equal("mocHeight", collector.reportData.screen.height);

        assert.equal("mocHeight", collector.reportData.window.height);
        assert.equal("mocWidth", collector.reportData.window.width);

        assert.equal("15.2.11", collector.reportData.clientVersion);
        assert.equal("js", collector.reportData.devPlatform);
        assert.equal("1.0", collector.reportData.logifyProtocolVersion);

        assert.equal("mocCustomData", collector.reportData.customData);
    }
});

class mocDoc {
    getElementsByTagName(tagName) {
        let result = [];
        let item = {};
        item.getAttribute = function(p1, p2){
            return "mocScript";
        };
        result.push(item);
        return result;
    }
}