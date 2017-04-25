'use strict';

export default class jsReportSender {

    sendReport(apiKey, reportData, sendCallback) {
        this._apiKey = apiKey;
        this._sendData = JSON.stringify(reportData);
        this._sendingAttemptCount = 0;
        this._sendCallback = sendCallback;
        this.sendReportCore();
    }

    callSendReportCallback(e) {
        if(this._sendCallback != undefined)
            this._sendCallback(e)
    }

    sendReportCore() {
        this._xhr = this.createCORSRequest("https://logify.devexpress.com/api/report/newreport");
        if(this._xhr == null)
            return false;

        this._xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
        this._xhr.setRequestHeader("Authorization", "amx " + this._apiKey);

        let callback = function() {
            if(this._xhr.readyState != 4)
                return;

            if(this._xhr.status != 200) {
                if(this._sendingAttemptCount < 3) {
                    this._sendingAttemptCount++;
                    this.sendReportCore();
                } else {
                    this.callSendReportCallback("The report was not sent");
                }
            } else {
                this.callSendReportCallback();
            }
        };

        this._xhr.onreadystatechange = callback.bind(this);

        this._xhr.send(this._sendData);
    }

    createCORSRequest(url) {
        let xhr = new XMLHttpRequest();
        if("onload" in xhr) {
            xhr.open("POST", url, true);
        } else if(typeof XDomainRequest != "undefined") {
            xhr = new XDomainREquest();
            xhr.open("POST", url);
        } else {
            xhr = null;
        }
        return xhr;
    }
}