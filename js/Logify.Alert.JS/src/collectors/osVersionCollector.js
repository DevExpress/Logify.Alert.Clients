'use strict'
import collectorBase from "./collectorBase.js";

export default class osVersionCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }
    
    process(win, report) {
        super.process(win, report);
        
        if(report.os === undefined)
            report.os = new Object();
        
        report.os.platform = win.navigator.platform;
    }
}

