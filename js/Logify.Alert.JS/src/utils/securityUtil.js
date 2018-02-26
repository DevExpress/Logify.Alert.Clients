'use strict';

export default class securityUtil {
    constructor(sensitiveDataRules) {
        this.maskValue = "stripped_by_logify_client";

        this.updateFilters(sensitiveDataRules);
    }

    updateFilters(newFilters) {
        this.sensitiveDataRules = !newFilters ? [] : newFilters;
        this.rulesCnt = this.sensitiveDataRules.length;

        this.prepareRules();
    }

    prepareRules() {
        for (let i = 0; i < this.rulesCnt; i++) {
            if (typeof this.sensitiveDataRules[i] === 'string') {
                this.sensitiveDataRules[i] = new RegExp("^" + this.sensitiveDataRules[i] + "$", "i");
            }
        }
    }

    maskedObject(dataObject) {
        if (typeof dataObject === "object") {
            const keys = Object.keys(dataObject);
            for (let i = 0; i < keys.length; i++) {
                if (this._isSecure(keys[i]))
                    dataObject[keys[i]] = this.maskValue;
                else {
                    dataObject[keys[i]] = this.maskedObject(dataObject[keys[i]]);
                }    
            }
        }
        return dataObject;
    }

    maskedInputValue(domInput) {
        if (!domInput)
            return null;
        if (this.inputShouldBeMasked(domInput))
            return this.maskValue;
        return domInput.value;    
    }

    inputShouldBeMasked(domInput) {
        if (!domInput || !domInput.type)
            return false;
        if (this._isPasswordInput(domInput))
            return true;
        return this._isSecure(domInput.id) || this._isSecure(domInput.name);
    }
    
    _isSecure(key) {
        if (!key)
            return false;
        for (let i = 0; i < this.rulesCnt; i++) {
            if (typeof this.sensitiveDataRules[i] === 'object' && typeof this.sensitiveDataRules[i].exec === 'function') {
                if (this.sensitiveDataRules[i].exec(key) !== null) {
                    return true;
                }
            } else if (this.sensitiveDataRules[i] === key) {
                return true;
            }
        }
        return false;
    }

    _isPasswordInput(domInput) {
        return domInput.type.toLowerCase() === "password";
    }
}