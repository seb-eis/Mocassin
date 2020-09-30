try {
    Write-Host "Sourcing helper functions ..."
    . "./subscripts/helper-functions.ps1"
}
catch {
    throw "Failed to source helper functions."
}

$Settings = Get-SettingsObject "./build-mocassin.json"
$deployRootPath = $Settings.Deploy.RootDirectory
$deployLogFile = $Settings.Deploy.LogFile
$version = $Settings.Meta.Version.Full
$isTestMode = $Settings.IsTestMode

# Delte and recreate the deploy folder
Write-Host "Build version  : " $version
Write-Host "Build directory: " $deployRootPath
Write-Host "Build log file : " $deployLogFile

New-Item -Force -Path $deployRootPath -ItemType directory | Out-Null
New-Item $deployLogFile -ItemType File | Out-Null

# Call local solver build
Write-Host "Invoking scripts ..."
foreach ($script in $Settings.Scripts) {
    if ($script.IsActive) {
        try {
            Write-Host "Running script : " $script.Path
            & $script.Path -Settings $Settings 3>&1 2>&1 | Write-Log -FilePath $deployLogFile
            Write-Host "Finished script: " $script.Path      
        }
        catch {
            Write-Warning ($script.Path + " has failed with an error: `n`t$_")
        }
    }
}

if ($isTestMode) {
    Remove-Item -Recurse $deployRootPath
}