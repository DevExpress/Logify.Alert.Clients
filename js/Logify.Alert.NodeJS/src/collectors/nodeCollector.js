'use strict'
import compositeCollector from "./compositeCollector.js";
import nodeVersionCollector from "./nodeVersionCollector.js";
import platformEnvironmentCollector from "./platformEnvironmentCollector.js";
import logifyInfoCollector from "./logifyInfoCollector.js";

export default class nodeCollector extends compositeCollector {
    constructor() {
        super();

        this.applicationName = undefined;
        this.applicationVersion = undefined;
        this.userId = undefined;

        this._reportData = new Object();
        this._parentProjectJsonFile = null;

        this.collectors.push(new nodeVersionCollector());
        this.collectors.push(new platformEnvironmentCollector());
        this.collectors.push(new logifyInfoCollector());
    }

    process(report) {
        super.process(report);
    }

    handleException(error) {
        this._reportData.error = new Object();
        this._reportData.error.message = error.message;
        this._reportData.error.stack = error.stack;
        this.collectData();
    }

    handleRejection(reason) {
        this._reportData.rejection = new Object();
        if(reason.message != undefined) {
            this._reportData.rejection.error = new Object();
            this._reportData.rejection.error.message = reason.message;
            if(reason.stack != undefined) {
                this._reportData.rejection.error.stack = reason.stack;
            }
        } else {
            this._reportData.rejection.reason = reason;
        }
        this.collectData();
    }
    
    collectData() {
        this.fillAppData();
        this.process(this._reportData);
    }

    getApplicationName() {
        if(this.applicationName != undefined)
            return this.applicationName;

        let process = require("process");
        if(process.env.npm_package_name != undefined)
            return process.env.npm_package_name;

        this.getPackageJson();
        if(this._parentProjectJsonFile != null)
            return this._parentProjectJsonFile.name;

        return "";
    }

    getApplicationVersion() {
        if(this.applicationVersion != undefined)
            return this.applicationVersion;

        let process = require("process");
        if(process.env.npm_package_version != undefined)
            return process.env.npm_package_version;

        this.getPackageJson();
        if(this._parentProjectJsonFile != null)
            return this._parentProjectJsonFile.version;

        return "";
    }

    getUserId() {
        return (this.userId == undefined) ? "" : this.userId;
    }

    fillAppData() {
        if(this._reportData.logifyApp === undefined)
            this._reportData.logifyApp = new Object();

        this._reportData.logifyApp.name = this.getApplicationName();
        this._reportData.logifyApp.version = this.getApplicationVersion();
        this._reportData.logifyApp.userId = this.getUserId();
    }

    get reportData() {
        return this._reportData;
    }
    
    getPackageJson() {
        if(this._parentProjectJsonFile != null)
            return;

        let pjsonPath = this.findPathToNodeModules();
        if(pjsonPath == null)
            return null;

        pjsonPath = this.findPackageJsonFilePath(pjsonPath);
        if(pjsonPath == null)
            return null;

        this._parentProjectJsonFile = require(pjsonPath);
    }
    findPathToNodeModules() {
        var path = require("path");
        var dir = path.join(__dirname);
        if(dir.indexOf("\\node_modules\\") == -1)
            return dir;

        let result = this.findParentFolderByCondition(dir, (d) => { return !d.endsWith("node_modules\\"); });
        if(result == null)
            return null;

        return path.join(result, '../');
    }

    findPackageJsonFilePath(dir) {
        if(dir.charAt(dir.length -1) != '\\')
            dir += '\\';

        let result = this.findParentFolderByCondition(dir, (d) => { return !this.isFileExist(d + 'package.json'); });
        if(result == null)
            return null;

        return result + 'package.json';
    }

    findParentFolderByCondition(dir, condition) {
        var path = require("path");
        var previousDir = "";
        while(condition(dir)) {
            dir = path.join(dir, '../');
            if(dir == previousDir)
                return null;

            previousDir = dir;
        }

        return dir;
    }

    isFileExist(filePath) {
        let fs = require('fs');
        try{
            fs.accessSync(filePath);
            return true;
        } catch(e) {
            return false;
        }
    }
}
