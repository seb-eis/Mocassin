param (
    [Parameter(Mandatory=$true)]
    [ValidateNotNull()]
    [object]
    $Settings
)

# Variables
$pathToMSbuild = $Settings.Compilers.Windows.MSBuild
$pathToSource = $Settings.Source.ModelBuilderDirectory

# Invoke MSBuild for Release
& $pathToMSbuild $pathToSource /property:Platform="x64" /property:Configuration="Release" -restore -m