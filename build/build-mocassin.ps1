function Get-AutoVersion {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [int]
        $Major,
        [Parameter(Mandatory=$true)]
        [int]
        $Minor
    )

    $now = [System.DateTime]::Now
    $year2000 = New-Object System.DateTime -ArgumentList 2000, 1, 1
    $midnightToday = New-Object System.DateTime -ArgumentList $now.Year, $now.Month, $now.Day
    $build = ($now - $year2000).Days.ToString("f0")
    $revision = (($now - $midnightToday).TotalSeconds / 2.0).ToString("f0")
    $version = $Major, $Minor, $build, $revision -join "."
    return $version
}

function Write-Log {
    [CmdletBinding()]
    param (
        [Parameter(Position=0, ValueFromPipeline)]
        [object]
        $Message,
        [Parameter(Mandatory=$true)]
        [string]
        $FilePath
    )
    Process {
        Write-Host $Message
        $Message | Out-File -FilePath $FilePath -Append
    }
}

$Settings = Get-Content -Path .\build-mocassin.json | ConvertFrom-Json

# Auto version by date and set the new deploy folder
$version = Get-AutoVersion -Major $Settings.Meta.Version.Major -Minor $Settings.Meta.Version.Minor
$deployRootPath = $Settings.Deploy.RootDirectory, ("Mocassin-" + $version) -join "/"
$deployLogFile = [System.IO.Path]::GetFullPath($deployRootPath), $Settings.Deploy.LogFileName -join "/"
$Settings.Deploy.RootDirectory = $deployRootPath
Add-Member -InputObject $Settings.Meta.Version -NotePropertyName Full -NotePropertyValue $version

# Delte and recreate the deploy folder
Write-Host "Automated build version is: " $version
Write-Host "Setting up build directory: " $deployRootPath
Write-Host "Writing log to: " $deployLogFile

New-Item -Force -Path $deployRootPath -ItemType directory | Out-Null

# Call local solver build
foreach ($script in $Settings.Scripts) {
    if ($script.IsActive) {
        Write-Host "Running script: " $script.Path
        & $script.Path -Settings $Settings 3>&1 2>&1 | Write-Log -FilePath $deployLogFile
        Write-Host "Finished script: " $script.Path
    }
}