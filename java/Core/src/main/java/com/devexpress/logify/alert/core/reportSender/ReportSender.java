package com.devexpress.logify.alert.core.reportSender;

import com.devexpress.logify.alert.core.LogifyClientExceptionReport;
import java.io.BufferedOutputStream;
import java.net.HttpURLConnection;
import java.net.URL;

public class ReportSender extends ExceptionReportSenderSkeleton {
    private final String endPoint = "api/report/newreport";

    public IExceptionReportSender createEmptyClone() {
        return new ReportSender();
    }

    public boolean sendExceptionReportCore(LogifyClientExceptionReport report) {
        try {
            URL url = new URL(createEndPointUrl(getServiceUrl(), endPoint));
            HttpURLConnection urlConnection = (HttpURLConnection) url.openConnection();
            urlConnection.setRequestMethod("POST");
            urlConnection.setConnectTimeout(getReportTimeoutMilliseconds());
            urlConnection.setRequestProperty("Content-Type", "application/json");
            urlConnection.setRequestProperty("Authorization", String.format("%s %s", "amx", this.getApiKey()));
            urlConnection.setUseCaches(false);
            urlConnection.setDoInput(true);
            urlConnection.setDoOutput(true);

            BufferedOutputStream wr = new BufferedOutputStream(urlConnection.getOutputStream());
            wr.write(report.getReportString().getBytes("UTF-8"));
            wr.flush();
            wr.close();


            String responseMessage = urlConnection.getResponseMessage();
            int responseCode = urlConnection.getResponseCode();
            urlConnection.disconnect();
            return responseMessage != null && responseCode == 200;
        } catch (java.net.SocketTimeoutException e) {
            return false;
        } catch (java.io.IOException e) {
            return false;
        }
    }

    String createEndPointUrl(String serviceUrl, String queryString) {
        String url = serviceUrl;
        if (url != null && url.length() > 0) {
            if (url.charAt(url.length() - 1) != '/')
                url += '/';
            url += queryString;
        }
        return url;
    }
}