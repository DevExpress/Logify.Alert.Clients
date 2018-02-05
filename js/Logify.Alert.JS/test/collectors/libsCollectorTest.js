import libsCollector from "../../src/collectors/libsCollector.js";
var assert = require('chai').assert;

describe('libsCollector tests', function() {
    it('check missing lib', function () {
        let owner = new Object();
        let collector = new libsCollector(owner);
        let reportData = new Object();
        let win = getWin("dx", null);

        collector.process(win, reportData);

        assert.equal(reportData.libs["dxASPx"], undefined);
    });

    it('check existing lib with missing property', function () {
        let owner = new Object();
        let collector = new libsCollector(owner);
        let reportData = new Object();
        let win = getWin("ASPx", null);

        collector.process(win, reportData);

        assert.notEqual(reportData.libs["dxASPx"], undefined);
        assert.equal(reportData.libs["dxASPx"]["versionInfo"], undefined);
    });

    it('check existing lib with existing property', function () {
        let owner = new Object();
        let collector = new libsCollector(owner);
        let reportData = new Object();
        const existingLibName = "ASPx";
        const existingPropertyName = "VersionInfo";
        let win = getWin(existingLibName, existingPropertyName);

        collector.process(win, reportData);

        assert.notEqual(reportData.libs["dxASPx"], undefined);
        assert.notEqual(reportData.libs["dxASPx"]["versionInfo"], undefined);
    });

    function getWin(libName, libPropertyName) {
        let win = new Object();

        if(libName) {
            win[libName] = new Object();

            if(libPropertyName) {
                win[libName][libPropertyName] = new Object();
            }
        }

        return win;
    }
});