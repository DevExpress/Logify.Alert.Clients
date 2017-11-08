'use strict';

import eventRecorderBase from './eventRecorderBase.js'

export class domEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "DOMContentLoaded",
        ];
    }
}

export class formEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "submit",
            "reset"
        ];
        this.category = "form";
    }
}

export class focusEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "focus",
            "blur"
        ];
    }
}

export class clipboardEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "cut",
            "copy",
            "paste"
        ];
    }
}

export class inputEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "change"
        ];
        this.category = "input";
    }
}

export class selectEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "select"
        ];
        this.category = "selectText";
    }
}

export class historyEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "popstate",
        ];
        this.category = "history";
    }
}

export class printEventsRecorder extends eventRecorderBase {
    constructor() {
        super();
        this._events = [
            "beforeprint",
            "afterprint"
        ];
        this.category = "print";
    }
}