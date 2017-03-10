# Logify Alert for WinForms and console applications
A WinForms and console client to report exceptions to [Logify Alert](https://logify.devexpress.com).

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Win/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Win.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Win.svg" style="max-width:100%;" /></a>
```sh
$ Install-Package Logify.Alert.Win
```

## Quick Start
### Automatic error reporting
```csharp
using DevExpress.Logify.Win;

LogifyAlert client = LogifyAlert.Instance;
client.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
client.StartExceptionsHandling();
```

### Manual error reporting
```csharp
using DevExpress.Logify.Win;
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    LogifyAlert.Instance.Send(e);
}
```

### Manual error reporting via [System.Diagnostics.Trace](https://msdn.microsoft.com/en-us/library/system.diagnostics.trace(v=vs.110).aspx)
```csharp
using System.Diagnostics;
using DevExpress.Logify.Win;
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    Trace.TraceError("An exception occurred", e);
}
```

You can set up the Logify Alert trace listener using the **App.config** file as follows.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.diagnostics>
    <trace autoflush="true" indentsize="4">
      <listeners>
        <add name="LogifyAlertTraceListener"  type="DevExpress.Logify.Win.LogifyAlertTraceListener, Logify.Alert.Win" />
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```

## Configuration
You can set up the Logify Alert client using the **App.config** file as follows.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="logifyAlert" type="DevExpress.Logify.LogifyConfigSection, Logify.Alert.Win" />
  </configSections>
  <logifyAlert>
    <apiKey value="My Api Key" />
    <confirmSend value="false" />
    <customData>
      <add key="MACHINE_NAME" value="My Server" />
    </customData>
  </logifyAlert>
</configuration>
```

## API
### Properties
#### ApiKey
String. Specifies an [API Key](https://logify.devexpress.com/Documentation/CreateApp) used to register the applications within the Logify service.
```csharp
client.ApiKey = "My Api Key";
```

#### AppName
String. Specifies the application name.
```csharp
client.AppName = "My Application";
```
#### AppVersion
String. Specifies the application version.
```csharp
client.AppVersion = "1.0.2";
```
#### ConfirmSendReport
Boolean. Specifies whether or not a user must confirm sending reports to the Logify Alert service.
By default, the **ConfirmSendReport** property is set to **false** and all uncaught exceptions are automatically processed and sent to the Logify Alert service.
```csharp
client.ConfirmSendReport = true;
```
#### CustomData
IDictionary<String, String>. Gets the collection of custom data sent with generated reports.
Use the **CustomData** property to attach additional information to the generated report. For instance, you can use this property to track additional metrics that are important in terms of your application: CPU usage, environment parameters, and so on.

```csharp
client.CustomData["CustomerName"] = "Mary";
```

#### Instance
Singleton. Returns the single instance of the LogifyAlert class.
```csharp
LogifyAlert client = LogifyAlert.Instance;
```

#### OfflineReportsCount
Integer. Specifiies the number of last reports to be kept once an Internet connection is lost. Reports are only saved when the *OfflineReportsEnabled* property is set to **true**.
```csharp
client.OfflineReportsEnabled = true;
client.OfflineReportsCount = 20; // Keeps the last 20 reports
```

#### OfflineReportsDirectory
String. Specifies a directory path that will be used to store reports once an Internet connection is lost. Reports are only saved when the *OfflineReportsEnabled* property is set to **true**.
```csharp
client.OfflineReportsEnabled = true;
client.OfflineReportsDirectory = "<directory-for-offline-reports>";
```

#### OfflineReportsEnabled
Boolean. Default value is **false**. Specifies whether or not Logify should store the last *OfflineReportsCount* reports once an Internet connection is lost. To send the kept reports once an Internet connection is available, call the *SendOfflineReports* method.
```csharp
client.OfflineReportsEnabled = true;
client.OfflineReportsCount = 20; // Keeps the last 20 reports
client.OfflineReportsDirectory = "<directory-for-offline-reports>";
```

#### UserId
String. Specifies a unique user identifier that corresponds to the sent report.
```csharp
client.UserId = "user@myapp.com";
```

### Methods for automatic reporting
Logify Alert allows you to automatically listen to uncaught exceptions and deliver crash reports. For this purpose, use the methods below.

#### StartExceptionsHandling()
Commands Logify Alert to start listening to uncaught exceptions and sends reports for all processed exceptions.
```csharp
client.StartExceptionsHandling();
```

#### StopExceptionsHandling()
Commands Logify Alert to stop listening to uncaught exceptions.
```csharp
client.StopExceptionsHandling();
```

### Methods for manual reporting
Alternatively, Logify Alert allows you to catch required exceptions manually, generate reports based on caught exceptions and send these reports only. For this purpose, use the methods below.

#### Send(Exception e)
Generates a crash report based on the caught exception and sends this report to the Logify Alert service.
```csharp
try {
    RunCode();
}
catch (Exception e) {
    client.Send(e);
}
```

#### Send(Exception e, IDictionary<String, String> additionalCustomData)
Sends the caught exception with specified custom data to the Logify Alert service.
```csharp
try {
    RunCode();
}
catch (Exception e) {
    var data = new Dictionary<String, String>();
    data["FailedOperation"] = "RunCode";
    client.Send(e, data);
}
```

#### SendOfflineReports
Sends all reports saved in the *OfflineReportsDirectory* folder to the Logify Alert service.

### Events

#### BeforeReportException
Occurs before Logify Alert sends a new crash report to the service.

The **BeforeReportException** event occurs between the [CanReportException](#canreportexception) event and sending a new report to the Logify Alert service. If report send is canceled in the [CanReportException](#canreportexception) event's handler, the report is not sent and the **BeforeReportException** event isn't raised.

Handle the **BeforeReportException event** to add custom data to the sent report. To do this, assign the required data to the [CustomData](#customdata) property.
```csharp
LogifyAlert.Instance.BeforeReportException += OnBeforeReportException;

void OnBeforeReportException(object sender, EventArgs e) {
    LogifyAlert.Instance.CustomData["LoggedInUser"] = "Mary";
}
```


#### CanReportException
Occurs between generating a new crash report and raising the [BeforeReportException](#beforereportexception) event.

The **CanReportException** event occurs right after a new report is generated and prepared to be sent to the Logify Alert service. Handle the **CanReportException** event to cancel sending the report. To do this, assign **true** to the appropriate CanReportExceptionEventArgs's **Cancel** property. Thus, the generated report is not posted to the service and the [BeforeReportException](#beforereportexception) isn't raised.
```csharp
LogifyAlert.Instance.CanReportException += OnCanReportException;

void OnCanReportException(object sender, CanReportExceptionEventArgs args) {
    if (args.Exception is MyCustomException)
        args.Cancel = true;
}
```

### Attributes
#### LogifyIgnoreAttribute
Indicates that exceptions thrown from a specific method should not be handled and sent to Logify Alert.

```csharp
[LogifyIgnore(true)]
void ThisMethodShouldNotReportAnyExceptions() {
    RunSomeCode();
}
```

## Custom Clients
If the described client is not suitable for you, you can create a custom one. For this purpose, make sure that the client sends data to the Logify Alert server in the JSON format.

Below there is a raw report’s example, which will be correctly parsed by Logify Alert. The example only uses basic report fields that store information about an application, OS, exceptions, and custom data. The structure of other fields is not in a final state yet, so we recommend that you use the mentioned fields only.

Note: Due to the fact that the report is generated for .NET application, we strongly recommend that you avoid modifying values of the **devPlatform** and the **logifyProtocolVersion** fields.

```
{
 "logifyProtocolVersion": "15.2.12",
 "logifyApp": {
  "name": "A string that specifies your application name",
  "version": "A string that specifies your application version",
  "userId": "A string that specifies a unique identifier of a user that created a report"
 },
 "devPlatform": "dotnet",
 "os": {
  "platform": "A string that specifies your platform name",
  "servicePack": "A string that contains information about a service pack",
  "version": "A string that specifies your OS version",
  "is64bit": true
 },
 "exception": [
  {
   "type": "A string that specifies the main exception’s type",
   "message": "A string that specifies the main exception’s message",
   "stackTrace": "A string that specifies stack lines, for example - at Line1\r\n   at Line2\r\n   at Line3",
   "normalizedStackTrace": "A string that specifies normalized stack lines, for example - Line1\r\nLine2\r\nLine3\r\n"
  },
  {
   "type": "A string that specifies an inner exception’s type",
   "message": "A string that specifies an inner exception’s message",
   "stackTrace": "A string that specifies stack lines, for example - at OtherLine1\r\n   at OtherLine2\r\n   at OtherLine3",
   "normalizedStackTrace": "A string that specifies normalized stack lines, for example - OtherLine1\r\nOtherLine2\r\nOtherLine3\r\n"
  }
 ],
 "customData": {
  "Key1": "Data1",
  "Key2": "Data2"
 }
}
```

If the structure above does not meet all your requirements and you want to use other fields in generated reports, please contact our [Support Team](mailto:support@devexpress.com) for detailed instructions.
