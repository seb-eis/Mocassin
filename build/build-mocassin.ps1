$Settings = Get-Content -Path .\build-mocassin.json | ConvertFrom-Json

# Variables
$deployRootPath = $Settings.Deploy.RootDirectory

# Delte and recreate the deploy folder

Write-Host "Setting up empty build directory: " $deployRootPath

if (Test-Path $deployRootPath) {
    Remove-Item -Force -Recurse $deployRootPath   
}
New-Item -Force -Path $deployRootPath -ItemType "directory"

# Call local solver build
foreach ($script in $Settings.Scripts) {
    & $script -Settings $Settings
}