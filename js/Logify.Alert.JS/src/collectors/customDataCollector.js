'use strict';
import collectorBase from "./collectorBase.js";

export default class customDataCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report, additionalCustomData) {
        super.process(win, report);

        if((this.owner == null) || (this.owner == undefined))
            return;

        report.customData = this.mergeCustomData(this.owner.customData, additionalCustomData);   
    }
    mergeCustomData(reportCustomData, additionalCustomData) {
        if (!reportCustomData)
            reportCustomData = {};
        if (typeof additionalCustomData === "object") {
            return Object.assign({}, reportCustomData, additionalCustomData);    
        } else if (typeof additionalCustomData === "string" || typeof additionalCustomData === "number") {
            return Object.assign({}, reportCustomData, { additionalData: additionalCustomData });    
        }
        return reportCustomData;
    }
}