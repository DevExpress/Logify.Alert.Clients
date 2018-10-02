package com.devexpress.logify.alert.android.reportSender;

import android.os.AsyncTask;
import android.util.Log;
import com.devexpress.logify.alert.core.LogifyClientExceptionReport;
import com.devexpress.logify.alert.core.reportSender.IExceptionReportSender;
import com.devexpress.logify.alert.core.reportSender.ReportSender;
import java.io.IOException;
import java.util.concurrent.Callable;

public class AndroidReportSender extends ReportSender {
    @Override
    public IExceptionReportSender createEmptyClone() {
        return new AndroidReportSender();
    }

    @Override
    public boolean sendExceptionReportCore(final LogifyClientExceptionReport report) {
        Callable<Boolean> sendExceptionReportCallable = new Callable<Boolean>(){
            public Boolean call() throws IOException {
                return sendExceptionReportCoreSuper(report);
            }
        };

        SendReportTask sendReportAsyncTask = new SendReportTask();
        sendReportAsyncTask.execute(sendExceptionReportCallable);

        return true;
    }

    private Boolean sendExceptionReportCoreSuper(LogifyClientExceptionReport report) throws IOException {
        return super.sendExceptionReportCore(report);
    }

    private class SendReportTask extends AsyncTask<Callable<Boolean>, Integer, Boolean> {
        @Override
        protected Boolean doInBackground(Callable<Boolean>... sendReportCallables) {
            try {
                if (sendReportCallables.length > 0 && sendReportCallables[0] != null)
                    return sendReportCallables[0].call();
                else return false;
            } catch (Throwable e) {
                Log.e(e.getClass().getName(), e.getMessage());
                return  false;
            }
        }
    }
}
