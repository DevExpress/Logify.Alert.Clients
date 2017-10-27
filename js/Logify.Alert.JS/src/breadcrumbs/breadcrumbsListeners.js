'use strict';

import breadcrumb from "./breadcrumb.js";

export default class breadcrumbsListeners {
    constructor(win, owner) {
        this.owner = owner;
        this.win = win;

        this._eventListeners = [];
    }

    addListener(eventListener) {
        this._eventListeners.push(eventListener);
    }

    startListening() {
        this.addListener(new mouseEventsListener());
        this.addListener(new formEventsListener());
        this.addListener(new domEventsListener());
        this.addListener(new clipboardEventsListener());
        this.addListener(new keyboardEventsListener());
        this.addListener(new selectEventsListener());
        this.addListener(new inputEventsListener());
        this.addListener(new historyEventsListener());
        this.addListener(new printEventsListener());
        this.addListener(new windowEventsListener());
        this.addListener(new dragDropEventsListener());
        this.addListener(new ajaxEventsListener());

        for (let i = 0; i < this._eventListeners.length; i++) {
            this._eventListeners[i].startListening(this.win, this.eventCallback.bind(this));
        }
    }

    eventCallback(breadcrumb) {
        if (this.owner.collectBreadcrumbs === false) {
            return;
        }

        this.owner.addBreadcrumbs(breadcrumb);
        //console.log(this.owner._breadcrumbs);
    }
}

class eventsListenerBase {
    constructor() {
        this._events = [];
    }
    startListening(win, eventCallback) {
        for (let i = 0; i < this._events.length; i++) {
            win.addEventListener(
                this._events[i],
                function (event) {
                    this.collectBreadcrumb(event, eventCallback)
                }.bind(this),
                false
            );
        }
    }
    collectBreadcrumb(event, eventCallback) {
        let breadcrumbsModel = new breadcrumb(new Date(), "", this.getEventName(event.type));
        breadcrumbsModel.level = "Info";
        breadcrumbsModel.customData = {};
        breadcrumbsModel.customData["timeStamp"] = event.timeStamp;
        this.parseEventData(event, breadcrumbsModel);
        eventCallback(breadcrumbsModel);
    }
    getEventName(eventType) {
        return eventType;
    }
    parseEventData(event, breadcrumb) {
        let element = event.target;
        const elementData = this.parseElementInfo(element);
        Object.assign(breadcrumb.customData, breadcrumb.customData, elementData);
    }
    isSecureElement(element) {
        return element.type && element.type.toLowerCase() === "password";
    }
    parseElementInfo(element) {
        let data = {};
        data.targetId = element.id;
        data.tag = element.tagName;
        if (data.tag === "BODY")
            return;
        if (element.name)
            data.name = element.name;
        if (element.href)
            data.href = element.href;
        if (element.type)
            data.type = element.type;
        if (element.value && !this.isSecureElement(element))
            data.value = element.value;
        if (element.checked != undefined)
            data.checked = element.checked;
        return data;
    }

}
class mouseEventsListener extends eventsListenerBase {
    constructor() {
        super();

        this._eventNames = {
            "click": "mouseClick",
            "auxclick": "mouseClick",
            "dblclick": "mouseDoubleClick"
        }

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
class formEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "submit",
            "reset"
        ];
    }
}

class domEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "DOMContentLoaded",
        ];
    }
}
class focusEventsListener extends eventsListenerBase {
    constructor() {
        super();

        this._events = [
            "focus",
            "blur"
        ];
    }
}

class clipboardEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "cut",
            "copy",
            "paste"
        ];
    }
}
class keyboardEventsListener extends eventsListenerBase {
    constructor() {
        super();

        this._eventNames = {
            "keydown": "keyDown",
            "keypress": "keyPress",
            "keyup": "keyUp"
        }

        this._events = Object.keys(this._eventNames);
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        breadcrumb.customData["scanCode"] = event.keyCode;
        breadcrumb.customData["action"] = event.type.replace("key", "");

        breadcrumb.customData["char"] = super.isSecureElement(event.target) ? "*" : event.key;
        breadcrumb.customData["key"] = super.isSecureElement(event.target) ? "Multiply" : event.code;
    }

    getEventName(eventType) {
        return this._eventNames.hasOwnProperty(eventType) ? this._eventNames[eventType] : eventType;
    }
}
class inputEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "change"
        ];
    }
}

class selectEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "select"
        ];
    }
}
class historyEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "popstate",
        ];
    }
}
class printEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "beforeprint",
            "afterprint"
        ];
    }
}

class windowEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "resize"
        ];
    }
    parseEventData(event, breadcrumb) {
        breadcrumb.customData["newWidth"] = event.target.innerWidth;
        breadcrumb.customData["newHeight"] = event.target.innerHeight;
    }
}

class dragDropEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "drag",
            "drop"
        ];
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        breadcrumb.customData["offsetX"] = event.offsetX;
        breadcrumb.customData["offsetY"] = event.offsetY;
    }
}

class ajaxEventsListener {
    constructor() {
        this._event = "request";
     }
    startListening(win, eventCallback) {
        this.addXMLRequestListenerCallback(
            function (request) {
                this.onStateChangeCallback(request, eventCallback);
            }.bind(this)
        );
    }
    addXMLRequestListenerCallback(callback) {
        if (XMLHttpRequest.callbacks) {
            XMLHttpRequest.callbacks.push(callback);
        } else {
            XMLHttpRequest.callbacks = [callback];
            const oldSend = XMLHttpRequest.prototype.send;
            XMLHttpRequest.prototype.send = function () {
                for (let i = 0; i < XMLHttpRequest.callbacks.length; i++) {
                    XMLHttpRequest.callbacks[i](this);
                }
                oldSend.apply(this, arguments);
            }
        }
    }
    onStateChangeCallback(request, eventCallback) {
        if ( !request.onreadystatechange ) {
            request.onreadystatechange = function(stateChangedEvent) {
                this.collectBreadcrumb(stateChangedEvent.currentTarget, eventCallback).bind(this);
            }.bind(this);
        } else {
            const oldEvent =  request.onreadystatechange;
            var callbackStateChanged = this.collectBreadcrumb;
            let instance = this;
            request.onreadystatechange = function(stateChangedEvent) {
                this.collectBreadcrumb(stateChangedEvent.currentTarget, eventCallback);
                oldEvent.apply(this, arguments);
            }.bind(this)
        }
    }
    collectBreadcrumb(request, eventCallback) {
        let breadcrumbsModel = new breadcrumb(new Date(), "", this._event);
        breadcrumbsModel.customData = {}
        
        breadcrumbsModel.customData["readyState"] = request.readyState;
        breadcrumbsModel.customData["status"] = request.status;
        breadcrumbsModel.customData["statusText"] = request.statusText;
        breadcrumbsModel.customData["responseURL"] = request.responseURL;

        eventCallback(breadcrumbsModel);
    }
}