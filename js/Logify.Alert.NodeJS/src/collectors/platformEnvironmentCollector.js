'use strict'
import collectorBase from "./collectorBase.js";
var os = require('os');

export default class platformEnvironmentCollector extends collectorBase {
    process(report) {
        super.process(report);

        if(report.platformEnvironment === undefined)
            report.platformEnvironment = new Object();

        report.platformEnvironment.platform = os.platform();
        report.platformEnvironment.cpus = this.getCpus();
        report.platformEnvironment.architecture = os.arch();
        report.platformEnvironment.freeMemory = os.freemem().toString();
        report.platformEnvironment.totalMemory = os.totalmem().toString();
        report.platformEnvironment.release = os.release();
    }
    
    getCpus() {
        let cpus = os.cpus();
        let result = [];
        for(let i = 0; i < cpus.length; i++) {
            result.push({ "model": cpus[i].model, "speed": cpus[i].speed.toString() });
        }
        return result;
    }
}