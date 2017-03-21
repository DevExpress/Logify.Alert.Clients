# Logify Alert for [Serilog](https://serilog.net/)

The Serilog logger client for [Logify Alert](https://logify.devexpress.com).

## Install <a href="https://www.nuget.org/packages/Logify.Alert.Serilog/"><img alt="Nuget Version" src="https://img.shields.io/nuget/v/Logify.Alert.Serilog.svg" data-canonical-src="https://img.shields.io/nuget/v/Logify.Alert.Serilog.svg" style="max-width:100%;" /></a>
```ps1
Install-Package Logify.Alert.Serilog
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


Register **LogifyAlert** target as a custom Serilog sink via code:

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
  .WriteTo.LogifyAlert()
  .CreateLogger();
```

... or in the **App.config** configuration file:

```ps1
Install-Package Serilog.Settings.AppSettings
```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="serilog:using:LogifyAlert" value="Logify.Alert.Serilog" />
    <add key="serilog:write-to:LogifyAlert" />
  </appSettings>
</configuration>
```

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
  .ReadFrom.AppSettings()
  .CreateLogger();
```

Now, the use of the Serilog [methods](https://github.com/serilog/serilog/wiki/Writing-Log-Events) sends a report to Logify Alert.
