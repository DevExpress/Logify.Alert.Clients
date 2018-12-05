'use strict';
import collectorBase from "./collectorBase.js";

export default class rawHtmlTagCollectorBase extends collectorBase {
    constructor(_owner, tagName) {
        super(_owner);
        this._tagName = tagName;
    }

    process(win, report) {
        super.process(win, report);

        const tags = win.document.getElementsByTagName(this._tagName);
        const tagsCount = tags.length;
        let result = [];
        for(let i = 0; i < tagsCount; i++) {
            if(tags[i].innerHTML === "")
                result.push(tags[i].outerHTML);
        }

        if(result.length == 0)
            return;

        const dataPropertyName = this._tagName + "s";
        report[dataPropertyName] = result;
    }
}