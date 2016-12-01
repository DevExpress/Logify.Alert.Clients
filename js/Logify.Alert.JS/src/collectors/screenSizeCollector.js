'use strict'
import collectorBase from "./collectorBase.js";

export default class screenSizeCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }
    
    process(win, report) {
        super.process(win, report);

        if(report.screen === undefined)
            report.screen = new Object();
        
        report.screen.width = win.screen.width;
        report.screen.height = win.screen.height;
    }
}