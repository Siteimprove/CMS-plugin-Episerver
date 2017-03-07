$currentLocation = Get-Location

Remove-Item "$currentLocation\..\modules\siteimprove\*" -Force -Recurse
Expand-Archive "$currentLocation\tmp\SiteImprove.zip" -Destination (New-Item "$currentLocation\..\modules\siteimprove" -Type Directory -Force) -Force