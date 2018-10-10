# Logify Alert for [Logback](https://logback.qos.ch/)
The Logback logger client for [Logify Alert](https://logify.devexpress.com).

## Install
### Maven
In the *pom.xml* file, do the following:
1. In the *dependencies* section, declare the *logify-alert-logback* library dependency:
    ```xml
    <dependency>
        <groupId>com.devexpress.logify</groupId>
        <artifactId>logify-alert-logback</artifactId>
        <version>1.0.1</version>
    </dependency>
    ```
2. In the *repositories* section, declare the Logify Alert's maven repository:
   ```xml
    <repository>
        <id>any-name-or-id</id>
        <url>https://github.com/DevExpress/Logify.Alert.Clients/raw/maven</url>
    </repository>
    ```

### Gradle
In the *build.gradle* file, do the following:
1. In the *dependencies* section, declare the *logify-alert-logback* library dependency:
    ```jsp
    compile "com.devexpress.logify:logify-alert-logback:1.0.1"
    ```
2. In the *repositories* section, declare the Logify Alert's maven repository:
   ```jsp
    maven {
        url "https://github.com/DevExpress/Logify.Alert.Clients/raw/maven"
    }
    ```

## Quick Start
Set up the Logify Alert's Java client in the *logify.properties* file:
```xml
apiKey=YOUR_API_KEY
appName=YOUR_APP_NAME
appVersion=YOUR_APP_VERSION
userId=YOUR_USER_ID
customData=NAME_1:VALUE_1,NAME_2:VALUE_2,NAME_3:VALUE_3
tags=TAG_1:VALUE_1,TAG_2:VALUE_2
```

Register the *LogifyAlertAppender* as a standard Logback appender in the *logback.xml* configuration file:

```xml
<configuration>
    <timestamp key="byDay" datePattern="yyyyMMdd'T'HHmmss" />

    <appender name="LOGIFYALERT" class="com.devexpress.logify.alert.logback.LogifyAlertAppender" />

    <root level="debug">
        <appender-ref ref="LOGIFYALERT" />
    </root>
</configuration>
```

Now a new report is generated and sent to Logify Alert each time your application calls the [error](https://logback.qos.ch/apidocs/ch/qos/logback/classic/Logger.html#error(java.lang.String,%20java.lang.Throwable)) Logback method with non-empty *ILoggingEvent.getThrowableProxy()* parameter. For example:

```jsp
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class Main {
    private final static Logger logger = LoggerFactory.getLogger(Main.class);

    public static void main(String[] args) {
        logger.error("logback test message", new Throwable("logback test error"));
    }
}
```