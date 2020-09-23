param ([object]$Settings)

if ($null -eq $Settings) {
    Write-Error "The Settings variable is not set."
    exit
}

# Variables
$originPath = $PWD
$tmpBuildPath = $Settings.Deploy.RootDirectory + "\" + $Settings.Tmp.Windows.RelativeSolverBuildPath
$deployDirPath = $Settings.Deploy.RootDirectory + "\" + $Settings.Deploy.RelativWin64SolverPath
$sourceDirPath = $Settings.Source.SolverDirectory
$pathToMinGwMake = $Settings.Compilers.Windows.MinGWMake

Write-Host "Building local solver to: " $deployDirPath

# Create the directories
Write-Host "Creating directories..."
New-Item -Force -Path $tmpBuildPath -ItemType "directory"
New-Item -Force -Path $deployDirPath -ItemType "directory"

# Call Cmake and make target
Write-Host "Calling cmake ..."

$cmakeExpression = "cmake -S $sourceDirPath -B $tmpBuildPath -D`"CMAKE_MAKE_PROGRAM:PATH=$pathToMinGwMake`" -D`"CMAKE_BUILD_TYPE=Release`""
Invoke-Expression $cmakeExpression

Write-Host "Compiling ..."

Set-Location $tmpBuildPath
Invoke-Expression $pathToMinGwMake
Set-Location $originPath

# Copy all exe and dll files to target

Write-Host "Copying from temporary: " $tmpBuildPath

Get-ChildItem $tmpBuildPath\* -Include *.exe, *.dll |
ForEach-Object {
    $copyDst = $deployDirPath + "\" + $_.BaseName + $_.Extension
    $copySrc = $_.FullName
    Copy-Item $copySrc -Destination $copyDst
}

# Remove the tmp build directory
Write-Host "Cleaning up"

Remove-Item -Recurse -Path $tmpBuildPath

Write-Host "Local solver build."