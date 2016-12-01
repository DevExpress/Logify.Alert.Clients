'use strict'
import collectorBase from "./collectorBase.js";

export default class windowSizeCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }
    
    process(win, report) {
        super.process(win, report);

        if(report.window === undefined)
            report.window = new Object();

        report.window.width = win.innerWidth;
        report.window.height = win.innerHeight;
    }
}