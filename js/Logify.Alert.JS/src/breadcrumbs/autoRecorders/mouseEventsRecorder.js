'use strict';

import eventRecorderBase from './eventRecorderBase.js'

export default class mouseEventsRecorder extends eventRecorderBase {
    constructor() {
        super();

        this._eventNames = {
            "click": "mouseClick",
            "auxclick": "mouseClick",
            "dblclick": "mouseDoubleClick"
        }

        this.category = "mouse";

        this._events = Object.keys(this._eventNames);
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        let action;
        switch (event.button) {
            case 2:
                action = "right";
                break;
            case 1:
                action = "middle";
                break;
            case 0:
            default:
                action = "left";
                break;
        }
        breadcrumb.customData["action"] = action;
        breadcrumb.customData["x"] = event.x;
        breadcrumb.customData["y"] = event.y;
    }

    getEventName(eventType) {
        return this._eventNames.hasOwnProperty(eventType) ? this._eventNames[eventType] : eventType;
    }
}