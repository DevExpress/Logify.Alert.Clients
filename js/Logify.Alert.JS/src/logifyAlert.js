'use strict';
import jsCollector from "./collectors/jsCollector.js";
import jsReportSender from "./reportSender/jsReportSender.js";

class logifyAlert {
    constructor(apiKey) {
        this._apiKey = apiKey;
        this._handleReports = true;
        this.applicationName = undefined;
        this.applicationVersion = undefined;
        this.userId = undefined;
        this.customData = undefined;
        this.collectLocalStorage = false;
        this.collectSessionStorage = true;
        this.collectCookies = true;
        this.beforeReportException = undefined;
    }

    stopHandling() {
        this._handleReports = false;
    }

    startHandling() {
        this._handleReports = true;
        
        window.onerror = (errorMsg, url, lineNumber, column, errorObj) => {
            if(this._handleReports) {
                this.sendExceptionCore(errorMsg, url, lineNumber, column, errorObj, this);
            }

            return false;
        };

        window.addEventListener('unhandledrejection', event => {
            if(this._handleReports) {
                this.sendRejection(event.reason, event.promise);
            }
        });
    }

    sendException(errorMsg, url, lineNumber, column, errorObj) {
        this.sendExceptionCore(errorMsg, url, lineNumber, column, errorObj, this);
    }

    sendExceptionCore(errorMsg, url, lineNumber, column, errorObj, owner) {
        this.callBeforeReportExceptionCallback();
        let collector = this.createCollector(owner);
        collector.collectErrorData(errorMsg, url, lineNumber, column, errorObj);

        let sender = new jsReportSender();
        sender.sendReport(owner._apiKey, collector.reportData);
    }

    sendRejection(reason, promise) {
        this.callBeforeReportExceptionCallback();
        let collector = this.createCollector(this);
        collector.collectRejectionData(reason, promise);

        let sender = new jsReportSender();
        sender.sendReport(this._apiKey, collector.reportData);
    }

    createCollector(owner) {
        let collector = new jsCollector(owner);
        collector.applicationName = owner.applicationName;
        collector.applicationVersion = owner.applicationVersion;
        collector.userId = owner.userId;
        return collector;
    }

    callBeforeReportExceptionCallback() {
        if(this.beforeReportException != undefined) {
            this.customData = this.beforeReportException(this.customData);
        }
    }
}

window["logifyAlert"] = logifyAlert;