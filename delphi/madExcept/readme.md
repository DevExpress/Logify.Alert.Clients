# madExcept Library for VCL Applications

Currently, Logify Alert does not provide users with a standalone client. However, you can use the **madExcept** library to integrate Logify Alert into the crash reporting process. To do this, follow the steps below.

1. Make sure you have at least one Logify Alert API key generated. Otherwise, log in to the Logify Alert web interface and follow the instructions from the [Generate the API Key](https://logify.devexpress.com/Documentation/GettingStarted/Step1) document.

2. [Download](http://madshi.net/) and install madExcept library version 4+ into your VCL project.

3. Open the madExcept settings dialog. For this purpose, select **Project | madExcept settings** from the RAD studio's main menu.

4. Enable the library by checking the **enable madExcept** box.

  ![madExcept settings](https://logify.devexpress.com/Content/documentation/madExOptions1.png)

5. Switch to the **email & upload settings** tab. Select **upload to web server** and specify the following information on the corresponding tab:
  * **web server** - logify.devexpress.com/api/madexcept/NewReport?apiKey=API_KEY_FROM_STEP_1
  * **encrypt** - SSL

  ![madExcept settings](https://logify.devexpress.com/Content/documentation/madExOptions2.png)

Now, each time your application crashes unexpectedly, madExcept shows a dialog similar to the one below.

![madExcept dialog](https://logify.devexpress.com/Content/documentation/madExSendBug.png)

Once you click **send bug report** and go through all wizard steps, you will get the corresponding crash report in Logify Alert. Use the [logify.devexpress.com](logify.devexpress.com) page to explore these reports.

Also, it may be useful to disable the madExcept email/notifications functionality so you do not get dozens of notifications and use advanced Logify notification options instead. Please refer to the [E-Mail Notifications](https://logify.devexpress.com/Documentation/SetUpApp/EMail) document for more information.
