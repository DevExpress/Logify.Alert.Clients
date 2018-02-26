'use strict';
import collectorBase from "./collectorBase.js";

export default class siteDataCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }
    
    process(win, report) {
        super.process(win, report);

        if(report.siteData === undefined)
            report.siteData = new Object();

        if((this.owner == null) || (this.owner == undefined))
            return;
            
        if(this.owner.collectCookies)
            report.siteData.cookie = win.document.cookie;

        if(this.owner.collectLocalStorage)
            report.siteData.localStorage = this.owner.securityUtil.maskedObject(win.localStorage);

        if(this.owner.collectSessionStorage)
            report.siteData.sessionStorage = this.owner.securityUtil.maskedObject(win.sessionStorage);
    }
}