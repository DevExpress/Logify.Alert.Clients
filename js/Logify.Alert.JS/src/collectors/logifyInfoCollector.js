'use strict';
import collectorBase from "./collectorBase.js";

export default class logifyInfoCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }
    
    process(win, report) {
        super.process(win, report);

        report.clientVersion = "15.2.11";
        report.devPlatform = "js";
        report.logifyProtocolVersion = "1.0";
    }
}