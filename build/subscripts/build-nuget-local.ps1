param (
    [Parameter(Mandatory=$true)]
    [ValidateNotNull()]
    [object]
    $Settings
)

# Variables
$pathToMSbuild = $Settings.Compilers.Windows.MSBuild
$pathToSource = $Settings.Source.ModelBuilderDirectory
$pathToNugetTmp = $Settings.Source.NugetDirectory
$pathToNugetDeploy = $Settings.Deploy.RootDirectory, $Settings.Deploy.RelativeNugetPath -join "\"

Write-Host "Building nuget packages ..."

# Setup directories
if (Test-Path $pathToNugetTmp) {
    Remove-Item -Recurse $pathToNugetTmp
    New-Item -Force -Path $pathToNugetTmp -ItemType "directory"
}

New-Item -Force -Path $pathToNugetDeploy -ItemType "directory"

# Invoke MSBuild for Release and Any CPU to trigger project auto nuget building
& $pathToMSbuild $pathToSource /property:Platform="Any CPU" /property:Configuration="Release" /verbosity:minimal -restore

Write-Host "Completed nuget packages! Copying files to deploy & cleaning up..."

# Copy data to deploy target and clean up
Get-ChildItem $pathToNugetTmp | ForEach-Object {
    $copyDst = $pathToNugetDeploy + "\" + $_.BaseName + $_.Extension
    $copySrc = $_.FullName
    Copy-Item $copySrc -Destination $copyDst
}

Remove-Item -Recurse $pathToNugetTmp