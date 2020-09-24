param (
    [Parameter(Mandatory=$true)]
    [ValidateNotNull()]
    [object]
    $Settings
)

# Variables
$localSourceDir = $Settings.Source.SolverDirectory
$remoteSourcePath = $Settings.Tmp.Linux.SolverBuildDirectory
$localDeployBasePath = $Settings.Deploy.RootDirectory, $Settings.Deploy.RelativeLinuxDeployPath -join "\"
$copyExcludes = $Settings.Remote.CopyFolderExcludes
$copyIncludes = $Settings.Remote.CopyFileIncludes
$copyBackPattern = $Settings.Remote.CompiledFilesPattern
$remotes = $Settings.Remote.LinuxRemotes
$compilerVar = "CC=" + "`"" + $Settings.Compilers.Linux.CCompiler + "`""

Write-Host "Starting remote builds ..."

foreach ($remote in $remotes) {
    
    # Get remote info and create local deploy folder
    $remoteName = $remote.Name
    $remoteHost = $remote.Host
    $localDeployPath = $localDeployBasePath, $remoteName -join "-"

    if (Test-Path $localDeployPath) {
        Remove-Item -Force -Recurse $localDeployPath
    }
    New-Item -Force -Path $localDeployPath -ItemType "directory"
    
    Write-Host "Building on $remoteHost ($remoteName) to: $localDeployPath"
    Write-Host "Setting up directories and uploading source files ..."

    # Setup tmp build directory
    ssh $remoteHost "rm -r $remoteSourcePath"
    ssh $remoteHost "mkdir $remoteSourcePath"
    
    # Copy directories and single files of source to remote
    Get-ChildItem $localSourceDir -Exclude $copyExcludes | ForEach-Object {
        $copySrc = $_
        $copyDst = ($remoteHost, $remoteSourcePath -join ":"), $_.BaseName -join "/"
        scp -r $copySrc $copyDst
    }

    Get-ChildItem $localSourceDir\* -Include $copyIncludes | ForEach-Object {
        $copySrc = $_.FullName
        $copyDst = $remoteHost, $remoteSourcePath -join ":"
        scp $copySrc $copyDst
    }

    Write-Host "Configurating & compiling ..."

    # Build prerun commands
    $buildCommand = $remote.PreBuildCommands -join " && "

    # Run cmake and make
    $cmakeCommand = "$compilerVar cmake -S . -D`"CMAKE_BUILD_TYPE=Release`""
    $makeCommand = "make"
    $buildCommand = "cd $remoteSourcePath", $buildCommand, $cmakeCommand, $makeCommand -join " && "
    ssh $remoteHost $buildCommand

    # Copy the compiled files to the local deploy directory
    Write-Host "Downloading compiled files & cleaning up ..."

    $compiledFileNames = (ssh $remoteHost  "cd $remoteSourcePath && ls $copyBackPattern")
    
    foreach ($fileName in $compiledFileNames) {
        $copySrc = ($remoteHost, $remoteSourcePath -join ":"), $fileName -join "/"
        $copyDst = $localDeployPath, $fileName -join "\"
        scp $copySrc $copyDst
    }

    # Remove tmp build directory
    ssh $remoteHost "rm -r $remoteSourcePath"

    Write-Host "Done building on $remoteHost as $remoteName!"
}