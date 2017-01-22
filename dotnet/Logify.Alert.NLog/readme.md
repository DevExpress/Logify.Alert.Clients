# Logify Alert for [NLog](http://http://nlog-project.org/)

The NLog logger client for [Logify Alert](https://logify.devexpress.com).

## Install <a href="https://www.nuget.org/packages/Logify.Alert.NLog/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.NLog.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.NLog.svg" style="max-width:100%;" /></a>
```ps1
Install-Package Logify.Alert.Client.NLog
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

Now, the use of the NLog [methods](https://github.com/NLog/NLog/wiki/Tutorial#writing-log-messages) sends a report to Logify Alert.
