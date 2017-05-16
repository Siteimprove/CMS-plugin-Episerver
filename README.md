# CMS-plugin-Episerver

### Run locally
Reference class library to project 

Add the following to Post-Build event command line:
``` shell
powershell -executionpolicy Unrestricted -File "$(ProjectDir)bin\movePackage.ps1" $(ConfigurationName)
```

### Build the package
Run the buidl\build.bat

### Nuget repository
 - http://nuget.episerver.com/en/OtherPages/Package/?packageId=SiteImprove.EPiServer.Plugin
