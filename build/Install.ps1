param($installPath, $toolsPath, $package, $project)

$projectFullName = $project.FullName
Write-Host "install.ps1 executing for " + $projectFullName
 
$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $projectFullName
$projectDirectory = $fileInfo.DirectoryName

$sourceDirectory = "$projectDirectory\modules\siteimprove\"

If (Test-Path $sourceDirectory){
    Write-Host "Deleted deprecated folder " + $sourceDirectory
    try {
    $project.projectItems.item("modules").projectitems.item("siteimprove").Delete() | Select-Object | Get-ChildItem -Recurse | Remove-Item -force -recurse
    }
    catch {
	    #Remove-Item $sourceDirectory -Recurse
    }
}