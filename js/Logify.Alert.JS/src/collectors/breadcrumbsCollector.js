'use strict';
import collectorBase from "./collectorBase.js";
import breadcrumb from "../breadcrumbs/breadcrumb.js";

export default class breadcrumbsCollector extends collectorBase {
    constructor(_owner) {
        super(_owner);
    }

    process(win, report) {
        super.process(win, report);

        if ((this.owner == null) || (this.owner == undefined))
            return;
        if (this.owner._breadcrumbs != undefined && this.owner._breadcrumbs.length > 0)
            report.breadcrumbs = this.owner._breadcrumbs;
    }
}