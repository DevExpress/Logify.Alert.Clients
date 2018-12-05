'use strict';
import rawHtmlTagCollectorBase from "./rawHtmlTagCollectorBase";

export default class metasCollector extends rawHtmlTagCollectorBase {
    constructor(_owner) {
        super(_owner, "meta");
    }
}
