'use strict';
import collectorBase from "./collectorBase.js";

export default class browserVersionCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report) {
        super.process(win, report);

        var scripts = win.document.getElementsByTagName('script');

        if(scripts == null || scripts == undefined)
            return;

        var scriptsCount = scripts.length;
        if(scriptsCount == undefined || scriptsCount == 0)
            return;

        var result = [];
        for(var i = 0; i < scriptsCount; i++) {
            if(scripts[i].innerHTML == "")
                result.push(scripts[i].outerHTML);
        }

        if(result.length == 0)
            return;

        report.scripts = result;
    }
}
