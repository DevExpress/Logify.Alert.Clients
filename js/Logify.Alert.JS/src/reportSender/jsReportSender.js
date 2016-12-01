'use strict';

export default class jsReportSender {

    sendReport(apiKey, reportData) {
        this._apiKey = apiKey;
        this._sendData = JSON.stringify(reportData);
        this._sendingAttamptCount = 0;
        this.sendReportCore();
    }

    sendReportCore() {
        let xhr = this.createCORSRequest("https://logify.devexpress.com/api/report/newreport");
        if(xhr == null)
            return false;

        xhr.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
        xhr.setRequestHeader("Authorization", "amx " + this._apiKey);

        let callback = function() {
            if(this.readyState != 4)
                return;

            callback.owner._sendingAttamptCount++;

            if(this.status != 200) {
                if(callback.owner._sendingAttamptCount < 3)
                    callback.owner.sendReportCore();
            }
        };
        callback.owner = this;

        xhr.onreadystatechange = callback;

        xhr.send(this._sendData);
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