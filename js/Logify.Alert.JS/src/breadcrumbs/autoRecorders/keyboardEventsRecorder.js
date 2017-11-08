'use strict';

import eventRecorderBase from './eventRecorderBase.js'

export default class keyboardEventsRecorder extends eventRecorderBase {
    constructor() {
        super();

        this._eventNames = {
            "keydown": "keyDown",
            "keypress": "keyPress",
            "keyup": "keyUp"
        }

        this._events = Object.keys(this._eventNames);

        this.category = "keyboard";
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        breadcrumb.customData["scanCode"] = event.code;
        breadcrumb.customData["action"] = event.type.replace("key", "");


        breadcrumb.customData["char"] = this.getChar(event);
        breadcrumb.customData["key"] = super.isSecureElement(event.target) ? "Multiply" : event.code;
    }

    getEventName(eventType) {
        return this._eventNames.hasOwnProperty(eventType) ? this._eventNames[eventType] : eventType;
    }

    getChar(event) {
        if (super.isSecureElement(event.target))
            return "*";
        if (event.which == null) {
            if (event.keyCode < 32) return null;
            return String.fromCharCode(event.keyCode);
        }
        if (event.which != 0 && event.charCode != 0) {
            if (event.which < 32) return null;
            return String.fromCharCode(event.which);
        }
        return null;
    }
}