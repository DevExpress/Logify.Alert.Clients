# Logify Alert for .NET Core web applications (Web, MVC, WebApi)
A .NET Core Web client to report exceptions to [Logify Alert](https://logify.devexpress.com).

Starting with the package version v1.0.37, the .Net Standard v2.0 target framework is only supported.

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Web/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Web.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Web.svg" style="max-width:100%;" /></a>
```sh
$ Install-Package Logify.Alert.Web
```

## Quick Start
### Automatic error reporting
#### Add LogifyAlert.json configuration File
Add the Logify Alert settings to the application's **LogifyAlert.json** file. To initialize your application, use the [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) generated for it.
```json
{
  "LogifyAlert": {
    "apiKey": "SPECIFY_YOUR_API_KEY_HERE",
    "customData": {
      "MACHINE_NAME": "My Machine"
    }
  }
}
```

#### Modify Application's Startup.cs file
Add the following code to the **BuildWebHost()** method declared in the application's **Program.cs** file.
```csharp
WebHost.CreateDefaultBuilder(args)  
    .ConfigureAppConfiguration((context, builder) => { 
        builder.AddJsonFile("LogifyAlert.json", optional: true, reloadOnChange: false); // <-- add this line
    })  
    .UseStartup<Startup>()
    .Build();
```

Add the following code to the **Configure()** method declared in the application's **Startup.cs** file.
```csharp
using DevExpress.Logify.Web;
...
public static void Register(HttpConfiguration config) {
    //...
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }
    // You should put Logify Alert initialization after app.UseExceptionHandler call
    app.UseLogifyAlert(Configuration.GetSection("LogifyAlert"));
}
```

That's it. Now, your application will report unhandled exceptions to the Logify Alert service. To manage and view generated reports, use the [Logify Alert](https://logify.devexpress.com) link.

### Manual error reporting
```csharp
using DevExpress.Logify.Web;
...
try {
    LogifyAlert.Instance.ApiKey = "SPECIFY_YOUR_API_KEY_HERE";
    RunYourCode();
}
catch (Exception e) {
    LogifyAlert.Instance.Send(e);
}
```

## Configuration
You can set up the Logify Alert client using the configuration file as follows.
#### JSON configuration file:
```json
{
  "LogifyAlert": {
    "apiKey": "SPECIFY_YOUR_API_KEY_HERE",
    "appName": "Your application name",
    "appVersion": "1.0.2",
    "customData": {
      "MACHINE_NAME": "My Server"
    }
  }
}
```

```csharp
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("LogifyAlert.json", optional: false, reloadOnChange: false);

IConfigurationRoot configuration = builder.Build();
LogifyAlert client = LogifyAlert.Instance;
client.Configure(configuration.GetSection("LogifyAlert"));
```

#### XML configuration file:

_Requires the Microsoft.Extensions.Configuration.Xml package_

```xml
<configuration>
  <LogifyAlert>
    <ApiKey>SPECIFY_YOUR_API_KEY_HERE</ApiKey>
    <AppName>Your application name</AppName>
    <AppVersion>1.0.2</AppVersion>
    <CustomData>
      <MACHINENAME>My Server</MACHINENAME>
    </CustomData>
  </LogifyAlert>
</configuration>
```

```csharp
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddXmlFile("LogifyAlert.xml", optional: false, reloadOnChange: false);

IConfigurationRoot configuration = builder.Build();
LogifyAlert client = LogifyAlert.Instance;
client.Configure(configuration.GetSection("LogifyAlert"));
```

#### INI configuration file:

_Requires the Microsoft.Extensions.Configuration.Ini package_

```ini
[LogifyAlert]
ApiKey = SPECIFY_YOUR_API_KEY_HERE
AppName = Your application name
ApiVersion = 1.0.2
```

```csharp
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddIniFile("LogifyAlert.ini", optional: false, reloadOnChange: false);

IConfigurationRoot configuration = builder.Build();
LogifyAlert client = LogifyAlert.Instance;
client.Configure(configuration.GetSection("LogifyAlert"));
```

## API
### Properties
#### ApiKey
String. Specifies the [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) used to register the applications within the Logify service.
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

#### Breadcrumbs
BreadcrumbCollection. Specifies a collection of manual breadcrumbs attached to a report. The total breadcrumbs size is limited by 1000 instances (or **3 Mb**) per one crash report by default. To change the maximum allowed size of attached breadcrumbs, use the *BreadcrumbsMaxCount* property.
```csharp
using DevExpress.Logify.Core;
using DevExpress.Logify.Web;

LogifyAlert.Instance.Breadcrumbs.Add(new Breadcrumb() { 
  DateTime = DateTime.UtcNow, 
  Event = BreadcrumbEvent.Manual, 
  Message = "A manually added breadcrumb" 
});
```

#### BreadcrumbsMaxCount
Integer. Specifies the maximum allowed number of breadcrumbs attached to one crash report. The default value is 1000 instances (or 3 MB).
```csharp
LogifyAlert.Instance.BreadcrumbsMaxCount = 2000;
```

#### CollectBreadcrumbs
Boolean. Specifies whether automatic breadcrumbs collecting is enabled. The default value is **false**.
The total breadcrumbs size is limited by 1000 instances (or **3 Mb**) per one crash report by default. To change the maximum allowed size of attached breadcrumbs, use the *BreadcrumbsMaxCount* property.
```csharp
LogifyAlert.Instance.CollectBreadcrumbs = true;  
```

#### CustomData
IDictionary<String, String>. Gets the collection of custom data sent with generated reports.
Use the **CustomData** property to attach additional information to the generated report. For instance, you can use this property to track additional metrics that are important in terms of your application: CPU usage, environment parameters, and so on. The field name can only consists of a-z, A-Z, 0-9, and _ characters.

```csharp
client.CustomData["CustomerName"] = "Mary";
```

#### Tags
IDictionary<String, String>. Gets the collection of tags specifying additional fields from a raw report, which will be used in auto ignoring, filtering or detecting duplicates. A key is a tag name (a string that consists of a-z, A-Z, 0-9, and _ characters), and a value is a tag value that is saved to a report. A new tag is added with **Allow search** enabled.

```csharp
client.Tags["OS"] = "Win8";
```

#### IgnoreCookies
String. Specifies cookies that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several cookies. Set this property to the asterisk (\*) character to remove all cookies from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a cookie to be ignored. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreCookies = "*";
```

#### IgnoreFormFields
String. Specifies form fields that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several form fields. Set this property to the asterisk (\*) character to remove all form data from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a form field to be ignored. For example, when you set *IgnoreFormFields ="\*Password\*"*, Logify Alert will ignore all form fields containing the *“password”* in the name. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreFormFields = "creditcard,password";
```

#### IgnoreHeaders
String. Specifies request headers that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several headers. Set this property to the asterisk (\*) character to remove all headers from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a header to be ignored. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreHeaders = "*";
```

#### IgnoreServerVariables
String. Specifies server variables that should be excluded from a crash report.  
Provide a comma separated list of names to ignore several server variables. Set this property to the asterisk (\*) character to remove all server variables from a Logify Alert report.  Also, use the asterisk (\*) character as a wildcard to substitute any character(s) when specifying a server variable to be ignored. This property is case-insensitive.

```csharp
LogifyAlert.Instance.IgnoreServerVariables = "HTTP*";
```

#### IgnoreRequestBody
Boolean. Specifies whether raw request body content should be ignored. 

```csharp
LogifyAlert.Instance.IgnoreRequestBody = true;
```

#### Instance
Singleton. Returns the single instance of the LogifyAlert class.
```csharp
LogifyAlert client = LogifyAlert.Instance;
```

#### OfflineReportsCount
Integer. Specifies the number of last reports to be kept once an Internet connection is lost. Reports are only saved when the *OfflineReportsEnabled* property is set to **true**.
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

#### ProxyCredentials
ICredentials. Specifies proxy credentials (a user name and a password) to be used by Logify Alert to authenticate within your system proxy server. The use of this property resolves the "*407 Proxy Authentication Required*" proxy error.
```csharp
client.ProxyCredentials = new NetworkCredential("MyProxyUserName", "MyProxyPassword");
```

#### UserId
String. Specifies a unique user identifier that corresponds to the sent report.
```csharp
client.UserId = "user@myapp.com";
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

#### AfterReportException
Occurs after Logify Alert sends a new crash report to the service.

```csharp
LogifyAlert.Instance.AfterReportException += OnAfterReportException;

void OnAfterReportException(object sender, EventArgs e) {
  MessageBox.Show("A new crash report has been sent to Logify Alert", "Crash report", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
```

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

## Custom Clients
If the described client is not suitable for you, you can create a custom one. For more information, refer to the [Custom Clients](https://github.com/DevExpress/Logify.Alert.Clients/blob/develop/CustomClients.md) document.
