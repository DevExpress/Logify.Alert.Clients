'use strict';

import eventRecorderBase from './eventRecorderBase.js'

export default class windowEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "resize"
        ];
        this.category = "window";
    }
    parseEventData(event, breadcrumb) {
        breadcrumb.customData["newWidth"] = event.target.innerWidth;
        breadcrumb.customData["newHeight"] = event.target.innerHeight;
    }
}