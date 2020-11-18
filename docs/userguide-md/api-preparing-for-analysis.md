# Preparing for API-based result evaluation

## Description

Many common properties, such as conductivity, mobility, and diffusion, can be extracted from the text dumps created by the simulator during processing. However, the raw data these results are calculated from are dumped as binary simulation state files (.mcs) which cannot be interpreted out of context due to their cryptic nature. Thus, Mocassin provides an API to help with advanced data evaluation that can be added to a .NET project using the Mocassin nuget packages.

## Usage

### [Collecting simulation results into the msl file](#collecting-simulation-results-into-the-msl-file)

Assuming that the startup [helper scripts](https://github.com/seb-eis/Mocassin/tree/master/src/McSolver/Scripts) where used to run multiple jobs from the same simulation database, then the working directory will contain a directory for each run job labeled 'Job00001, Job00002, ...'. To collect the results (stdout, run.mcs, prerun.mcs) back into the msl file, the [collector script](https://github.com/seb-eis/Mocassin/blob/master/src/McSolver/Scripts/mocassin_collector.py) can be used. Note that the provided path information needs to be absolute for the script to work in all situations.

```shell
python3 mocassin_collector.py [project.msl] [projectdir]
```

### [Setting up a project](#setting-up-a-project)

It is generally recommended to use an F# or C# .NET Core 3.1 or .NET 5 project for data evaluation as these can be used platform independently. The required SDKs can be downloaded from [here](https://dotnet.microsoft.com/download). 

To set up a project, it is required to create a local nuget package source from which the package manager can pull the packages that are shipped with releases versions. This step has to be done only once. The dotnet CLI can be used to check if one already exists. Calling the following should usually yield at least an entry 'nuget.org', which is the official online package source.

```shell
dotnet nuget list source
```

Adding or removing a new local source can be done using the following commands:

```shell
dotnet nuget add source [Directory] --name [SourceName]
dotnet nuget remove source [SourceName]
```

After the source is created the Mocassin nuget packages can just be copy-pasted into the directory and the nuget package manager included in the dotnet CLI can used it.

The second step is to create a new F# or C# project (usually console application):

```shell
mkdir [ProjectDirectory]
cd [ProjectDirectory]

## Initialize a new C# console application
dotnet new console

## Initialize a new F# console application
dotnet new console --language F#
```

The last step is to add the Mocassin nuget packages to your project as shown below. Installing 'Mocassin.Tools.Evaluation' will cause all other Mocassin packages and affiliated dependencies to be installed, including the symmetry database. The 'ProjectFile' parameter is optional and defaults to the project file found in the current working directory of not specified.

```shell
dotnet add [[ProjectFile]] package Mocassin.Tools.Evaluation
```