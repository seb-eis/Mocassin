# Preparing for API-based result evaluation

## Description

Many common properties, such as conductivity, mobility, and diffusion, can be extracted from the text dumps created by the simulator during processing. However, the raw data these results are calculated from are dumped as binary simulation state files (.mcs) which cannot be interpreted out of context due to their cryptic nature. Thus, Mocassin provides an API to help with advanced data evaluation that can be added to a .NET project using the Mocassin nuget packages. The MOCASSIN API is entirely written in C# and can well be accesses from both C# and F#.

**It is generally recommended to use an F# or C# .NET Core 3.1/.NET 5 project or an F# 5.0 script file ".fsx" for data evaluation as these can be used platform independently and fully support all required nuget packages. The required SDKs can be downloaded from [here](https://dotnet.microsoft.com/download). C# script files ".csx" for use with the [dotnet script tool](https://github.com/filipw/dotnet-script) are not working properly as the "#r" directive seems to not correctly handle the "e_sqlite" native SQLite library and cannot find it.**

## Usage

### [Collecting simulation results into the msl file](#collecting-simulation-results-into-the-msl-file)

Assuming that the startup [helper scripts](https://github.com/seb-eis/Mocassin/tree/master/src/McSolver/Scripts) where used to run multiple jobs from the same simulation database, then the working directory will contain a directory for each run job labeled 'Job00001, Job00002, ...'. To collect the results (stdout, run.mcs, prerun.mcs) back into the msl file, the [collector script](https://github.com/seb-eis/Mocassin/blob/master/src/McSolver/Scripts/mocassin_collector.py) can be used. Note that the provided path information needs to be absolute for the script to work in all situations.

```shell
python3 mocassin_collector.py [project.msl] [projectdir]
```

### [Setting up the nuget package source](#setting-up-the-nuget-package-source)

Before setting up a project or script file, it is required to create a local nuget package source from which the package manager can pull the packages that are shipped with releases versions. This step has to be done only once. The dotnet CLI can be used to check if one already exists. Calling the following should usually yield at least an entry 'nuget.org', which is the official online package source.

```shell
dotnet nuget list source
```

Adding or removing a new local source can be done using the following commands:

```shell
dotnet nuget add source [Directory] --name [SourceName]
dotnet nuget remove source [SourceName]
```

After the source is created the Mocassin nuget packages can just be copy-pasted into the directory and the nuget package manager included in the dotnet CLI can used it.

Optionally, a `nuget.config` file that overwrites the global config can be added to the root directory of a project and the local package source can be configured so that it is available to the project only. **This does not work for F# script files**.

```shell
dotnet new nugetconfig
```

Then add a package source the `nuget.config` and configurate a local source. The simplest option is to create a folder in the project root directory (here "nugets") and place the packages there. The `nuget.config` should look similar to this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!--To inherit the global NuGet package sources remove the <clear/> line below -->
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <!--Config this to target your folder -->
    <add key="local" value="./nugets/"/>
  </packageSources>
</configuration>

```

### [Creating an F# script file](#creating-an-f-script-file)

For many purposes of working with the Mocassin API it s sufficient and the most convenient to use F# 5.0 script files ".fsx" to use with the dotnet F# script interface. These have the advantga of beeing a single file that is easily portable to every system where the Mocassin nuget packages are properly configured. An F# ".fsx" script example that accesses the Mocassin API looks like this:

```fsharp
// Reference Mocassin and System.Reactive (all other packages are implicitly referenced correctly)
#r "nuget: Mocassin"
#r "nuget: System.Reactive"

// Lets load all space groups and print their Mauguin notation to the console
open System
open Mocassin.Model.DataManagement
open Mocassin.Symmetry.SpaceGroups

let project = ModelProjectFactory.CreateDefault ()
let groups = project.SpaceGroupService.GetFullGroupList ()

groups |> Seq.iter (fun x -> printfn "%A" x.MauguinNotation)

```

### [Setting up a full project](#setting-up-a-full-project)

If a full project is required, F# or C# projects (usually console application) can be created as follows:

```shell
mkdir [ProjectDirectory]
cd [ProjectDirectory]

## Initialize a new C# console application
dotnet new console

## Initialize a new F# console application
dotnet new console --language F#
```

The last step is to add the Mocassin nuget packages to your project as shown below. Installing the 'Mocassin' package will cause all Mocassin packages and affiliated dependencies to be installed, including the symmetry database. The 'ProjectFile' parameter is optional and defaults to the project file found in the current working directory of not specified.

```shell
dotnet add [[ProjectFile]] package Mocassin
```

**Note: In older versions of the packages the 'Mocassin' package does not exist. Use 'Mocassin.Tools.Evaluation' instead if 'Mocassin' does not exist.**