'use strict'
import collectorBase from "./collectorBase.js";

export default class tagsCollector extends collectorBase {
    constructor(_tags) {
        super();
        this.tags = _tags;
    }
    process(report) {
        super.process(report);

        if(this.tags != undefined)
            report.tags = this.tags;
    }
}