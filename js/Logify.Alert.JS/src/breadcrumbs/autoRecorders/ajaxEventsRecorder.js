'use strict';

import { breadcrumb, autoRecordedBreadcrumb } from "../breadcrumb.js";

export default class ajaxEventsRecorder {
    constructor() {
        this._event = "xhr";
        this.category = "request";
        this._callbackEvents = [
            "onload",
            "onabort",
            "onerror"
        ];
    }

    startListening(win, owner, eventCallback) {
        this._owner = owner;

        this.addXMLRequestListenerCallback(
            function (request, additionalData) {
                const requestTarget = request.target;
                if (requestTarget && requestTarget.readyState === 4)
                    this.collectBreadcrumb(request.type, requestTarget, additionalData, eventCallback);
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
                let initTime = new Date().getTime();
                const requestInfo = {
                    method: arguments[0],
                    requestUrl: arguments[1],
                    initTime: initTime
                };

                for (let i = 0; i < wrapper._callbackEvents.length; i++) {
                    wrapper.addXhrWrapper(this, wrapper._callbackEvents[i], requestInfo, callback);
                }

                wrapper._defaultCallback.apply(this, arguments);
            }
        }
    }

    addXhrWrapper(xhr, functionName, requestInfo, callback) {
        try {
            if (functionName in xhr) {
                let oldFunction;
                if (xhr[functionName]) {
                    oldFunction = xhr[functionName];
                }

                xhr[functionName] = function () {
                    if (arguments && arguments[0])
                        callback(arguments[0], requestInfo);
                    if (oldFunction) {
                        oldFunction.apply(this, arguments);
                    }
                }
            }
        } catch (e) { }
    }

    collectBreadcrumb(type, request, additionalData, eventCallback) {
        let breadcrumbsModel = new autoRecordedBreadcrumb("", this._event);
        breadcrumbsModel.level = "Info";
        breadcrumbsModel.category = this.category;

        breadcrumbsModel.customData = {};
        breadcrumbsModel.customData["type"] = type;
        breadcrumbsModel.customData["readyState"] = request.readyState;
        breadcrumbsModel.customData["status"] = request.status;
        breadcrumbsModel.customData["statusText"] = request.statusText;
        breadcrumbsModel.customData["responseURL"] = request.responseURL;
        if (additionalData) {
            if (additionalData.requestUrl)
                breadcrumbsModel.customData["requestUrl"] = additionalData.requestUrl;
            if (additionalData.method)
                breadcrumbsModel.customData["method"] = additionalData.method;
            if (additionalData.initTime)
                breadcrumbsModel.customData["duration"] = (new Date().getTime() - additionalData.initTime);
        }

        eventCallback(breadcrumbsModel);
    }
}