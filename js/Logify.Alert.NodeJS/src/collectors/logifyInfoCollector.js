'use strict';
import collectorBase from "./collectorBase.js";

export default class logifyInfoCollector extends collectorBase {
    process(report) {
        super.process(report);

        report.clientVersion = "15.2.11";
        report.devPlatform = "node.js";
        report.logifyProtocolVersion = "1.0";
    }
}