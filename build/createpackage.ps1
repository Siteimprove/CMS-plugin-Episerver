param (
    [Parameter(Mandatory = $false, HelpMessage = 'Major, Minor, or Patch?')][ValidateSet('Major', 'Minor', 'Patch')][string]$releaseType
)

function Get-ReleaseType { 
    $title = 'Releasing project'
    $message = 'What type of release are you making?'

    $majorRelease = New-Object System.Management.Automation.Host.ChoiceDescription '&1 Major', `
        'Choose Major for huge breaking changes.'

    $minorRelease = New-Object System.Management.Automation.Host.ChoiceDescription '&2 Minor', `
        'Choose Minor for new functionality or small changes.'

    $patchRelease = New-Object System.Management.Automation.Host.ChoiceDescription '&3 Patch', `
        'Choose Patch for bugfixes.'

    $options = [System.Management.Automation.Host.ChoiceDescription[]]($majorRelease, $minorRelease, $patchRelease)

    $result = $host.ui.PromptForChoice($title, $message, $options, 2) 

    switch ($result) {
        0 {Write-Output 'Major'}
        1 {Write-Output 'Minor'}
        2 {Write-Output 'Patch'}
    }
}


function Get-PlatformVersion { 
    $title = 'Releasing project'
    $message = 'Which versions of episerver is affected??'

    $majorRelease = New-Object System.Management.Automation.Host.ChoiceDescription '&0 Episerver V10', `
        'Create package for Episerver >= V10.'

    $minorRelease = New-Object System.Management.Automation.Host.ChoiceDescription '&1 Episerver V11', `
        'Create package for Episerver >= V11.'

    $options = [System.Management.Automation.Host.ChoiceDescription[]]($majorRelease, $minorRelease)

    $result = $host.ui.PromptForChoice($title, $message, $options, 1) 

    switch ($result) {
        0 {Write-Output '10'}
        1 {Write-Output '11'}
    }
}


#Calling the Get-ProjectType if it is missing
if (!$releaseType) {
    $releaseType = Get-ReleaseType
}

$platform = Get-PlatformVersion

Push-Location $PSScriptRoot

if($platform -eq "10")
{
    $fileName = Get-ChildItem -Filter "buildV10.xml" -Recurse
}
else
{
    $fileName = Get-ChildItem -Filter "buildV11.xml" -Recurse
}
$xmlDoc = [System.Xml.XmlDocument](Get-Content $fileName.FullName)

$node = $xmlDoc.Project.PropertyGroup.ChildNodes
foreach ($properties in $node) {
    if ($properties.Name -eq "VersionMajor") {
        $majorNumber = [int]$properties.'#text'
        if ($releaseType -eq "Major") {
            $majorNumber = $majorNumber + 1
            $properties.set_InnerXML($majorNumber) 

            $minorNumber = 0
            $patchNumber = 0
        }
    }

    if ($properties.Name -eq "VersionMinor") {
        $minorNumber = [int]$properties.'#text'
        if($releaseType -eq "Minor")
        {
            $minorNumber = $minorNumber + 1
            $patchNumber = 0
        }
    } 
    
    if ($properties.Name -eq "VersionMinor") {
        if ($minorNumber -or $minorNumber -eq 0) {
            $properties.set_InnerXML($minorNumber) 
        }
    }
    if ($properties.Name -eq "VersionPatch") {
        $patchNumber = [int]$properties.'#text'
        if($releaseType -eq "Patch")
        {
            $patchNumber = $patchNumber + 1
        }
    }

    if ($properties.Name -eq "VersionPatch") 
    {
        if ($patchNumber -or $patchNumber -eq 0) {
            $properties.set_InnerXML($patchNumber) 
        }
    }
}

# Save File
$xmlDoc.Save($fileName.FullName)

$newAssemblyInfoPath = "$majorNumber.$minorNumber.$patchNumber"


& $env:windir\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe $fileName.Name

if($platform -eq "10")
{
    & .\nuget.exe push "package/SiteImprove.EPiServer.Plugin.$newAssemblyInfoPath.nupkg"  -Source http://nuget.nmester.dk:3311/api
}
else
{
    & .\nuget.exe push "package/SiteImprove.EPiServer11.Plugin.$newAssemblyInfoPath.nupkg"  -Source http://nuget.nmester.dk:3311/api
}

