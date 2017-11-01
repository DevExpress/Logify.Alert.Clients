'use strict';

import { breadcrumb, autoRecorderBreadcrumb } from "./breadcrumb.js";

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
        this.addListener(new consoleEventListeners());

        for (let i = 0; i < this._eventListeners.length; i++) {
            this._eventListeners[i].startListening(this.win, this.owner, this.eventCallback.bind(this));
        }
    }

    eventCallback(breadcrumb) {
        if (this.owner.collectBreadcrumbs === false) {
            return;
        }

        this.owner.addBreadcrumbs(breadcrumb);
    }
}

class eventsListenerBase {
    constructor() {
        this.delayListening = false;
        this._timeOut = 300;
        this._timeOutId;
        this._events = [];
        this.category = "";
    }
    startListening(win, owner, eventCallback) {
        this._owner = owner;
        for (let i = 0; i < this._events.length; i++) {
            win.addEventListener(
                this._events[i],
                function (event) {
                    if (this.delayListening) {
                        this.delayedListener(event, eventCallback);
                    } else {
                        this.collectBreadcrumb(event, eventCallback)
                    }
                }.bind(this),
                false
            );
        }
    }
    delayedListener(event, eventCallback) {
        if (this._owner.collectBreadcrumbs) {
            if (this._timeOutId != undefined) {
                clearTimeout(this._timeOutId);
            }
            this._timeOutId = setTimeout(function () {
                this.collectBreadcrumb(event, eventCallback);
            }.bind(this), this._timeOut);
        }
    }
    collectBreadcrumb(event, eventCallback) {
        if (this._owner.collectBreadcrumbs) {
            let breadcrumbsModel = new autoRecorderBreadcrumb("", this.getEventName(event.type));
            breadcrumbsModel.level = "Info";
            breadcrumbsModel.customData = {};
            breadcrumbsModel.customData["timeStamp"] = event.timeStamp;
            this.parseEventData(event, breadcrumbsModel);
            eventCallback(breadcrumbsModel);
        }
    }
    getEventName(eventType) {
        return eventType;
    }
    parseEventData(event, breadcrumb) {
        let element = event.target;
        this.setCategory(breadcrumb);
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
        // if (data.tag === "BODY")
        //     return;
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
    setCategory(breadcrumb) {
        if (this.category) {
            breadcrumb.category = this.category;
        }
    }
    setPropertyPrefixes(object, prefix) {
        if (!object)
            return {};
        const props = Object.keys(object);
        for (let i = 0; i < props.length; i++) {
            object[prefix + "_" + props[i]] = object[props[i]];
            delete object[props[i]];
        }
        return object;
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
class formEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "submit",
            "reset"
        ];
        this.category = "form";
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

        this.category = "keyboard";
    }
    parseEventData(event, breadcrumb) {
        super.parseEventData(event, breadcrumb);
        breadcrumb.customData["scanCode"] = event.code;
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
        this.category = "input";
    }
}

class selectEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "select"
        ];
        this.category = "selectText";
    }
}
class historyEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "popstate",
        ];
        this.category = "history";
    }
}
class printEventsListener extends eventsListenerBase {
    constructor() {
        super();
        this._events = [
            "beforeprint",
            "afterprint"
        ];
        this.category = "print";
    }
}

class windowEventsListener extends eventsListenerBase {
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

class dragDropEventsListener extends eventsListenerBase {
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

class ajaxEventsListener {
    constructor() {
        this._event = "xhr";
        this.category = "request";
    }
    startListening(win, owner, eventCallback) {
        this._owner = owner;
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
        if (this._owner.collectBreadcrumbs) {
            if (!request.onreadystatechange) {
                request.onreadystatechange = function (stateChangedEvent) {
                    this.collectBreadcrumb(stateChangedEvent.currentTarget, eventCallback);
                }.bind(this);
            } else {
                const oldEvent = request.onreadystatechange;
                var callbackStateChanged = this.collectBreadcrumb;
                let instance = this;
                request.onreadystatechange = function (stateChangedEvent) {
                    if (stateChangedEvent.currentTarget.readyState === 4) {
                        this.collectBreadcrumb(stateChangedEvent.currentTarget, eventCallback);
                    }
                    oldEvent.apply(this, arguments);
                }.bind(this);
            }
        }
    }
    collectBreadcrumb(request, eventCallback) {
        let breadcrumbsModel = new autoRecorderBreadcrumb("", this._event);
        breadcrumbsModel.level = "Info";
        breadcrumbsModel.category = this.category;

        breadcrumbsModel.customData = {};
        breadcrumbsModel.customData["status"] = request.status;
        breadcrumbsModel.customData["statusText"] = request.statusText;
        breadcrumbsModel.customData["responseURL"] = request.responseURL;

        eventCallback(breadcrumbsModel);
    }
}

class consoleEventListeners {
    constructor() {
        this._events = [
            "log",
            "error",
            "warn"
        ];
        this.category = "console";
    }
    startListening(win, owner, eventCallback) {
        this._owner = owner;
        if (win.console != undefined) {
            for (let i = 0; i < this._events.length; i++) {
                this.wrapObject(console, this._events[i], eventCallback);
            }
        }
    }
    wrapObject(object, property, callback) {
        let oldFunction = object[property];
        let wrapperClass = this;
        object[property] = function () {
            let args = Array.prototype.slice.call(arguments, 0);

            wrapperClass.createBreadcrumb(args, property, callback);

            if (typeof oldFunction === "function") {
                Function.prototype.apply.call(oldFunction, console, args);
            }
        };
    }
    createBreadcrumb(values, property, callback) {
        if (this._owner.collectBreadcrumbs) {
            let breadcrumbModel = new autoRecorderBreadcrumb("", "console." + property);
            breadcrumbModel.category = "console";
            breadcrumbModel.level = property == "warn" ? "Warning" : property == "error" ? "Error" : "Info";
            if (values != undefined && values.length > 0) {
                breadcrumbModel.customData = {};
                breadcrumbModel.customData["value"] = values.join(", ");
            }

            callback(breadcrumbModel);
        }
    }
}