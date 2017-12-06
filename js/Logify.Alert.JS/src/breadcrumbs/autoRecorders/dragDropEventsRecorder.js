'use strict';

import eventRecorderBase from './eventRecorderBase.js'

export default class dragDropEventsListener extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "drag",
            "drop"
        ];
        this.category = "dragDrop";
        this.delayListening = true;
        this._lastEvent = null;
    }

    collectBreadcrumb(event, eventCallback) {
        let recordLastEvent = true;
        if (!this._lastEvent) {
            super.collectBreadcrumb(event, eventCallback);
        } else {
            if (this._lastEvent.type != event.type) {
                super.collectBreadcrumb(this._lastEvent, eventCallback);
                super.collectBreadcrumb(event, eventCallback);
                this._lastEvent = null;
                recordLastEvent = false;
            }
        }
        if (recordLastEvent) {
            this._lastEvent = event;
        }
    }
    forceExecute() {
        if (this._lastEvent) {
            super.collectBreadcrumb(this._lastEvent, eventCallback);
        }
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        breadcrumb.customData["offsetX"] = event.offsetX;
        breadcrumb.customData["offsetY"] = event.offsetY;
    }
}