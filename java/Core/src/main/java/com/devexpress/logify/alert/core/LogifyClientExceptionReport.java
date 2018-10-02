package com.devexpress.logify.alert.core;

import java.util.HashMap;
import java.util.Map;

public class LogifyClientExceptionReport {
    private String report;
    private StringBuilder reportContent;
    private Map<String, Object> data;

    public StringBuilder getReportContent() {
        return reportContent;
    }
    public void setReportContent(StringBuilder reportContent) {
        report = null;
        this.reportContent = reportContent;
    }

    public String getReportString() {
        if (report == null) {
            if (reportContent != null)
                report = reportContent.toString();
        }
        return report;
    }

    public Map<String, Object> getData() {
        return data;
    }
    public void setData(Map<String, Object> data) {
        this.data = data;
    }

    public void resetReportString() {
        report = null;
    }

    public LogifyClientExceptionReport clone() {
        LogifyClientExceptionReport clone = new LogifyClientExceptionReport();
        clone.copyFrom(this);
        return clone;
    }

    void copyFrom(LogifyClientExceptionReport value) {
        if (value.reportContent != null)
            this.reportContent = new StringBuilder(value.reportContent.toString());
        this.report = value.report;

        if (value.data != null) {
            this.data = new HashMap<String, Object>();
            for (String key : value.data.keySet())
                this.data.put(key, value.data.get(key));
        }
    }
}