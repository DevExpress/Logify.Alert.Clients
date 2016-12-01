'use strict'
import collectorBase from "./collectorBase.js";

export default class customDataCollector extends collectorBase {
    constructor(_customData) {
        super();
        this.customData = _customData;
    }
    process(report) {
        super.process(report);

        if(this.customData != undefined)
            report.customData = this.customData;
    }
}