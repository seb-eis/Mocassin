# How to build?

**Note for users:**

*If your are not contributing to Mocassin, it is generally not recommended or required to build the source code yourself. The windows releases of the UI and the solver are expected to run on practically any 64-bit Windows 8/10 machine. It is only required to build the Linux solver binaries for your HPC system.*

## Manual building

### Linux simulator binaries

Navigate into the solver source directory and execute the following commands to build a release version using the currently set default C compiler. The source code can be deleted after compilation.

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
- Local Win64 GUI (Mocassin) build & msi installer packing for UI + solver using MSBuild
- Local compilation of the simulator binaries (Mocsim) for Win64 computers
- Remote compilation on multiple Linux hosts via an SSH connection
- Packing of the .NET Standard 2.0 libraries as nuget packages 

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

### Win64 simulator binaries (build-solver-local.ps1)

Building the simulator requires a GNU GCC or Intel ICC compiler, the MSVC is not supported. The following programs are required to build with GCC from MSYS2:
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
### .NET Standard 2.0 nuget packages (build-nuget-local.ps1)

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