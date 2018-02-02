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
            try {  
                this.collectBreadcrumb(event, eventCallback);
            } catch (ex) { }
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
        let target = event.target;
        let element = this.isTextElement(target) ? this.getTextElement(target) : target;
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
            data.name = addStringifiedValue(element.name);
        if (element.href)
            data.href = addStringifiedValue(element.href);
        if (element.type)
            data.type = addStringifiedValue(element.type);
        if (element.value && !this.isSecureElement(element))
            data.value = addStringifiedValue(element.value);
        if (element.checked != undefined)
            data.checked = addStringifiedValue(element.checked);
        return data;
    }

    addStringifiedValue(value) {
        if (typeof value === "object")
            return JSON.stringify(value);
        return value;    
    }

    isTextElement(element) {
        return element.nodeType === 3;
    }
    getTextElement(element) {
        return element.parentNode;
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
