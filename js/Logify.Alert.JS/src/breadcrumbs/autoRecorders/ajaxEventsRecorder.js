'use strict';

import { breadcrumb, autoRecordedBreadcrumb } from "../breadcrumb.js";

export default class ajaxEventsRecorder {
    constructor() {
        this._event = "xhr";
        this.category = "request";
    }

    startListening(win, owner, eventCallback) {
        this._owner = owner;

        this.addXMLRequestListenerCallback(
            function (request) {
                if (request.target != undefined)
                    this.collectBreadcrumb(request.target, eventCallback);
            }.bind(this)
        );
    }

    stopListening(win, owner) {
        if (this._defaultCallback) {
            XMLHttpRequest.prototype.send = this._defaultCallback;
        }
    }

    addXMLRequestListenerCallback(callback) {
        if (XMLHttpRequest.callbacks) {
            XMLHttpRequest.callbacks.push(callback);
        } else {
            XMLHttpRequest.callbacks = [callback];

            this._defaultCallback = XMLHttpRequest.prototype.open;
            const wrapper = this;

            XMLHttpRequest.prototype.open = function () {
                const xhr = this;
                try {
                    if ('onload' in xhr) {
                        if (!xhr.onload) {
                            xhr.onload = callback;
                        } else {
                            const oldFunction = xhr.onload;
                            xhr.onload = function() {
                                callback(Array.prototype.slice.call(arguments));
                                oldFunction.apply(this, arguments);
                            }
                        }
                    }
                } catch (e) {
                    this.onreadystatechange = callback;
                }

                wrapper._defaultCallback.apply(this, arguments);
            }
        }
    }

    collectBreadcrumb(request, eventCallback) {
        let breadcrumbsModel = new autoRecordedBreadcrumb("", this._event);
        breadcrumbsModel.level = "Info";
        breadcrumbsModel.category = this.category;

        breadcrumbsModel.customData = {};
        breadcrumbsModel.customData["readyState"] = request.readyState;
        breadcrumbsModel.customData["status"] = request.status;
        breadcrumbsModel.customData["statusText"] = request.statusText;
        breadcrumbsModel.customData["responseURL"] = request.responseURL;

        eventCallback(breadcrumbsModel);
    }
}