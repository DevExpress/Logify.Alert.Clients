# Logify Alert for Node.js

Node.js client for reports exceptions to [Logify Alert](https://logify.devexpress.com/).

[![npm](https://img.shields.io/npm/v/logify-alert.svg)](https://www.npmjs.com/package/logify-alert)

## Install

```sh
$ npm install logify-alert --save
```

## Quick Start

```javascript
const logifyAlert = require('logify-alert');
let client = new logifyAlert.default('SPECIFY_YOUR_API_KEY_HERE')
client.startHandling();
```

## API

### Fields

#### applicationName

Specifies an application name. This name is shown in generated reports.

```javascript
client.applicationName = 'My Application';
```

#### applicationVersion

Specifies an application version. This version is shown in generated reports. 

```javascript
client.applicationVersion = '1.0.1';
```

#### userId

Specifies a unique user identifier that corresponds to a sent report.

```javascript
client.userId = 'unique user id';
```

#### customData

A dictionary that contains custom data sent with a generated report. 

```javascript
client.customData = {FIRST_KEY:  "FIRST DATA", SECOND_KEY: "SECOND DATA"};
```

### Methods for automatic reporting

Logify Alert allows you to automatically listen to uncaught exceptions and deliver crash reports. For this purpose, use the methods below.

#### startHandling()

Commands Logify Alert to start listening to uncaught exceptions and rejections and sending reports for all processed exceptions. 

```javascript
client.startHandling();
```

#### stopHandling()

Commands Logify Alert to stop listening to uncaught exceptions and rejections. 

```javascript
client.stopHandling();
```

### Methods for manual reporting

Alternatively, Logify Alert allows you to catch required exceptions manually, generate reports based on caught exceptions and send these reports only. For this purpose, use the methods below.

#### sendException(error)

Sends information about the caught exception to the Logify Alert server.

```javascript
client.sendException(error);
```

#### sendRejection(reason)

Sends information about the caught rejection to the Logify Alert server.

```javascript
client.sendRejection(reason);
```

### Callbacks

#### beforeReportException(customData)

Specifies a delegate to be called before sending exceptions and rejections to the Logify Alert server.

The *customData* parameter holds custom data sent among with an exception or a rejection. Default value is *undefined*. If you change a parameter value within a callback, a new value will be stored and passed to a callback next time you call it in your application.

```javascript
client.beforeReportException = function (customData) {
        if (customData == undefined) {
            customData = {};
        }
        customData["test key"] = "test value";
        return customData;
}
```
