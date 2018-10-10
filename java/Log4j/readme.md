# Logify Alert for [log4j](https://logging.apache.org/log4j/1.2/)
The log4j logger client for [Logify Alert](https://logify.devexpress.com).

## Install
### Maven
In the *pom.xml* file, do the following:
1. In the *dependencies* section, declare the *logify-alert-log4j* library dependency:
    ```xml
    <dependency>
        <groupId>com.devexpress.logify</groupId>
        <artifactId>logify-alert-log4j</artifactId>
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
1. In the *dependencies* section, declare the *logify-alert-log4j* library dependency:
    ```jsp
    compile "com.devexpress.logify:logify-alert-log4j:1.0.1"
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

Register the *LogifyAlertAppender* as a standard log4j appender in the *log4j.properties* configuration file:

```xml
log4j.rootLogger=logifyalert
...
log4j.appender.logifyalert=com.devexpress.logify.alert.log4j.LogifyAlertAppender
```

Now a new report is generated and sent to Logify Alert each time your application calls the [error](https://logging.apache.org/log4j/1.2/apidocs/org/apache/log4j/Category.html#error(java.lang.Object,%20java.lang.Throwable)) log4j method with non-empty *LoggingEvent.getThrowableInformation()* parameter. For example:

```jsp
import org.apache.log4j.*;

public class Main {
    private final static Logger logger = LogManager.getRootLogger();

    public static void main(String[] args) {
    // ...
    logger.error("log4j test message", new Throwable("log4j test error"));
    }
}
```