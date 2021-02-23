# Preparing for API-based result evaluation

## Description

Many common properties, such as conductivity, mobility, and diffusion, can be extracted from the text dumps created by the simulator during processing. However, the raw data these results are calculated from are dumped as binary simulation state files (.mcs) which cannot be interpreted out of context due to their cryptic nature. Thus, Mocassin provides an API to help with advanced data evaluation that can be added to a .NET project using the Mocassin nuget packages. The MOCASSIN API is entirely written in C# and can well be accesses from both C# and F#.

**Note:** Only a part of the example source code is provided for both C# and F# since they are often similar. As Mocassin is written in C#, the F# code is omitted for later examples.

**Important:**

- It is generally recommended to use an F# or C# .NET Core 3.1/.NET 5 project or an F#/C# script file for data evaluation as these can be used platform independently and fully support all required nuget packages. The required SDKs can be downloaded from [here](https://dotnet.microsoft.com/download). C# script files ".csx" for use with the [dotnet script tool](https://github.com/filipw/dotnet-script) require the provided hotfix package `Mocassin.Csx` and a manual `Mocassin.Symmetry.db` location definition to work.
- You can get F# and C# Intellisense and debugging in Visual Studio Code for both projects and script files using the [Microsoft C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and [ionide-fsharp](https://marketplace.visualstudio.com/items?itemName=Ionide.Ionide-fsharp) extensions.

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

Adding the Mocassin API to F# 5.0 script files ".fsx" to use with the dotnet F# script interface requires to add a package reference to `Mocassin` using the `#r nuget: name` syntax. Additionally, the interpreter can be defined to `dotnet fsi` using "shebang" for systems that support it:

```fsharp
// Reference to Mocassin nuget package and set the linux interpreter to dotnet fsi
#!dotnet fsi
#r "nuget: Mocassin"

// Example: Lets load all space groups and print their Mauguin notation to the console
open System
open Mocassin.Model.DataManagement
open Mocassin.Symmetry.SpaceGroups

let project = ModelProjectFactory.CreateDefault ()
let groups = project.SpaceGroupService.GetFullGroupList ()

groups |> Seq.iter (fun x -> printfn "%A" x.MauguinNotation)

```

### [Creating a C# script file](#creating-a-c-script-file)

Using the C# script interface requires to first install the system as a global dotnet tool for all operating systems:

```shell
### Install the script tool
dotnet tool install -g dotnet-script

### Init a folder to be a csx script folder to get intellisense and debugging in vscode
dotnet script init

### Run the script interface with a script file
dotnet script [script.csx]
```

Adding the Mocassin API to the C# script files ".csx" to use with the dotnet C# script interface requires to add a package reference to `Mocassin` and `Mocassin.Csx` using the `#r nuget: name, version` syntax and to load the script package using the `#load "nuget: Mocassin.Csx, 1.0.0"`. Additionally, the interpreter can be defined to `dotnet script` using "shebang" for systems that support it:

**Important:** 

- The symmetry database has to be redirected manually as dotnet-script does not correctly handle the unpacking of additional content. The database can be downloaded [here](https://github.com/seb-eis/Mocassin/blob/master/src/ModelBuilder/ICon.Framework.Symmetry/Data/Mocassin.Symmetry.db).
- The dotnet-script execution cache can only work if the exact versions of the nuget packages are specified. If not specified no execution cache is used which significantly increases the startup time on sequential runs.


```csharp
// Reference to Mocassin, Mocassin.Csx and load Mocassin.Csx, replace X.X.X by correct package version
#!dotnet script
#r "nuget: Mocassin, X.X.X"
#r "nuget: Mocassin.Csx, 1.0.0"
#load "nuget: Mocassin.Csx, 1.0.0"

// Globally set the default path to the symmetry db
Mocassin.Symmetry.SpaceGroups.SpaceGroupContextSource.DefaultDbPath = "<Path to Mocassin.Symmetry.db>"

// Example: Lets load all space groups and print their Mauguin notation to the console
using System;
using Mocassin.Model.DataManagement;
using Mocassin.Symmetry.SpaceGroups;

var project = ModelProjectFactory.CreateDefault();
var groups = project.SpaceGroupService.GetFullGroupList();

foreach (var item in groups)
{
    Console.WriteLine("{0}", item.MauguinNotation);
}

```

### [Setting up a full C#/F# project](#setting-up-a-full-project)

If a full project is required, F# or C# projects (usually console application) can be created as follows:

```shell
mkdir [ProjectDirectory]
cd [ProjectDirectory]

## Initialize a new C# console application
dotnet new console

## Initialize a new F# console application
dotnet new console --language F#
```

The last step is to add the Mocassin nuget packages to your project as shown below. Installing the `Mocassin` package will cause all other packages and affiliated dependencies to be installed, including the symmetry database. The 'ProjectFile' parameter is optional and defaults to the project file found in the current working directory of not specified.

```shell
dotnet add [[ProjectFile]] package Mocassin
```

**Note**: In older versions of the packages the `Mocassin` package does not exist. Use `Mocassin.Tools.Evaluation` instead if `Mocassin` does not exist.