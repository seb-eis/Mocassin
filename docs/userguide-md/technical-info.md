# Introduction - Technical info

## Software components

Mocassin consists of three main components and is mostly written is C#. The solver code for HPC systems is written in C conforming to the 2011 standard. It was a primary goal to write most of the functionality in managed code and keep the performance critical and usually unsafe HPC code to a minimum.
- Model builder API (.NET Standard 2.0)
- Model building GUI (.NET Framework 4.8)
- Monte Carlo solver (C11)

## Platform compatibility

All components of Mocassin are developed for x86_64 systems. Other target platforms or 32-bit operating systems are not supported.

## How to build?

**Note for users:**

*If your are not contributing to Mocassin, it is generally not recommended or required to build the source code yourself. The windows releases of the UI and the solver are expected to run on practically any 64-bit Windows 8/10 machine. It is only required to build the Linux solver binaries for your HPC system.*

### [Compiling the solver for Linux](#compiling-the-solver-for-linux)

Solver compilation for Linux systems has been successfully tested with GNU GCC 8+ and Intel ICC 19+. Usually, ICC compilations show slightly better performance, the difference for GCC 9 and 10 is however marginal in most practical scenarios.

Building the Linux solver requires "CMake" and "make" to be installed. First create the make files by calling CMake from the source code directory and then build using make. It is recommended to use a temporary "build" directory.

```sh
mkdir build
cmake -B ./build -D"CMAKE_BUILD_TYPE=Release"
cd build
make
```

### [Compiling the solver for Win64](#compiling-the-solver-for-win64)

The C solver source code requires modern C features not supported by the MSVC compiler. Thus, compilation for a Win64 systems requires an active MinGW toolchain installation (e.g. with [MSYS2](https://www.msys2.org/)) and [CMake](https://cmake.org/) for Windows. After the toolchain is installed, ensure that the MinGW is added to the PATH variable and compile the solver from the Powershell:

```powershell
mkdir -Force "./build"
cmake -B "./build" -G "MinGW Makefiles" -D"CMAKE_MAKE_PROGRAM:PATH=C:\msys64\mingw64\bin\mingw32-make.exe" -D"CMAKE_BUILD_TYPE=Release"
cd "./build"
Invoke-Expression "<PathToMinGW64>\bin\mingw32-make.exe"
```

### [Compiling the Win64 GUI](#compiling-the-win64-gui)

The C# components come with a Visual Studio solution file. It is recommended to use Visual Studio to build the GUI project. For CLI based building please refer to the Powershell build scripts within the repository.

### [Compiling the Win64 installer](#compiling-the-win64-installer)

Building the Win64 ".msi" file requires the [WiX Toolset 3.11.2+](https://wixtoolset.org/releases/) to be installed. Using the WiX extension for Visual Studio, the installer project can be directly build from the IDE. For CLI based building please refer to the Powershell build scripts within the repository.

### [Packing the API as NuGet](#packing-the-api-as-nuget)

The Mocassin API projects automatically create auto-versioned nuget packages when the project is build with the "Any CPU" and "Release" configuration. The files are place in "ModelBuilder/nuget". For CLI based building please refer to the Powershell build scripts within the repository.

## Scripted deploy

The repository contains a set of Powershell scripts to build an auto-versioned release. This also includes building on multiple remote Linux hosts via ssh connections. Using the system requires several components and dependencies to be present:
- Documentation
  - A current version of [Katex](https://github.com/KaTeX/KaTeX)
  - A current version of [Pandoc](https://pandoc.org/) that is added to PATH
- Win64 Solver
  - A MinGW 64 installation that is added to PATH
  - CMake for Windows
- Linux Solver (remote)
  - OpenSSH for Windows (comes with most modern Windows installations)
  - CMake, make and either ICC or GCC on each target host
- Win64 UI + solver Installer
  - A current Visual Studio Installation
  - The [VSSetup](https://devblogs.microsoft.com/setup/visual-studio-setup-powershell-module-available/) Powershell module for automated MSBuild lookup (optional)
  - WiX Toolset 3.11.2+

The main script executes a set of subscripts that perform the individual build tasks. Scripts can be added, removed, or deactivated in the "build-mocassin.json" config file:
```json
{
    ...
    "Scripts": [
        {
            "Path": "./subscripts/build-documentation.ps1",
            "IsActive": true
        },
        {
            "Path": "./subscripts/build-solver-linuxremote.ps1",
            "IsActive": true
        },
        {
            "Path": "./subscripts/build-solver-local.ps1",
            "IsActive": true
        },
        {
            "Path": "./subscripts/build-gui-installer.ps1",
            "IsActive": true
        },
        {
            "Path": "./subscripts/build-nuget-local.ps1",
            "IsActive": true
        }
    ],
    ...
}
```

### [Win64 installer MSI](#win64-installer-msi)

*Script: build-gui-installer.ps1*

**Note:** *This script requires "build-solver-local.ps1" to have run at least once so that a compiled Win64 solver for packing exists!*

Building and packing the Win64 GUI + solver installer with the build scripts requires the following components to be installed:
- MSBuild
- WIX Toolset 3.11 or later

Within the configuration, file the path to MSBuild has to be configured. The favored option, if Visual Studio is installed, is to leave the entry empty and install the "VSSetup" module for the Powershell. This allows the script to automatically locate the latest instance of Visual Studio and automatically set the path to MSBuild.
```json
{
    ...
    "Compilers": {
        "Windows": {
            "MSBuild": "<PathToMSBuild>"
        }
    },
    ...
}
```

### [Win64 solver](#win64-solver)

*Script: build-solver-local.ps1*

Building the simulator requires a GNU GCC or Intel ICC compiler, the MSVC compiler is not supported. The following programs are required to build with GCC from MSYS2:
- CMake for Windows (CMake has to be added to PATH)
- MSYS2 + MinGW toolchain. ("mingw64/bin" directory has to be added to PATH)

Within the configuration file the path to the mingw make (usually mingw32-make.exe) has to be configured:
```json
{
    ...
    "Compilers": {
        "Windows": {
            "MinGwMake": "<PathToMinGW64>/bin/mingw32-make.exe",
        }
    },
    ...
}
```

### [Linux solver on remote](#linux-solver-on-remote)

*Script: build-solver-linuxremote.ps1*

Building on Linux remotes with the build scripts requires the following components to be installed on the target Linux machine:
- GCC 9+ (Older versions may work as well)
- CMake

On the windows host make sure that the following components are installed:
- Open SSH (https://github.com/PowerShell/Win32-OpenSSH/releases)

The target remotes can be configured in the configuration file. This also allows to define commands that will be executed before invoking make and cmake on the target host, e.g. load a module. The "Name" property will be used to automatically name the deploy directory and has to be unique.
```json
{
    ...
    "Remote": {
        "LinuxRemotes": [
            {
                "Name": "centos",
                "Host": "username@my.example-host.com",
                "PreBuildCommands": [
                    "module unload gcc",
                    "module load gcc/10"
                ]
            }
        ]
    },
    ...
}
```
### [API NuGet packages](#api-nuget-packages)

*Script: build-nuget-local.ps1*

Building and packing the .NET Standard 2.0 API libraries with the build scripts requires the following components to be installed:
- MSBuild

Within the configuration file the path to MSBuild has to be configured or the VSSetup method described above has to be used.
```json
{
    ...
    "Compilers": {
        "Windows": {
            "MSBuild": "<PathToMSBuild>"
        }
    },
    ...
}
```

### [HTML documentation pages](#html-documentation-pages)

*Script: build-documentation.ps1*

Building the documentation converts the user guide markdown pages into a standalone set of static HTML pages. The following programs are required:
- Katex API (for Latex math support)
- Pandoc (for markdown to html conversion)

Before building the path to the directory that contains the Katex API has to be defined:

```json
{
  ...
  "Source": {
    ...
    "KatexDirectory": "<PathToKatexDir>",
    ...
  }
  ...
}
```