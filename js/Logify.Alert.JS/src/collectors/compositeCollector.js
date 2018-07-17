'use strict'
import collectorBase from "./collectorBase.js";

export default class compositeCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
        
        this.collectors = [];
    }

    process(win, report, customData) {
        super.process(win, report);

        let collectorsCount = this.collectors.length;
        for(let i = 0; i < collectorsCount; i++) {
            this.collectors[i].process(win, report, customData);
        }
    }
}