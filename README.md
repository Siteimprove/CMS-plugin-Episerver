# CMS-plugin-Episerver

Siteimprove Plugins – Your Insights Always Within Reach
The Siteimprove plugin bridges the gap between Episerver and the Siteimprove Intelligence Platform.
Thanks to the seamless integration, you are now able to put your Siteimprove results to use where they are most valuable – during your content creation and editing process.
 
With analytics and content insights always at hand, contributors can test, fix, and optimize their work continuously . Once the detected issues have been assessed, you can directly re-recheck the relevant page when it is published and see if further actions are needed.
Delivering a superior digital experience has never been more efficient and convenient. 
 
About Siteimprove
Siteimprove provides organizations of all shapes and sizes with a solution that makes website management, maintenance, and optimization both easier and more affordable. Not a customer yet? Have a look: www.siteimprove.com

### Nuget repository
 - http://nuget.episerver.com/en/OtherPages/Package/?packageId=SiteImprove.EPiServer.Plugin

### Run locally
Reference class library to project 

Add the following to Post-Build event command line:
``` shell
powershell -executionpolicy Unrestricted -File "$(ProjectDir)bin\movePackage.ps1" $(ConfigurationName)
```

#### Build the package
Run the build\build.bat


