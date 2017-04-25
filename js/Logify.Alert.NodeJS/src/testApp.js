'use strict';
import logifyAlert from "./logifyAlert.js"

let client = new logifyAlert("B05B3BA1E17C4726ADEB59B3DB84967E");
client.applicationName = "testApp";
client.applicationVersion = "123";
client.customData = {"Key1":  "Data1", "Key2": "Data2"};

let afterReportExceptionCallback = function (e) {
};

client.afterReportException = afterReportExceptionCallback.bind("qwe");
client.startHandling();

let qwe = undefined;
qwe.asd2();

// let zzz = 1 / 0;
//
// var promise = new Promise((resolve, reject) => {
//     setTimeout(() => {
//         reject(new Error("Test error"));
//     }, 10);
//
// });
