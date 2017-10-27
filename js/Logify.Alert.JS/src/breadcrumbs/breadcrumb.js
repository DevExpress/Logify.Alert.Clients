'use strict';

export default class breadcrumb {
    constructor(dateTime, message, eventType) {
        this.dateTime = dateTime;
        this.message = message;
        this.event = eventType;
    }
}