# Logify Alert for [NLog](http://nlog-project.org/)

The NLog logger client for [Logify Alert](https://logify.devexpress.com).

## Install <a href="https://www.nuget.org/packages/Logify.Alert.NLog/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.NLog.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.NLog.svg" style="max-width:100%;" /></a>
```ps1
Install-Package Logify.Alert.NLog
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


Register **LogifyAlert** target as a custom NLog target in the NLog.config configuration file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="Logify.Alert.NLog"/>
  </extensions>
  <targets>
    <target name="logifyAlert" xsi:type="LogifyAlert" />
  </targets>
  <rules>
    <logger name="*" minlevel="Trace" writeTo="logifyAlert" />
  </rules>
</nlog>
```

Add the following code to your application's code. The example below demonstrates how to set up the plugin for a console or a WinForms application.

```cs
using DevExpress.Logify.Win;

LogifyAlert client = LogifyAlert.Instance;
```

Now, each time your application sends an [Error](https://github.com/NLog/NLog/wiki/Tutorial#writing-log-messages) message with non-empty *LogEvent.Exception* parameter to the NLog system, a new report is generated and sent to Logify Alert.
