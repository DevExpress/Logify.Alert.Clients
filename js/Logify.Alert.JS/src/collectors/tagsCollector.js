'use strict';
import collectorBase from "./collectorBase.js";

export default class tagsCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report) {
        super.process(win, report);

        if((this.owner == null) || (this.owner == undefined))
            return;

        if(this.owner.tags != undefined)
            report.tags = this.owner.tags;
    }
}