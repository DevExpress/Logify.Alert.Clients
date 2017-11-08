'use strict';

import delayedEventRecorderBase from './delayedEventRecorderBase.js'

export default class dragDropEventsListener extends delayedEventRecorderBase {
    constructor() {
        super();
        this._events = [
            "drag",
            "drop"
        ];
        this.category = "dragDrop";
        this.delayListening = true;
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        if (event.toElement) {
            let toElement = super.parseElementInfo(event.toElement);
            Object.assign(breadcrumb.customData, super.setPropertyPrefixes(toElement, "to"));

        }
        breadcrumb.customData["offsetX"] = event.offsetX;
        breadcrumb.customData["offsetY"] = event.offsetY;
    }
}