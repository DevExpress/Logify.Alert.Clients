'use strict'
import compositeCollector from "./compositeCollector.js";
import browserVersionCollector from "./browserVersionCollector.js";
import osVersionCollector from "./osVersionCollector.js";
import screenSizeCollector from "./screenSizeCollector.js";
import windowSizeCollector from "./windowSizeCollector.js";
import siteDataCollector from "./siteDataCollector.js";
import logifyInfoCollector from "./logifyInfoCollector.js";
import customDataCollector from "./customDataCollector.js";
import tagsCollector from "./tagsCollector.js";
import scriptsCollector from "./scriptsCollector.js";
import metasCollector from "./metasCollector";
import domStateCollector from "./domStateCollector.js";
import breadcrumbsCollector from "./breadcrumbsCollector.js";
import libsCollector from "./libsCollector";

export default class jsCollector extends compositeCollector {
    constructor(_owner, isTest = false) {
        super(_owner);

        this.applicationName = undefined;
        this.applicationVersion = undefined;
        this.userId = undefined;
        this.securityUtil = undefined;

        this._reportData = new Object();

        this.breadcrumbsCollector = new breadcrumbsCollector(this.owner);
        
        this.collectors.push(new browserVersionCollector(this.owner));
        this.collectors.push(new osVersionCollector(this.owner));
        this.collectors.push(new screenSizeCollector(this.owner));
        this.collectors.push(new windowSizeCollector(this.owner));
        this.collectors.push(new siteDataCollector(this.owner));
        this.collectors.push(new libsCollector(this.owner));
        this.collectors.push(new logifyInfoCollector(this.owner));
        this.collectors.push(new customDataCollector(this.owner));
        this.collectors.push(new tagsCollector(this.owner));
        this.collectors.push(new scriptsCollector(this.owner));
        this.collectors.push(new metasCollector(this.owner));
        this.collectors.push(new domStateCollector(this.owner));
        this.collectors.push(this.breadcrumbsCollector);

        if(!isTest) {
            this._window = window;
            this._document = document;
        }
    }

    process(win, report, customData) {
        super.process(win, report, customData);
    }

    collectRejectionData(reason, promise) {
        this.fillRejectionData(reason);
        this.fillAppData(this.getScriptUrl(this._document));
        this.process(this._window, this._reportData);
    }
    
    collectErrorData(errorMsg, url, lineNumber, column, errorObj, customData) {
        this.fillErrorData(errorMsg, url, lineNumber,column, errorObj);
        this.fillAppData(url ? url : this.getLocation(this._document));
        this.process(this._window, this._reportData, customData);
    }

    getApplicationName(url) {
        return (this.applicationName == undefined) ? url : this.applicationName;
    }

    getApplicationVersion() {
        return (this.applicationVersion == undefined) ? "" : this.applicationVersion;
    }

    getUserId() {
        return (this.userId == undefined) ? "" : this.userId;
    }

    getLocation(doc) {
        return doc.location ? doc.location.href : "";
    }

    getScriptUrl(doc) {
        var scripts = doc.getElementsByTagName('script'),
            script = scripts[scripts.length - 1];
        
        if (script.getAttribute.length !== undefined) {
            return script.getAttribute('src')
        }

        return script.getAttribute('src', 2)
    }

    fillRejectionData(reason) {
        if(this._reportData.rejection === undefined)
            this._reportData.rejection = new Object();
        if(reason.message != undefined) {
            this._reportData.rejection.error = new Object();
            this._reportData.rejection.error.message = reason.message;
            if(reason.stack != undefined) {
                this._reportData.rejection.error.stack = reason.stack;
            }
        } else {
            this._reportData.rejection.reason = reason;
        }
    }

    fillErrorData(errorMsg, url, lineNumber, column, errorObj) {
        if(this._reportData.error === undefined)
            this._reportData.error = new Object();

        this._reportData.error.message = errorMsg;
        this._reportData.error.url = url;
        this._reportData.error.lineNumber = lineNumber;
        this._reportData.error.column = column;
        if((errorObj != undefined) && (errorObj.stack != undefined)) {
            this._reportData.error.stack = errorObj.stack;
        }
    }

    fillAppData(url) {
        if(this._reportData.logifyApp === undefined)
            this._reportData.logifyApp = new Object();

        this._reportData.logifyApp.name = this.getApplicationName(url);
        this._reportData.logifyApp.version = this.getApplicationVersion();
        this._reportData.logifyApp.userId = this.getUserId();
    }

    get reportData() {
        return this._reportData;
    }
}
