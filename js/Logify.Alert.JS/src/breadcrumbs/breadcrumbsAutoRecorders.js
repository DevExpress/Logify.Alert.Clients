'use strict';

import mouseEventRecorder from "./autoRecorders/mouseEventsRecorder.js";
import { 
    formEventsRecorder, 
    domEventsRecorder, 
    clipboardEventsRecorder, 
    inputEventsRecorder, 
    selectEventsRecorder,
    historyEventsRecorder,
    printEventsRecorder
 } from "./autoRecorders/simpleEventsRecorders.js";

 import windowEventsRecorder from "./autoRecorders/windowEventsRecorder.js";
 import keyboardEventsRecorder from "./autoRecorders/keyboardEventsRecorder.js";
 import dragDropEventsRecorder from "./autoRecorders/dragDropEventsRecorder.js";
 import ajaxEventsRecorder from "./autoRecorders/ajaxEventsRecorder.js";
 import consoleEventRecorder from "./autoRecorders/consoleEventRecorder.js";
 

export default class breadcrumbsAutoRecorders {
    constructor(win, owner) {
        this.owner = owner;
        this.win = win;

        this._eventListeners = [];
        this._eventDelayedListeners = [];
    }

    addRecorder(eventListener) {
        this._eventListeners.push(eventListener);
        if (eventListener.delayListening) {
            this._eventDelayedListeners.push(eventListener);
        }
    }

    startListening() {
        this.addRecorder(new mouseEventRecorder());
        this.addRecorder(new formEventsRecorder());
        this.addRecorder(new domEventsRecorder());
        this.addRecorder(new clipboardEventsRecorder());
        this.addRecorder(new inputEventsRecorder());
        this.addRecorder(new selectEventsRecorder());
        this.addRecorder(new historyEventsRecorder());
        this.addRecorder(new printEventsRecorder());
        this.addRecorder(new windowEventsRecorder());
        this.addRecorder(new dragDropEventsRecorder());
        this.addRecorder(new keyboardEventsRecorder());        
        this.addRecorder(new ajaxEventsRecorder());
        this.addRecorder(new consoleEventRecorder());
        
        for (let i = 0; i < this._eventListeners.length; i++) {
            this._eventListeners[i].startListening(this.win, this.owner, this.eventCallback.bind(this));
        }
    }

    stopListening() {
        if (this._eventListeners) {
            for (let i = 0; i < this._eventListeners.length; i++) {
                this._eventListeners[i].stopListening(this.win, this.owner);
            }
        }
    }

    eventCallback(breadcrumb) {
        if (this.owner.collectBreadcrumbs === false) {
            return;
        }

        this.owner.addBreadcrumbs(breadcrumb);
    }

    beforeReadBreadcrumbs() {
        if (!this.owner.collectBreadcrumbs)
            return;
        for (let i = 0; i < this._eventDelayedListeners.length; i++) {
            this._eventDelayedListeners[i].forceExecute();
        }
    }
}