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

# Build the references and fix packages only package "Mocassin" and "Mocassin.Csx" with the nuspec file using nuget
Write-Host "Building additional nuget packages ..."
if ((Test-Path "./raw.nuspec") && (Test-Command nuget)) {

    $nuspecDir = "./nuspecs"

    Get-ChildItem $nuspecDir/* -Recurse -Include "*.nuspec" | ForEach-Object {
        $rawNuspecFile = $_.FullName
        $tmpNuspecFile = $_.Directory, "tmp.nuspec" -join "/"
        $date = [System.DateTime]::Now
        $version = $date.Year, $date.Month, $date.Day -join "."
        $nuspecXml = (Get-Content -Path $rawNuspecFile).Replace("`$version",$version)

        Out-File -FilePath $tmpNuspecFile -InputObject $nuspecXml -Encoding utf8
        & nuget pack $tmpNuspecFile -OutputDirectory $pathToNugetDeploy
        Remove-Item -Recurse $tmpNuspecFile
    } 
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