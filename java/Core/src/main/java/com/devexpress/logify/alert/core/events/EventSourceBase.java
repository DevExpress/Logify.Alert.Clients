package com.devexpress.logify.alert.core.events;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class EventSourceBase {
    private List<LogifyEventListener> listeners = Collections.synchronizedList(new ArrayList<LogifyEventListener>());

    public void addListener(LogifyEventListener listener) {
        listeners.add(listener);
    }

    public void removeListener(LogifyEventListener listener)  {
        listeners.remove(listener);
    }

    public <T extends LogifyEvent> void fireEvent(T event) {
        for (LogifyEventListener listener : listeners) {
            listener.handle(event);
        }
    }

    public int getSize() {
        return listeners.size();
    }
}