'use strict';

export default class breadcrumb {
    constructor(message, eventType) {
        this.dateTime = new Date().toUTCString();
        this.message = message;
        this.event = eventType;
    }
}

export class autoRecorderBreadcrumb extends breadcrumb {
    constructor(message, eventType) {
        super(message, eventType);
        this.a = "1";
    }
}
