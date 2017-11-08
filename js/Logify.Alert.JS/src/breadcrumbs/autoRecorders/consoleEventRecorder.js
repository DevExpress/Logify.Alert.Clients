'use strict';

import { breadcrumb, autoRecordedBreadcrumb } from "../breadcrumb.js";

export default class consoleEventRecorder {
    constructor() {
        this._events = [
            "log",
            "error",
            "warn"
        ];
        this.category = "console";
        this._defaultCallback = [];
    }

    startListening(win, owner, eventCallback) {
        this._owner = owner;
        
        if (win.console != undefined) {
            for (let i = 0; i < this._events.length; i++) {
                this.wrapObject(console, this._events[i], eventCallback);
            }
        }
    }

    stopListening(win, owner) {
        for (let i = 0; i < this._events.length; i++) {
            if (this._defaultCallback && typeof this._defaultCallback[this._events[i]] === "function") {
                console[this._events[i]] = this._defaultCallback[this._events[i]];
            }
        }
    }

    wrapObject(object, property, callback) {
        this._defaultCallback[property] = object[property];
        let wrapperClass = this;
        object[property] = function () {
            let args = Array.prototype.slice.call(arguments, 0);

            wrapperClass.createBreadcrumb(args, property, callback);

            if (typeof wrapperClass._defaultCallback[property] === "function") {
                Function.prototype.apply.call(wrapperClass._defaultCallback[property], console, args);
            }
        };
    }

    createBreadcrumb(values, property, callback) {
        if (this._owner.collectBreadcrumbs) {
            let breadcrumbModel = new autoRecordedBreadcrumb("", "console." + property);
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