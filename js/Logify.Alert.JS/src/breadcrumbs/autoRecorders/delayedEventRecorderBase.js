'use strict';

import eventRecorderBase from './eventRecorderBase.js'

export default class delayedEventRecorderBase extends eventRecorderBase {
    constructor() {
        super();

        this.delayListening = false;
        this.lastArguments = [];
        this._timeOut = 50;
        this._timeOutId;
    }

    collectBreadcrumb(event, eventCallback) {
        if (this.delayListening === true)
            this.delayedListener(event, eventCallback);
        else
            super.collectBreadcrumb(event, eventCallback);
    }

    delayedListener(event, eventCallback) {
        if (this._owner.collectBreadcrumbs) {
            if (this._timeOutId != undefined) {
                clearTimeout(this._timeOutId);
            }
            const collectFunction = super.collectBreadcrumb;
            this.lastArguments = [
                event,
                eventCallback
            ];
            this._timeOutId = setTimeout(function () {
                collectFunction.apply(this, [event, eventCallback]);
                this.lastArguments = [];
            }.bind(this), this._timeOut);
        }
    }

    forceExecute() {
        if (this.lastArguments && this.lastArguments.length > 0) {
            clearTimeout(this._timeOutId);
            super.collectBreadcrumb(this.lastArguments[0], this.lastArguments[1]);
            this.lastArguments = [];
        }
    }
}