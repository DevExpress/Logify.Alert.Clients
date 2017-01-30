'use strict';
import collectorBase from "./collectorBase.js";

export default class domStateCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report) {
        super.process(win, report);

        if(report.domState === undefined)
            report.domState = new Object();

        var activeElement = win.document.activeElement;
        if(activeElement) {
            report.domState.activeElementId = activeElement.id;
            report.domState.activeElementTagName = activeElement.tagName;
            report.domState.activeElementScrollTop = activeElement.scrollTop;
        }

        var body = win.document.body;
        if(body) {
            report.domState.bodyScrollTop = body.scrollTop;
        }

        if(win.document.location) {
            report.domState.location = win.document.location.href;
        }
        
        if(win.document.readyState) {
            report.domState.readyState = win.document.readyState;
        }

        try {
            report.domState.isInsideIFrame = (win.self !== win.top);
        } catch(e) { }

        if((this.owner != null) && (this.owner != undefined)) {
            if(this.owner.collectInputs)
                this.collectInputs(win, report);
        }
    }

    collectInputs(win, report) {
        var inputs = win.document.getElementsByTagName('input');
        var inputsCount = inputs.length;
        var result = [];
        for(var i = 0; i < inputsCount; i++) {
            var currentInput = inputs[i];
            if(currentInput.type != 'password') {
                var inputInfo = new Object();
                inputInfo.name = currentInput.name;
                inputInfo.id = currentInput.id;
                inputInfo.type = currentInput.type;
                inputInfo.value = currentInput.value;

                result.push(inputInfo);
            }
        }

        report.domState.inputs = result;
    }
}