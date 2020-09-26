param (
    [Parameter(Mandatory=$true)]
    [ValidateNotNull()]
    [object]
    $Settings
)

# Variables
$version = $Settings.Meta.Version.Full
$pathToMSbuild = $Settings.Compilers.Windows.MSBuild
$pathToWixProject = $Settings.Source.ModelBuilderWixProject
$installerDirectory = $Settings.Source.InstallerDirectory
$guiDeployPath = $Settings.Deploy.RootDirectory, $Settings.Deploy.RelativeGuiPath -join "/"

Write-Host "Building windows gui installer ..."

# Create directories
New-Item -Path $guiDeployPath -ItemType directory

# Invoke MSBuild for Release
& $pathToMSbuild $pathToWixProject /p:Platform="x64" /p:Configuration="Release" /p:ProductVersion=$version /p:Version=$version /t:Clean,Build /verbosity:minimal -restore

Write-Host "Completed installer build! Copying files to deploy & cleaning up..."

Get-ChildItem $installerDirectory\* -Include *.msi | ForEach-Object {
    $copySrc = $_
    $copyDst = ($guiDeployPath, $_.BaseName -join "/"), $_.Extension -join ""
    Copy-Item $copySrc -Destination $copyDst
}
