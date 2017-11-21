# Logify Alert for PHP applications
A PHP client to report exceptions to [Logify Alert](https://logify.devexpress.com).

* Copy the [Logify](https://github.com/DevExpress/LogifyAlertClientforPHP/tree/master/Logify) folder to your PHP project.
* Use the code below to include the **LoadHelper.php** file to the PHP script you use to call the Logify API. 
```require_once('/Logify/LoadHelper.php');```.
* Register the library autoloader by executing the following code:
```PHP
spl_autoload_register(array("DevExpress\LoadHelper", "LoadModule"));
```
All classes in the library are wrapped in the DevExpress\Logify namespace. Apply the [use](http://php.net/manual/en/language.namespaces.importing.php) operator as demonstrated below to get rid of long names in your code:
```PHP
use DevExpress\Logify\LogifyAlertClient;
use DevExpress\Logify\Core\Attachment;
```

## Quick Start

### Automatic error reporting
```PHP
use DevExpress\Logify\LogifyAlertClient;
    
    $client = LogifyAlertClient::get_instance();
    $client->apiKey = 'SPECIFY_YOUR_API_KEY_HERE';
    $client->start_exceptions_handling();
```

### Manual error reporting
```PHP
use DevExpress\Logify\LogifyAlertClient;
    
    try {
        $client = LogifyAlertClient::get_instance();
        $client->apiKey = 'SPECIFY_YOUR_API_KEY_HERE';
    }
    catch (Exception $e) {
        $client->send($e);
    }
```


## Configuration
You can set up the Logify Alert client using the **config.php** file:
```PHP
<?php
class LogifyAlert{
    public $settings = array(
        'apiKey' => 'SPECIFY_YOUR_API_KEY_HERE',
        'userId' => 'php user',
        'appName' => 'Test PHP Application',
        'appVersion' => '0.0.0.1',
    );
    public $collectExtensions = true;
    public $offlineReportsCount = 10;
    public $offlineReportsDirectory = '<directory-for-offline-reports>';
    public $offlineReportsEnabled = true;

    public $globalVariablesPermissions = array(
        'get' => true,
        'post' => true,
        'cookie' => true,
        'files' => true,
        'enviroment' => true,
        'request' => true,
        'server' => true,
    );
}
?>
```
Execute the code below so the Logify client uses the created configuration file.
```PHP
    $client->pathToConfigFile = '/config.php';
```


## API

### Properties

#### apiKey
String. Specifies an [API Key](https://logify.devexpress.com/Alert/Documentation/CreateApp) used to register an application within the Logify service.
```PHP
$client->apiKey = 'SPECIFY_YOUR_API_KEY_HERE';
```

#### appName
String. Specifies the application name.
```PHP
$client->appName = 'My Application';
```

#### appVersion
String. Specifies the application version.
```PHP
$client->appVersion = '0.0.0.1';
```

#### attachments
Attachments collection. Specifies a collection of files attached to a report. The total attachments size must not exceed **3 Mb** per crash report. The attachment name must be unique within a crash report.

```PHP
use DevExpress\Logify\LogifyAlertClient;
use DevExpress\Logify\Core\Attachment;

    $client = LogifyAlertClient::get_instance();
    $client->apiKey = 'SPECIFY_YOUR_API_KEY_HERE';
  
    $attachment = new Attachment();
    $attachment->name = "My attachment's unique name per one report";
    $attachment->content = file_get_contents('C:\Work\Image_to_attach.jpg');
  
    // We strongly recommend that you specify the attachment type.
    $attachment->mimeType = 'image/jpeg';
    
    $client->attachments = $attachments;
```

#### customData
Array. Gets the collection of custom data sent with generated reports. Use the **customData** property to attach additional information to the generated report. For instance, you can use this property to track additional metrics that are important regarding your application: CPU usage, environment parameters, etc.

```PHP
    $customData = array('CustomerName' => 'Mary');
    $client->customData = $customData;
```

#### userId
String. Specifies a unique user identifier that corresponds to the sent report.
```PHP
    $client->userId = "user@myapp.com";
```

#### globalVariablesPermissions
Array. Specifies configuration values used to disable the $GLOBALS system values collection.

**Note**: Before collecting $GLOBALS system values, make sure none of them stores personal or private data (passwords, logins, and etc.).

```PHP
$client->globalVariablesPermissions = array(
    'get' => true,
    'post' => true,
    'cookie' => true,
    'files' => true,
    'enviroment' => true,
    'request' => true,
    'server' => true,
);
```

```PHP
$client->globalVariablesPermissions['get'] = true;
```
Boolean. The default value is **false*. Specifies whether the **$_GET** array's data is collected and sent to the server.

```PHP
$client->globalVariablesPermissions['post'] = true;
```
Boolean. The default value is **false**. Specifies whether the **$_POST** array's data is collected and sent to the server.

```PHP
$client->globalVariablesPermissions['cookie'] = true;
```
Boolean. The default value is **false**. Specifies whether the **$_COOKIE** array's data is collected and sent to the server.

```PHP
$client->globalVariablesPermissions['files'] = true;
```
Boolean. The default value is **false**. Specifies whether the **$_FILES** array's data is collected and sent to the server.

```PHP
$client->globalVariablesPermissions['enviroment'] = true;
```
Boolean. The default value is **false**. Specifies whether the **$_ENV** array's data is collected and sent to the server.

```PHP
$client->globalVariablesPermissions['request'] = true;
```
Boolean. The default value is **false**. Specifies whether the **$_REQUEST** array's data is collected and sent to the server.

```PHP
$client->globalVariablesPermissions['server'] = true;
```
Boolean. The default value is **false**. Specifies whether the **$_SERVER** array's data is collected and sent to the server.

#### pathToConfigFile
String. Specifies the configuration file path. Refer to the Configuration section above for more information on the configuration file structure.
```PHP
    $client->pathToConfigFile = '/config.php';
```

#### collectExtensions
```PHP
    $client->collectExtensions = true;
```
Boolean. The default value is **false**. Specifies whether loaded PHP extensions are collected and sent to the server.

#### offlineReportsCount
```PHP
    $client->offlineReportsCount = 20;
```
Integer. Specifies how many previous reports are kept once an Internet connection is lost. Reports are only saved when the OfflineReportsEnabled property is set to **true*.
```PHP
    $client->offlineReportsEnabled = true;
    $client->offlineReportsCount = 20;
```

#### offlineReportsDirectory
```PHP
    $client->offlineReportsDirectory = '<directory-for-offline-reports>';
```
String. Specifies a directory path for storing reports once an Internet connection is lost. Reports are only saved when the OfflineReportsEnabled property is set to **true**.
```PHP
    $client->offlineReportsEnabled = true;
    $client->offlineReportsDirectory = '<directory-for-offline-reports>';
```

#### offlineReportsEnabled
```PHP
    $client->offlineReportsEnabled = true;
```
Boolean. The default value is **false**. Specifies if Logify should store the last offlineReportsCount reports once an Internet connection is lost. Call the send_offline_reports method to send the kept reports once an Internet connection is available.
```PHP
    $client->offlineReportsEnabled = true;
    $client->offlineReportsCount = 20; // Keeps the last 20 reports
    $client->offlineReportsDirectory = "<directory-for-offline-reports>";
```

### Static Methods

#### get_instance
Returns a single instance of the LogifyAlert class.
```PHP
    $client = LogifyAlertClient::get_instance();
```

### Methods for automatic reporting
Logify Alert allows you to listen to uncaught exceptions and deliver crash reports automatically using the methods below.

#### start_exceptions_handling()
Commands Logify Alert to start listening to uncaught exceptions and sends reports for all processed exceptions.
```PHP
    $client->start_exceptions_handling();
```

#### stop_exceptions_handling()
Commands Logify Alert to stop listening to uncaught exceptions.
```PHP
    $client->stop_exceptions_handling();
```

### Methods for manual reporting
Alternatively, Logify Alert allows you to catch the required exceptions manually, generate reports based on caught exceptions and send these reports only. For this purpose, use the methods below.

#### send(Exception $ex)
Generates a crash report based on the caught exception and sends this report to the Logify Alert service.
```PHP
try {
    RunCode();
}
catch (Exception $ex) {
    $client->send($ex);
}
```

#### send(Exception $ex, $customData)
Sends the caught exception with the specified custom data to the Logify Alert service.
```PHP
try {
    RunCode();
}
catch (Exception $ex) {
    $customdata = array('FailedOperation' => 'RunCode');
    $client->send($ex, $customdata);
}
```

#### send(Exception $ex, $customData, $attachments)
Sends the caught exception with the specified custom data and attachments to the Logify Alert service.
```PHP
use DevExpress\Logify\Core\Attachment;

try {
  RunCode();
}
catch (Exception $ex) {
  $customdata = array('FailedOperation' => 'RunCode');
  
  $attachment = new Attachment();
  $attachment->name = "My attachment's unique name per one report";
  $attachment->content = file_get_contents('C:\Work\Image_to_attach.jpg');
  // We strongly recommend that you specify the attachment type.
  $attachment->mimeType = 'image/jpeg';
  $attachments = array($attachment);

  $client->send($ex, $customdata, $attachments);
}
```

#### send_offline_reports()
Sends all reports saved in the offlineReportsDirectory folder to the Logify Alert service.


### Callbacks

#### set_can_report_exception_callback(callable $canReportExceptionHandler)
Occurs between generating a new crash report and calling the $beforeReportExceptionHandler.

The **set_can_report_exception_callback** event occurs right after a new report is generated and ready to be sent to the Logify Alert service. Handle this event by returning **false** from your function to cancel sending the report. Thus, the generated report is not sent to the service and the **set_before_report_exception_callback* is not raised.
```PHP
use DevExpress\Logify\LogifyAlertClient;

$client = LogifyAlertClient::get_instance();
$client->start_exceptions_handling();
$client->set_can_report_exception_callback('can_report_exception');

function can_report_exception($exception){
    if($exception instanceof Error){
        return false;
    }
    return true;
}
```

#### set_before_report_exception_callback(callable $beforeReportExceptionHandler)
Occurs before Logify Alert sends a new crash report to the service.

The **set_before_report_exception_callback** event occurs between the **set_can_report_exception_callback** event and sending a new report to the Logify Alert service. If sending the report is canceled in the **set_can_report_exception_callback** event's handler, the report is not sent and the **set_before_report_exception_callback** event is not raised.
Use the **set_can_report_exception_callback** event's handler to add custom data or an attachment to the sent report by assigning the required data to the **customData** or **attachments** property.
```PHP 
use DevExpress\Logify\LogifyAlertClient;
use DevExpress\Logify\Core\Attachment;

$client->set_before_report_exception_callback('before_report_exception');
 
function before_report_exception(){
    $client = LogifyAlertClient::get_instance();
 
    $attachment = new Attachment();
    $attachment->name = 'My attachment's unique name per one report';
	$attachment->mimeType = 'image/jpeg';
	$attachment->content = file_get_contents(''C:\Work\Image_to_attach.jpg'');;
	$client->attachments = array($attachment);
 
	$client->customData = array('CustomerName' => 'Mary');
 
}
```

#### set_after_report_exception_callback(callable $afterReportExceptionHandler)
Occurs after Logify Alert sends a new crash report to the service.
The $afterReportExceptionHandler's **$response** parameter is **true** if the report has been sent successfully. Otherwise, the **$response** parameter contains the error message that occurred during sending.

```PHP 
use DevExpress\Logify\LogifyAlertClient;

$client = LogifyAlertClient::get_instance();
$client->start_exceptions_handling();
$client->set_after_report_exception_callback('after_report_exception');

function after_report_exception($response){
    if($response !== true){
        echo $response.'<br />';
    }
}
```

## Custom Clients
You can create a custom client if the described one is not suitable. Refer to the [Custom Clients](https://github.com/DevExpress/Logify.Alert.Clients/blob/develop/CustomClients.md) document for more information.
