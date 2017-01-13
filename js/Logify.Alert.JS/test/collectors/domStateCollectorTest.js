import domStateCollector from "../../src/collectors/domStateCollector.js";
var assert = require('chai').assert;

describe('domStateCollector tests', function() {

    it('full info test', function () {
        let reportData = new Object();
        let owner = new Object();
        owner.collectInputs = true;

        let collector = new domStateCollector(owner);
        let win = getWin(true, true, true, true);
        collector.process(win, reportData);

        assert.equal("mocId", reportData.domState.activeElementId);
        assert.equal("mocTagName", reportData.domState.activeElementTagName);
        assert.equal("mocElementScrollTop", reportData.domState.activeElementScrollTop);
        assert.equal("mocBodyScrollTop", reportData.domState.bodyScrollTop);
        assert.equal("mocLocation", reportData.domState.location);
        assert.equal(true, reportData.domState.isInsideIFrame);

        assert.equal(2, reportData.domState.inputs.length);
        assert.equal("name1", reportData.domState.inputs[0].name);
        assert.equal("1", reportData.domState.inputs[0].id);
        assert.equal("text", reportData.domState.inputs[0].type);
        assert.equal("value1", reportData.domState.inputs[0].value);
        assert.equal("name3", reportData.domState.inputs[1].name);
        assert.equal("3", reportData.domState.inputs[1].id);
        assert.equal("date", reportData.domState.inputs[1].type);
        assert.equal("value3", reportData.domState.inputs[1].value);
    });

    it('only active element test', function () {
        let reportData = new Object();
        let owner = new Object();
        owner.collectInputs = false;

        let collector = new domStateCollector(owner);
        let win = getWin(true, false, false, false);
        collector.process(win, reportData);

        assert.equal("mocId", reportData.domState.activeElementId);
        assert.equal("mocTagName", reportData.domState.activeElementTagName);
        assert.equal("mocElementScrollTop", reportData.domState.activeElementScrollTop);
        assert.equal(undefined, reportData.domState.bodyScrollTop);
        assert.equal(undefined, reportData.domState.location);
        assert.equal(false, reportData.domState.isInsideIFrame);

        assert.equal(undefined, reportData.domState.inputs);
    });

    it('only body test', function () {
        let reportData = new Object();

        let collector = new domStateCollector();
        let win = getWin(false, true, false, false);
        collector.process(win, reportData);

        assert.equal(undefined, reportData.domState.activeElementId);
        assert.equal(undefined, reportData.domState.activeElementTagName);
        assert.equal(undefined, reportData.domState.activeElementScrollTop);
        assert.equal("mocBodyScrollTop", reportData.domState.bodyScrollTop);
        assert.equal(undefined, reportData.domState.location);
        assert.equal(false, reportData.domState.isInsideIFrame);

        assert.equal(undefined, reportData.domState.inputs);
    });

    it('only location test', function () {
        let reportData = new Object();
        let owner = new Object();
        owner.collectInputs = false;

        let collector = new domStateCollector(owner);
        let win = getWin(false, false, true, false);
        collector.process(win, reportData);

        assert.equal(undefined, reportData.domState.activeElementId);
        assert.equal(undefined, reportData.domState.activeElementTagName);
        assert.equal(undefined, reportData.domState.activeElementScrollTop);
        assert.equal(undefined, reportData.domState.bodyScrollTop);
        assert.equal("mocLocation", reportData.domState.location);
        assert.equal(false, reportData.domState.isInsideIFrame);

        assert.equal(undefined, reportData.domState.inputs);
    });

    function getWin(isActiveElement, isBody, isLocation, isInFrame) {
        let win = new Object();
        win.document = new Object();

        if(isActiveElement) {
            win.document.activeElement = new Object();
            win.document.activeElement.id = "mocId";
            win.document.activeElement.tagName = "mocTagName";
            win.document.activeElement.scrollTop = "mocElementScrollTop";
        }
        
        if(isBody) {
            win.document.body = new Object();
            win.document.body.scrollTop = "mocBodyScrollTop";
        }

        if(isLocation) {
            win.document.location = new Object();
            win.document.location.href = "mocLocation";
        }
        
        if(isInFrame) {
            win.self = 1;
            win.top = 2;
        } else {
            win.self = 1;
            win.top = 1;
        }

        win.document.getElementsByTagName =  function (tagName) {
            if(tagName == 'input') {
                var result = [];
                var inp = { "name":"name1", "id":"1", "type":"text", "value":"value1" };
                result.push(inp);
                inp = { "name":"name2", "id":"2", "type":"password", "value":"value2" };
                result.push(inp);
                inp = { "name":"name3", "id":"3", "type":"date", "value":"value3" };
                result.push(inp);
                return result;
            }
        };

        return win;
    }
});