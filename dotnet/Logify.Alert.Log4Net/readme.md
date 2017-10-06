# Logify Alert for [log4net](https://logging.apache.org/log4net/)

The log4net logger client for [Logify Alert](https://logify.devexpress.com).

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Log4Net/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Log4Net.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Log4Net.svg" style="max-width:100%;" /></a>
```ps1
Install-Package Logify.Alert.Log4Net
```
Also, execute the command below.

WinForms and Console applications:
```ps1
Install-Package Logify.Alert.Win
```
Web applications:
```ps1
Install-Package Logify.Alert.Web
```
WPF applications:
```ps1
Install-Package Logify.Alert.Wpf
```


## Quick Start

Set up the Logify Alert client for your target platform. The **App.config** example below is for a console or WinForms application.
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


Register LogifyAlertAppender as a standard log4net appender, either in the application's App.config/Web.config or a special log4net.config configuration file:

```xml
<log4net>
  <root>
    <level value="ALL"/>
    <appender-ref ref="LogifyAlertAppender"/>
  </root>
  <appender name="LogifyAlertAppender" type="DevExpress.Logify.Alert.Log4Net.LogifyAppender, Logify.Alert.Log4Net">
  </appender>
</log4net>
```

Add the following code to your application's code. The example below demonstrates how to set up the plugin for a console or a WinForms application.

```cs
using DevExpress.Logify.Win;

LogifyAlert client = LogifyAlert.Instance;
```

Now,  each time your application calls the [ILog.Error](http://logging.apache.org/log4net/release/sdk/html/M_log4net_ILog_Error.htm)  log4Net method with non-empty *LoggingEvent.ExceptionObject* parameter, a new report is generated and sent to Logify Alert.
