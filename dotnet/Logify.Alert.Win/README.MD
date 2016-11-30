# Logify Alert for WinForms and console applications
WinForms and console client to report exceptions to [Logify Alert](https://logify.devexpress.com)

## Install
```sh
$ nuget install Logify.Alert.Win
```

## Quick Start
```csharp
using DevExpress.Logify.Win;

LogifyAlert client = LogifyAlert.Instance;
client.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
client.StartExceptionsHandling();
```

## API
### Properties
#### ApiKey
String. Specifies an [API Key](https://logify.devexpress.com/Documentation/CreateApp) used to register the applications within the Logify service.
```csharp
client.ApiKey = "My Api Key";
```

#### AppName
String. Specifies an application name.
```csharp
client.AppName = "My Application";
```
#### AppVersion
String. Specifies an application version.
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

#### UserId
String. Specifies a unique user identifier that corresponds to a sent report.
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
    var data = new Dictionary<String, String> additionalCustomData();
    data["FailedOperation"] = "RunCode";
    client.Send(e, data);
}
```
### Events

#### BeforeReportException

#### CanReportException