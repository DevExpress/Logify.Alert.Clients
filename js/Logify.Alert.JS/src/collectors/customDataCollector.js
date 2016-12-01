'use strict';
import collectorBase from "./collectorBase.js";

export default class customDataCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report) {
        super.process(win, report);

        if((this.owner == null) || (this.owner == undefined))
            return;

        if(this.owner.customData != undefined)
            report.customData = this.owner.customData;
    }
}