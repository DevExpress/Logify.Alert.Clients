package com.devexpress.logify.alert.core.events;

public interface LogifyEventListener<T extends LogifyEvent> {
    void handle(T event);
}