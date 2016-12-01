'use strict'
import collectorBase from "./collectorBase.js";

export default class compositeCollector extends collectorBase {
    constructor() {
        super();

        this.collectors = [];
    }

    process(report) {
        super.process(report);

        let collectorsCount = this.collectors.length;
        for(let i = 0; i < collectorsCount; i++) {
            this.collectors[i].process(report);
        }
    }
}