package com.devexpress.logify.alert.core.events;

import com.devexpress.logify.alert.core.LogifyClientBase;
import java.util.EventObject;

public class LogifyEvent extends EventObject {
    public  LogifyEvent(LogifyClientBase source, Throwable exception){
        super(source);
        this.exception = exception;
    }

    Throwable exception;
    public  Throwable getException(){
        return  this.exception;
    }
}