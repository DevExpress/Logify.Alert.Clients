'use strict';
import collectorBase from "./collectorBase.js";

export default class libsCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report) {
        super.process(win, report);

        if(report.libs === undefined)
            report.libs = new Object();

        const dxASPx = win.ASPx;
        if(dxASPx) {
            report.libs.dxASPx = new Object();
            if(dxASPx.VersionInfo) {
                report.libs.dxASPx.versionInfo = dxASPx.VersionInfo;
            }
        }
    }
}