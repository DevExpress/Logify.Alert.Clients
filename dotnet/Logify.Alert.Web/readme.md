# Logify Alert for ASP.NET WebForms and MVC applications
A WebForms and MVC client to report exceptions to [Logify Alert](https://logify.devexpress.com)

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Web/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Web.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Web.svg" style="max-width:100%;" /></a>
```sh
$ nuget install Logify.Alert.Web
```

## Quick Start
### Automatic error reporting
#### Modify Application's Web.config File
Add the Logify Alert settings to the application's **Web.config** file. To initialize your application, use the [API Key](https://logify.devexpress.com/Documentation/CreateApp) generated for it.
```xml
<configuration>
  <configSections>
    <section name="logifyAlert" type="DevExpress.Logify.WebLogifyConfigSection, Logify.Alert.Web"/>
  </configSections>
  <logifyAlert>
    <apiKey value="SPECIFY_YOUR_API_KEY_HERE"/>
  </logifyAlert>
</configuration>
```
Add the Logify.Alert.Web module to the **Modules** section.
```xml
<system.webServer>
  <modules>
    <add name="Logify.Alert.Web" type="DevExpress.Logify.Web.AspExceptionHandler, Logify.Alert.Web" preCondition="managedHandler"/>
  </modules>
</system.webServer>
```
#### Modify Application's WebApiconfig.cs File (WebApi applications only)
Add the following code to the end of the **Register()** method declared in the application's **WebApiconfig.cs** file.
```csharp
public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
        //...
        config.Filters.Add(new DevExpress.Logify.Web.WebApiExceptionHandler());
    }
}
```

That's it. Now, your application will report unhandled exceptions to the Logify Alert service. To manage and view generated reports, use the [Logify Alert](https://logify.devexpress.com) link.

### Manual error reporting
```csharp
using DevExpress.Logify.Web;
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
using DevExpress.Logify.Web;
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    Trace.TraceError("an exception occured", e);
}
```

You can set up the Logify Alert trace listener using the **Web.config** file as follows.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.diagnostics>
    <trace autoflush="true" indentsize="4">
      <listeners>
        <add name="LogifyAlertTraceListener"  type="DevExpress.Logify.Web.LogifyAlertTraceListener, Logify.Alert.Web" />
      </listeners>
    </trace>
  </system.diagnostics>
</configuration>
```

## Configuration
You can set up the Logify Alert client using the **Web.config** file as follows.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="logifyAlert" type="DevExpress.Logify.WebLogifyConfigSection, Logify.Alert.Web" />
  </configSections>
  <logifyAlert>
    <apiKey value="My Api Key" />
    <appName value="My Site" />
    <version value="1.0.5" />
    <customData>
      <add key="MACHINE_NAME" value="My Server" />
    </customData>
  </logifyAlert>
</configuration>
```

## API
### Properties
#### ApiKey
String. Specifies the [API Key](https://logify.devexpress.com/Documentation/CreateApp) used to register the applications within the Logify service.
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

#### CustomData
IDictionary<String, String>. Gets the collection of custom data sent with generated reports.
Use the **CustomData** property to attach additional information to the generated report. For instance, you can use this property to track additional metrics that are important in terms of your application: CPU usage, environment parameters, and so on.

```csharp
client.CustomData["CustomerName"] = "Mary";
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
Indicates that exceptions thrown at a specific method should not be handled and sent by Logify Alert.

```csharp
[LogifyIgnore(true)]
void ThisMethodShouldNotReportAnyExceptions() {
    RunSomeCode();
}
```
