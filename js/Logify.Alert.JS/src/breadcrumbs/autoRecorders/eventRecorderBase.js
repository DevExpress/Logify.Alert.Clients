'use strict';

import { breadcrumb, autoRecordedBreadcrumb } from "../breadcrumb.js";

export default class eventRecorderBase {
    constructor() {
        this._events = [];
        this.category = "";
    }

    startListening(win, owner, eventCallback) {
        this._owner = owner;

        this._mainCallback = function (event) {        
            this.collectBreadcrumb(event, eventCallback);
        }.bind(this);

        for (let i = 0; i < this._events.length; i++) {
            win.addEventListener(
                this._events[i],
                this._mainCallback,
                false
            );
        }
    }

    stopListening(win, owner) {
        if (this._owner && this._mainCallback) {
            for (let i = 0; i < this._events.length; i++) {
                win.removeEventListener(
                    this._events[i],
                    this._mainCallback,
                    false
                );
            }
        }
    }

    collectBreadcrumb(event, eventCallback) {
        if (this._owner.collectBreadcrumbs) {
            let breadcrumbsModel = new autoRecordedBreadcrumb("", this.getEventName(event.type));
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
