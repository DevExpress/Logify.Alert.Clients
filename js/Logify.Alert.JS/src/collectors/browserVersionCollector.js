'use strict';
import collectorBase from "./collectorBase.js";

export default class browserVersionCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }
    
    process(win, report) {
        super.process(win, report);

        if(report.browser === undefined)
            report.browser = new Object();

        report.browser.appCodeName = win.navigator.appCodeName;
        report.browser.userAgent = win.navigator.userAgent;
        report.browser.appName = win.navigator.appName;
        report.browser.appVersion = win.navigator.appVersion;
        report.browser.language = win.navigator.language;
    }
}