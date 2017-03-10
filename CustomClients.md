# Custom Clients
If the available clients are not suitable for you, you can create a custom one. For this purpose, make sure that the client sends data to the Logify Alert server in the JSON format.

Below there is a raw report’s example, which will be correctly parsed by Logify Alert. The example only uses basic report fields that store information about an application, OS, exceptions, and custom data. The structure of other fields is not in a final state yet, so we recommend that you use the mentioned fields only.

Note: Due to the fact that the report is generated for .NET application, we strongly recommend that you avoid modifying values of the **devPlatform** and the **logifyProtocolVersion** fields.

```
{
 "logifyProtocolVersion": "15.2.12",
 "logifyApp": {
  "name": "A string that specifies your application name",
  "version": "A string that specifies your application version",
  "userId": "A string that specifies a unique identifier of a user that created a report"
 },
 "devPlatform": "dotnet",
 "os": {
  "platform": "A string that specifies your platform name",
  "servicePack": "A string that contains information about a service pack",
  "version": "A string that specifies your OS version",
  "is64bit": true
 },
 "exception": [
  {
   "type": "A string that specifies the main exception’s type",
   "message": "A string that specifies the main exception’s message",
   "stackTrace": "A string that specifies stack lines, for example - at Line1\r\n   at Line2\r\n   at Line3",
   "normalizedStackTrace": "A string that specifies normalized stack lines, for example - Line1\r\nLine2\r\nLine3\r\n"
  },
  {
   "type": "A string that specifies an inner exception’s type",
   "message": "A string that specifies an inner exception’s message",
   "stackTrace": "A string that specifies stack lines, for example - at OtherLine1\r\n   at OtherLine2\r\n   at OtherLine3",
   "normalizedStackTrace": "A string that specifies normalized stack lines, for example - OtherLine1\r\nOtherLine2\r\nOtherLine3\r\n"
  }
 ],
 "customData": {
  "Key1": "Data1",
  "Key2": "Data2"
 }
}
```

If the structure above does not meet all your requirements and you want to use other fields in generated reports, please contact our [Support Team](mailto:support@devexpress.com) for detailed instructions.
