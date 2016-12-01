'use strict';

var request = require('request');

export default class nodeReportSender {
    sendReport(apiKey, reportData) {
        this._apiKey = apiKey;
        this._sendData = reportData;
        this._sendingAttamptCount = 0;
        this.sendReportCore();
    }

    sendReportCore() {
        let callback =  function (error, response, body){
            if((response != null) && (response != undefined) && (response.statusCode != 200)) {
                if(callback.owner._sendingAttamptCount < 3) {
                    callback.owner._sendingAttamptCount++;
                    callback.owner.sendReportCore();
                }
            }
        };
        callback.owner = this;

        request({
            url: "https://logify.devexpress.com/api/report/newreport",
            method: "POST",
            json: true,
            body: this._sendData,
            headers: {
                "Authorization": "amx " + this._apiKey,
                "Content-Type": "application/json; charset=UTF-8"
            }
        }, callback);
    }
}