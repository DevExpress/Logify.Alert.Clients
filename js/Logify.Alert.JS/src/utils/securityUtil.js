'use strict';

export default class securityUtil {
    constructor(sensitiveDataRules) {
        this.maskValue = "stripped_by_logify_client";

        this.updateFilters(sensitiveDataRules);
    }

    updateFilters(newFilters) {
        this.sensitiveDataRules = !newFilters ? [] : newFilters;
        this.rulesCnt = this.sensitiveDataRules.length;
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

    maskedCookies(cookies) {
        if (this.rulesCnt <= 0 || cookies.length <= 0)
            return cookies;
        const cookiesDelimiter = "; ";
        let keyValuePairsString = cookies.split(cookiesDelimiter);
        if (keyValuePairsString.length <= 0)
            return cookies;
        let resultCookies = "", keyValuePairs;      
        for (let i = 0; i < keyValuePairsString.length; i++) {
            keyValuePairs = keyValuePairsString[i].split("=");            
            resultCookies += (keyValuePairs.length === 2 && this._isSecure(keyValuePairs[0])) ?  (keyValuePairs[0] + "=" + this.maskValue) : keyValuePairsString[i];
            resultCookies += cookiesDelimiter;
        }
        return resultCookies;    
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
            try {
                if (typeof this.sensitiveDataRules[i] === 'object' && typeof this.sensitiveDataRules[i].exec === 'function') {
                    if (this.sensitiveDataRules[i].exec(key) !== null) {
                        return true;
                    }
                } else if (typeof this.sensitiveDataRules[i] === 'string' 
                            && typeof key === 'string' 
                            && this.sensitiveDataRules[i].toLowerCase() === key.toLowerCase()) {
                    return true;
                } else if (this.sensitiveDataRules[i] === key) {
                    return true;
                }
            } catch (e) { }
        }
        return false;
    }

    _isPasswordInput(domInput) {
        return domInput.type.toLowerCase() === "password";
    }
}