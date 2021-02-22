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
& $pathToMSbuild $pathToSource /property:AutoPack="true" /property:Platform="Any CPU" /property:Configuration="Release" /verbosity:minimal -restore

# Build the references only package "Mocassin" with the nuspec file using nuget
Write-Host "Building refernce nuget package ..."
if ((Test-Path "./raw.nuspec") && (Test-Command nuget)) {
    $rawNuspecFile = "./raw.nuspec"
    $tmpNuspecDir = "./.tmpnuspec/"
    $tmpNuspecFile = "$tmpNuspecDir/mocassin.nuspec"
    $date = [System.DateTime]::Now
    $version = $date.Year, $date.Month, $date.Day -join "."
    $nuspecXml = (Get-Content -Path $rawNuspecFile).Replace("`$version",$version)

    New-Item -ItemType Directory $tmpNuspecDir -Force
    Copy-Item "./icon.png" "$tmpNuspecDir/icon.png"
    Out-File -FilePath $tmpNuspecFile -InputObject $nuspecXml -Encoding utf8
    & nuget pack $tmpNuspecFile -OutputDirectory $pathToNugetDeploy
    Remove-Item -Recurse $tmpNuspecDir
}
else {
    Write-Host "Either raw nuspec or nuget command is missing. Cannot build refernce package."
}

Write-Host "Completed nuget packages! Copying files to deploy & cleaning up..."

# Copy data to deploy target and clean up
Get-ChildItem $pathToNugetTmp | ForEach-Object {
    $copyDst = $pathToNugetDeploy + "\" + $_.BaseName + $_.Extension
    $copySrc = $_.FullName
    Copy-Item $copySrc -Destination $copyDst
}

Remove-Item -Recurse $pathToNugetTmp