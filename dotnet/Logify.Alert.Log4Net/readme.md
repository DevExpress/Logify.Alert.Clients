# Logify Alert for [log4net](https://logging.apache.org/log4net/)

The log4net logger client for [Logify Alert](https://logify.devexpress.com).

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Log4Net/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Log4Net.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Log4Net.svg" style="max-width:100%;" /></a>
```ps1
Install-Package Logify.Alert.Log4Net
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

Now, the use of the log4net [ILog.Error](http://logging.apache.org/log4net/release/sdk/html/M_log4net_ILog_Error.htm) method sends a report to Logify Alert.
