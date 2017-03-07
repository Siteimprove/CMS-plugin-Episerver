param ([string]$config)

$currentLocation = split-path -parent $MyInvocation.MyCommand.Definition
$tempFolder = "$currentLocation\tmp\SiteImprove"

# Create if not exist
New-Item $tempFolder -type directory -Force

# Clear folder
Remove-Item "$tempFolder\*" -Force -Recurse

New-Item "$tempFolder\ClientResources" -type directory -Force
New-Item "$tempFolder\bin" -type directory -Force

# Copy all files
Copy-Item "$currentLocation\ClientResources\*" "$tempFolder\ClientResources" -Recurse -Force
Copy-Item "$currentLocation\Views" "$tempFolder\Views" -Recurse -Force
Copy-Item "$currentLocation\module.config" $tempFolder -Force
Copy-Item "$currentLocation\bin\$config\SiteImprove.EPiserver.Plugin.dll" "$tempFolder\bin" -Force

Compress-Archive -Path "$tempFolder\*" -DestinationPath "$tempFolder.zip" -Force

#Write-Host $config
Write-Host "Done"

