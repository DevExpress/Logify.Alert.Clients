'use strict'
import collectorBase from "./collectorBase.js";
var process = require('process');

export default class nodeVersionCollector extends collectorBase {
    process(report) {
        super.process(report);

        report.nodeEnvironmentVersions = process.versions;
    }
}