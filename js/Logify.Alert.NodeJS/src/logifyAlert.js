'use strict';
import nodeCollector from "./collectors/nodeCollector.js";
import customDataCollector from "./collectors/customDataCollector.js";
import nodeReportSender from "./reportSender/nodeReportSender.js";

export default class logifyAlert {
    constructor(apiKey) {
        this._apiKey = apiKey;
        this._handleReports = true;
        this.applicationName = undefined;
        this.applicationVersion = undefined;
        this.userId = undefined;
        this.customData = undefined;
        this.beforeReportException = undefined;
    }

    startHandling() {
        this._handleReports = true;

        process.on('uncaughtException', (error) => {
            if(this._handleReports) {
                this.sendException(error);
            }
        });

        process.on('unhandledRejection', (reason, promise) => {
            if(this._handleReports) {
                this.sendRejection(reason);
            }
        });
    }

    stopHandling() {
        this._handleReports = false;
    }
    
    sendException(error) {
        this.callBeforeReportExceptionCallback();
        let collector = this.createCollector();
        collector.handleException(error);
        this.sendReportCore(collector.reportData);
    }
    
    sendRejection(reason) {
        this.callBeforeReportExceptionCallback();
        let collector = this.createCollector();
        collector.handleRejection(reason);
        this.sendReportCore(collector.reportData);
    }
    
    sendReportCore(reportData) {
        let sender = new nodeReportSender();
        sender.sendReport(this._apiKey, reportData);
    }
    
    createCollector() {
        let collector = new nodeCollector();
        collector.applicationName = this.applicationName;
        collector.applicationVersion = this.applicationVersion;
        collector.userId = this.userId;
        collector.collectors.push(new customDataCollector(this.customData));
        return collector;
    }

    callBeforeReportExceptionCallback() {
        if(this.beforeReportException != undefined) {
            this.customData = this.beforeReportException(this.customData);
        }
    }
}