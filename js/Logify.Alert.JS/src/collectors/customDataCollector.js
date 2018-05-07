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

        if(this.owner.customData != undefined)
            report.customData = this.owner.customData;
        if (additionalCustomData)    
            report.customData = this.mergeCustomData(report.customData, additionalCustomData);   
    }
    mergeCustomData(reportCustomData, additionalCustomData) {
        if (typeof additionalCustomData === "object") {
            if (!reportCustomData)
                reportCustomData = {};
            return Object.assign({}, reportCustomData, additionalCustomData);    
        } else if (typeof additionalCustomData === "string" || typeof additionalCustomData === "number") {
            return Object.assign({}, reportCustomData, { additionalData: additionalCustomData });    
        }
        return reportCustomData;
    }
}