'use strict';

var request = require('request');

export default class nodeReportSender {
    sendReport(apiKey, reportData, sendCallback) {
        this._apiKey = apiKey;
        this._sendData = reportData;
        this._sendCallback = sendCallback;
        this._sendingAttemptCount = 0;
        this.sendReportCore();
    }

    callSendReportCallback(e) {
        if(this._sendCallback != undefined)
            this._sendCallback(e)
    }

    sendReportCore() {
        let callback =  function (error, response, body){
            if(error) {
                this.callSendReportCallback(error);
                return;
            }
            if((response != null) && (response != undefined) && (response.statusCode != 200)) {
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

        request({
            url: "https://logify.devexpress.com/api/report/newreport",
            method: "POST",
            json: true,
            body: this._sendData,
            headers: {
                "Authorization": "amx " + this._apiKey,
                "Content-Type": "application/json; charset=UTF-8"
            }
        }, callback.bind(this));
    }
}