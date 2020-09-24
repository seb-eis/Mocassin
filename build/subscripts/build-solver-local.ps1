param (
    [Parameter(Mandatory=$true)]
    [ValidateNotNull()]
    [object]
    $Settings
)

# Variables
$originPath = $PWD
$tmpBuildPath = $Settings.Deploy.RootDirectory + "\" + $Settings.Tmp.Windows.RelativeSolverBuildPath
$deployDirPath = $Settings.Deploy.RootDirectory + "\" + $Settings.Deploy.RelativWin64SolverPath
$sourceDirPath = $Settings.Source.SolverDirectory
$pathToMinGwMake = $Settings.Compilers.Windows.MinGWMake

Write-Host "Building solver locally ... "

# Create the directories

New-Item -Force -Path $tmpBuildPath -ItemType "directory"
New-Item -Force -Path $deployDirPath -ItemType "directory"

# Call Cmake and make target
Write-Host "Configurating ..."

$cmakeExpression = "cmake -S $sourceDirPath -B $tmpBuildPath -D`"CMAKE_MAKE_PROGRAM:PATH=$pathToMinGwMake`" -D`"CMAKE_BUILD_TYPE=Release`""
Invoke-Expression $cmakeExpression

Write-Host "Compiling ..."

Set-Location $tmpBuildPath
Invoke-Expression $pathToMinGwMake
Set-Location $originPath

# Copy all exe and dll files to target

Write-Host "Copying files to deploy & cleaning up"

Get-ChildItem $tmpBuildPath\* -Include *.exe, *.dll |
ForEach-Object {
    $copyDst = $deployDirPath + "\" + $_.BaseName + $_.Extension
    $copySrc = $_.FullName
    Copy-Item $copySrc -Destination $copyDst
}

Remove-Item -Recurse -Path $tmpBuildPath