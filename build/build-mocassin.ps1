$Settings = Get-Content -Path .\build-mocassin.json | ConvertFrom-Json

# Auto version by date and set the new deploy folder
$deployRootPath = $Settings.Deploy.RootDirectory, ("Mocassin-" + [System.DateTime]::Now.ToString("yyyy.MM.dd.HHmm")) -join "/"
$Settings.Deploy.RootDirectory = $deployRootPath

# Delte and recreate the deploy folder

Write-Host "Setting up build directory: " $deployRootPath

#New-Item -Force -Path $deployRootPath -ItemType "directory"

# Call local solver build
foreach ($script in $Settings.Scripts) {
    if ($script.IsActive) {
        & $script.Path -Settings $Settings
    }
}