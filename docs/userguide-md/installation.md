# Installing Mocassin components

## Windows installation

Mocassin comes with an ".msi" installer for Windows computers. The installer can install both the model builder GUI and the solver for Win64 to a custom location. It additionally sets the file associations for Mocassin project files ".mocprj" and creates desktop and start menu shortcuts to the GUI.

After installation, the solver can be found in "\<InstallDir\>/Solver/bin" and the UI in "\<InstallDir\>/Model Builder/bin".

## Solver installation (Linux)

It is usually recommended to [compile the solver](./technical-info.md) on your Linux target machine to ensure maximized performance. If you are using precompiled binaries make sure that at least the following ".so" files and components are located in your deploy directory:

- libframework
- libjobloader
- libprogressprint
- libsimulator
- libsqlite3
- Mocassin.Simulator (Solver exe)

Optional components are:
- libmmcfe.mocext (Simulated annealing MMC extension)
- libutility (Utility library)
- Mocassin.Utility (Utility exe)

You can test if the simulator works by calling "Mocassin.Simulator" without any arguments, this should create console output that looks similar to:
```text
[FAILURE]: Missing input. Make sure all required command line arguments are defined:
[REQUIRED]: -dbPath     <value>
[REQUIRED]: -jobId      <value>
[REQUIRED]: -ioPath     <value>
ERROR:  0x0000000e
Func    ResolveMocassinCommandLineArguments
Line:   213
Type:   Command Argument missing or invalid
                (Expected reason: Database path or database load instruction is missing or not defined)
Info:   Failed to resolve essential command line arguments.
```

## .NET Core project integration

In the majority of cases, a .NET Core project integration is done to write advanced evaluation programs based on the binary state dumps of the simulator. To add the NuGet packages to an existing .NET Core project, first configurate your project to accept a local directory as a NuGet package source. This can be done by adding the directory to the packages sources of the "nuget.config" file, e.g.:

```xml
<configuration>
  <packageSources>
    <clear />
    ...
    <add key ="mocassin" value="<RelativePathToMocassinNugetDirectory>"/>
  </packageSources>
</configuration>
```

After that, the entire Mocassin API and affiliated dependencies can be added to the project by adding the evaluation package to the project using the dotnet CLI:

```powershell
dotnet add Myproject.csproj package Mocassin.Tools.Evaluation
```

**Important:** The vast majority of API features require the space group database "Mocassin.Symmetry.db" which is available separately from the repository. To verify that everything is setup correctly, make sure that the following simple program runs to completion without an error after \<PathToSymmetryDb\> is replaced by the correct path string.

```csharp
using System;
using Mocassin.Model.ModelProject;
using Mocassin.Model.DataManagement;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ProjectSettings.CreateDefault();
            settings.SymmetrySettings.SpaceGroupDbPath = "<PathToSymmetryDb>";
            var project = ModelProjectFactory.Create(settings);
            var allGroups = project.SpaceGroupService.GetFullGroupList();
        }
    }
}
```