# CMS-plugin-Episerver

### Run locally
Reference class library to project 

Add the following to Post-Build event command line:
``` shell
powershell -executionpolicy Unrestricted -File "$(ProjectDir)bin\movePackage.ps1" $(ConfigurationName)
```

### Build the package
Run the buidl\build.bat

### TODO
 - Upload package to some nuget repository