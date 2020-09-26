function Find-MSBuild {
    [OutputType([string])]
    param ()
    $vsSetupInstance = Get-VSSetupInstance
    $msBuildPath = $vsSetupInstance.InstallationPath, "MSBuild\Current\Bin\MSBuild.exe" -join "\"
    if (-not (Test-Path $msBuildPath)) {
        throw "Could not locate MSBuild."
    }
    return $msBuildPath
}

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

function Expand-String {
    [CmdletBinding()]
    param (
        [Parameter(Position=0, ValueFromPipeline)]
        [object]
        [ValidateNotNull()]
        $Value
    )

    $ExecutionContext.InvokeCommand.ExpandString($Value)
}

function Get-SettingsObject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string]
        [ValidateNotNullOrEmpty()]
        $PathToSettingsJson
    )

    if (-not (test-Path $PathToSettingsJson)) {
        throw "$PathToSettingsJson is not a valid file path."
    }

    $settings = Get-Content -Path $PathToSettingsJson | ConvertFrom-Json

    # Auto version
    $version = Get-AutoVersion -Major $settings.Meta.Version.Major -Minor $settings.Meta.Version.Minor
    $deployRootPath = $settings.Deploy.RootDirectory, ("Mocassin-" + $version) -join "/" | Expand-String
    $deployLogFile = $deployRootPath, $settings.Deploy.LogFileName -join "/"
    $settings.Deploy.RootDirectory = $deployRootPath
    Add-Member -InputObject $settings.Deploy -NotePropertyName LogFile -NotePropertyValue $deployLogFile
    Add-Member -InputObject $settings.Meta.Version -NotePropertyName Full -NotePropertyValue $version

    # Auto set MSBuild if not set
    if (-not (Test-Path $settings.Compilers.Windows.MSBuild)) {
        Write-Host "MSBuild is no defined, searching ..."
        $msBuild = Find-MSBuild
        $settings.Compilers.Windows.MSBuild = $msBuild
        Write-Host "MSBuild located at: $msBuild"
    }

    return $settings
}