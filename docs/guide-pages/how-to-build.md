# How to build?

## Manual building

### Linux simulator binaries
Navigate into the source directory and execute the following commands to build a release version using the currently set default C compiler:
```bash
mkdir build
cmake -B ./build -S . -D"CMAKE_BUILD_TYPE=Release"
cd build
make
cd ..
```

Note: In older versions of CMake the flags "-B" and "-S" will not be recognized. In this case just build inside the source code directory:
```bash
cmake . -D"CMAKE_BUILD_TYPE=Release"
make
```

## Scripted building

Mocassin comes with a modular Powershell build script that can be extended if required. The following features are currently available out of the box:
- Local Win64 GUI (Mocassin) build & msi installer packing using MSBuild
- Local compilation of the simulator binaries (Mocsim) for Win64 computers
- Remote compilation on multiple Linux hosts via an SSH connection

The main scripts executes a set of subscripts that perform the individual build tasks. Scripts can be added, removed, or deactivated in the "build-mocassin.json" config file:
```json
{
    ...
    "Scripts": [
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

### Mocassin Win64 GUI Installer (build-gui-installer.ps1)

Building and packing the GUI installer with the build scripts requires the following components to be installed:
- MSBuild
- WIX Toolset 3.11 or later

Within the configuration file the path to MSBuild has to be configured:
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

### Win64 simulator binaries (build-solver-local.ps1)

Building the simulator requires a GNU GCC or Intel ICC compiler. The MSVC compiler cannot build the simulator as several modern C features are not supported. The following programs are required to build with the GCC from MSYS2:
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

### Linux simulator binaries on remotes (build-solver-linuxremote.ps1)

Building on Linux remotes with the build scripts requires the following components to be installed on the target Linux machine:
- GCC 9+
- CMake

On the windows host make sure that the following components are installed:
- Open SSH (https://github.com/PowerShell/Win32-OpenSSH/releases)

The target remotes can be configured in the configuration file. This also allows to define commands that will be executed before invoking make and cmake on the target host, e.g. load a module:
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

### .NET Standard 2.0 nuget packages (build-nuget-local.ps1)

Building and packing the .NET Standard 2.0 API libraries with the build scripts requires the following components to be installed:
- MSBuild

Within the configuration file the path to MSBuild has to be configured:
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
The script builds auto versioned nuget packages including all DLLs of Mocassin. Note that the symmetry database is not included in the packages.