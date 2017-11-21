# EurekaLog Library for Delphi Applications

Currently, Logify Alert does not provide users with a standalone client for Delphi applications. However, you can use the **EurekaLog** library to integrate Logify Alert into the crash reporting process. To do this, follow the steps below.

1. Make sure you have at least one Logify Alert API key generated. Otherwise, log in to the Logify Alert web interface and follow the instructions from the [Generate the API Key](https://logify.devexpress.com/Alert/Documentation/BasicSetup/Delphi) document.

2. [Download](https://www.eurekalog.com/downloads_delphi.php) and install EurekaLog library version 7+ into your Delphi project.

3. Open the EurekaLog settings dialog. For this purpose, select **Project | EurekaLog options** from the RAD studio's main menu.

4. Enable the library by checking the **Activate EurekaLog** box.

  ![EurekaLog settings](https://logify.devexpress.com/Content/documentation/EurekaOptions1.png)

5. Switch to the **Bug Report | Content** options tab. Specify information to be added to Logify Alert bug reports.

  ![EurekaLog settings](https://logify.devexpress.com/Content/documentation/EurekaOptions2.png)

6. Switch to the **Anvanced | Code | Send Engines** options tab and click **Customize**.

  ![EurekaLog settings](https://logify.devexpress.com/Content/documentation/EurekaOptions3.png)
 
 7. Select **HTTP upload** and specify the following information on the corresponding tab:

  * check the **[V] SSL/TLS** box
  * specify the following URL - logify.devexpress.com/api/eurekalog/NewReport?apiKey=API_KEY_FROM_STEP_1
  
  ![EurekaLog settings](https://logify.devexpress.com/Content/documentation/EurekaOptions4.png)
  
  8. Click **OK**.
  
Now, each time your application crashes unexpectedly, EurekaLog shows a dialog similar to the one below.

![EurekaLog dialog](https://logify.devexpress.com/Content/documentation/EurekaSendDialog.png)

Once you click **Send Error Report** and go through all wizard steps, you will get the corresponding crash report in Logify Alert. Use the [logify.devexpress.com](logify.devexpress.com) page to explore these reports.

Also, it may be useful to disable the EurekaLog email/notifications functionality so you do not get dozens of notifications and use advanced Logify notification options instead. Please refer to the [E-Mail Notifications](https://logify.devexpress.com/Alert/Documentation/Notifications/EMail) document for more information.
