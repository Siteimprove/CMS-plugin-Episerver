# CMS-plugin-Episerver

The Siteimprove CMS Add-On bridges the gap between the Episerver content management system (CMS) and the Siteimprove Intelligence Platform. Now you can scan your website for errors as soon as a page is published, allowing you to fix mistakes, optimize content, and manage your site more efficiently.

The seamless integration between Siteimprove and Episerver streamlines workflow efficiencies for your web team. With the Add-On, your team can fix errors and optimize content directly within the editing environment. Once the detected issues have been assessed, you can re-check the relevant page in real-time and assess if further actions are needed.

[User Guide](https://cdn2.hubspot.net/hubfs/321800/Partners/Siteimprove%20Episerver%20Add-On%20User%20Guide.pdf)

The Siteimprove CMS Add-On provides insights into*:
* Misspellings and broken links
* Readability levels
* Accessibility issues (A, AA, AAA conformance level)
* High-priority policies
* SEO: technical, content, UX, and mobile 
* Page visits and page views
* Feedback rating and comments

*Data shown in the Siteimprove CMS Add-On depends on the Siteimprove services you are subscribed to.

About Siteimprove:

Siteimprove's cloud-based software provides eye-opening insights that empower you and your team to understand, prioritize, and optimize the performance of your website and beyond. With the world’s most comprehensive Digital Presence Optimization (DPO) solution, we provide the clarity and direction needed to run a high-performance website. More than 7,000 organizations around the world trust the Siteimprove Intelligence Platform (SIP) to perfect their digital presence. Learn why at siteimprove.com.


### Nuget repository
#### Episerver 10
 - http://nuget.episerver.com/en/OtherPages/Package/?packageId=SiteImprove.EPiServer.Plugin
#### Episerver 11
 - http://nuget.episerver.com/en/OtherPages/Package/?packageId=SiteImprove.EPiServer11.Plugin

### Run locally
Reference class library to project 

Add the following to Post-Build event command line:
``` shell
powershell -executionpolicy Unrestricted -File "$(ProjectDir)bin\movePackage.ps1" $(ConfigurationName)
```

#### Build the package
Run the build\build.bat

### Installation
Run the following command in the NuGet Package Manager Console for your website:
```
Install-Package SiteImprove.EPiServer.Plugin
```
For Episerver v11:
```
Install-Package SiteImprove.EPiServer11.Plugin
```

You need to add the EPiServer NuGet Feed to Visual Studio (see https://nuget.episerver.com/feed/)

### Configuration

We allow the following groups access:
* Administrators, WebAdmins, CmsAdmins, SiteimproveAdmins

SiteimproveAdmins is a custom group, where you can assign any group in your solution
