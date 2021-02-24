# Evaluation classes provided by the Mocassin API

## Description

Mocassin provides several ready-to-use implementations of `JobEvaluation<T>` for typical evaluation scenarios. These currently include the following classes that extract data from the `JobContext` instnaces:

- [ParticleCountEvaluation](#particlecountevaluation): Creates a list of the ensemble sizes for all `IParticle` objects
- [TransitionMobilityEvaluation](#transitionmobilityevaluation): Calculates mobility information for each `IGlobalTrackerModel` 
- [EnsembleMobilityEvaluation](#ensemblemobilityevaluation): Calculates mobility information for each `IParticle` ensemble
- [StaticTrackingEvaluation](#statictrackingevaluation): Calculates all static tracker results and provides the local velocity vectors
- [EnsembleDisplacementEvaluation](#ensembledisplacementevaluation): Calculates displacement data for all mobile particle ensembles
- [SquaredDisplacementEvaluation](#squareddisplacementevaluation): Calculates mean square displacement (MSD) data for all mobile particle ensembles
- [EnsembleDiffusionEvaluation](#ensemblediffusionevaluation): Calculates the diffusion data for each `IParticle` ensembles
- [EnsembleCorrelationEvaluation](#ensemblecorrelationevaluation): Calculates the movement correlation data between all `IParticle` ensembles
- [MobileTrackingEvaluation](#mobiletrackingevaluation): Prepares mobile tracking data for all mobile particles
- [GlobalTrackingEvaluation](#globaltrackingevaluation): Prepares global tracking data for all `IGlobalTrackerModel` objects
- [CellSiteCoordinationEvaluation](#cellsitecoordinationevaluation): Performs local surrounding analysis averaged for each symmetry equivalent position and occupation
- [LocalSiteCoordinationEvaluation](#localsitecoordinationevaluation): Performs local surrounding analysis for each position of the lattice individually
- [LatticeMetaEvaluation](#latticemetaevaluation): Collects meta information about the lattice
- [EnsembleMetaEvaluation](#ensemblemetaevaluation): Combines `LatticeMetaEvaluation` and `ParticleCountEvaluation`
- [EnsembleJumpStatisticsEvaluation](#ensemblejumpstatisticsevaluation): Prepares the jump statistics data for further processing

**Important:** In general all output is provided in standard SI units without prefixes (expect for the mass unit `kg`) unless stated otherwise by the XML comments of the API class. For example, the conductivity is given as `S/m` and not as `S/cm` as commonly done in material sciences.

The following examples are all based on an open `MslEvaluationContext` where an `IEvaluableJob` set was created. The following C#/F# script templates can be used to work with the examples after X.X.X is replaced by the correct nuget package version and the symmetry database is defined in the C# script file.

The basic `script.csx` for C#:

```csharp
#!dotnet script
#r "nuget: Mocassin, X.X.X"
#r "nuget: Mocassin.Csx, 1.0.0"
#load "nuget: Mocassin.Csx, 1.0.0"

using Mocassin.Symmetry.SpaceGroups;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries;

SpaceGroupContextSource.DefaultDbPath = @"<Symmetry-DB-Parent-Dir>/Mocassin.Symmetry.db";

// Note: Wrapped code into a function to allow ref structs on local variables
void Main() 
{
     // Get the script cmd arguments
    var args = Args;

    // Load the job set
    var context = MslEvaluationContext.Create("./myproject.msl");
    var jobSet = context.LoadJobsAsEvaluable(x => x);

    // do stuff ...
}

Main();
```

The basic `script.fsx` for F#:

```fsharp
#!dotnet fsi
#r "nuget: Mocassin, X.X.X"

open Mocassin.Tools.Evaluation.Context
open Mocassin.Tools.Evaluation.Queries

// Get the script cmd arguments
let argv = fsi.CommandLineArgs |> Array.tail

// Load the job set
let context = MslEvaluationContext.Create "./myproject.msl"
let jobSet = context.LoadJobsAsEvaluable (fun x -> x)

// do stuff ...
```

## Usage

### [ParticleCountEvaluation](#particlecountevaluation)

This evaluation is useful when the ensemble size of an `IParticle` is required during evaluation. The data is provided as a `ReadOnlyList<int>` where the `index` of the elements is the `IParticle.Index` and the value at each index is the particle count.

```csharp
var eval = new ParticleCountEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var (count, particleId) in data.Select((count, index) => (count, index)))
    {
        Console.WriteLine($"Job {jobContext.DataId}: The particle at index {particleId} has a count of {count}");
    }
}
```

### [TransitionMobilityEvaluation](#transitionmobilityevaluation)

This evaluation is useful when the contributions to mobility and conductivity in field direction must be analyzed for each `ITransition` and `IParticle` combination. The data is provided as a `ReadOnlyList<TransitionMobility>` and contains multiple information for each `IGlobalTrackerModel`.

```csharp
var eval = new TransitionMobilityEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var mobility = item.EnsembleMobility;
        var particle = item.TrackerModel.TrackedParticle;
        var transition = item.TrackerModel.KineticTransitionModel;

        // E.g. get the conductivity and mobility in field direction
        var conductivity = mobility.Conductivity;
        var ionicMobility = mobility.IonicMobility;

        Console.WriteLine($"Job {jobContext.DataId}: {particle} migrating by {transition} contibute: sigma = {conductivity} S/m and u = ({ionicMobility} m^2/(s V))");
    }
}
```

### [EnsembleMobilityEvaluation](#ensemblemobilityevaluation)

This evaluation is the classical mobility evaluation providing mobility and conductivity in field direction for each `IParticle` ensemble that is mobile in the simulation. The data is provided as a `ReadOnlyList<EnsembleMobility>` for each mobile `IParticle`.

```csharp
var eval = new EnsembleMobilityEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var mobility = item.IonicMobility;
        var sigmaD = item.EffectiveDiffusionCoefficient; // Effective D_sigma from Nernst-Einstein
        var conductivity = item.Conductivity;
        var particle = item.Particle;

        Console.WriteLine($"Job {jobContext.DataId}: {particle} contibutes: sigma = {conductivity} S/m and u = ({mobility} m^2/(s V))");
    }
}
```

### [StaticTrackingEvaluation](#statictrackingevaluation)

This evaluation is useful to analyze the average magnitude and direction of the `velocity` of `IParticle` objects at specific positions in the lattice. The data is provided as a `ReadOnlyList<StaticTrackerResult>` for each existing static tracker.

```csharp
var eval = new StaticTrackingEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var tracker in data)
    {
        var positionId = tracker.PositionId;
        var particle = tracker.Particle;
        var velocity = tracker.VelocityVector;
        Console.WriteLine($"Job {jobContext.DataId}, average velocity [m/s] of {particle} at position {positionId} is {velocity}");
    }
}
```

### [EnsembleDisplacementEvaluation](#ensembledisplacementevaluation)

This evaluation provides the classical ensemble displacement evaluation for each `IParticle` of the supercell. The data is provided both for all directions $\langle |\vec{r}| \rangle$ and as an average vector $(\langle x \rangle,\langle y \rangle,\langle z \rangle)$ for the cartesian directions $(x,y,z)$. By setting the `YieldMeanResult` flag to `false` the evaluation can also yield the total displacement. The data is provided as a `ReadOnlyList<EnsembleDisplacement>`.

```csharp
var eval = new EnsembleDisplacementEvaluation(jobSet) { YieldMeanResult = true };
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var particle = item.Particle;
        var vectorR = item.VectorR;
        var radialR = item.DisplacementR;
        Console.WriteLine($"Job {jobContext.DataId}: The radial displacement [m] of {particle} is R = {radialR} and the components are {vectorR}");
    }
}
```

### [SquaredDisplacementEvaluation](#squareddisplacementevaluation)

This evaluation provides the classical ensemble mean square displacement (MSD) evaluation for each `IParticle` of the supercell. The data is provided both for all directions $\langle |\vec{r}|^2 \rangle$ and as an average vector $(\langle x^2 \rangle, \langle y^2 \rangle, \langle z^2 \rangle)$ for the cartesian directions $(x,y,z)$. By setting the `YieldMeanResult` flag to `false` the evaluation can also yield the total displacement. The data is provided as a `ReadOnlyList<EnsembleDisplacement>`.

```csharp
var eval = new SquaredDisplacementEvaluation(jobSet) { YieldMeanResult = true };
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var particle = item.Particle;
        var componentMSD = item.VectorR;
        var msd = item.DisplacementR;
        Console.WriteLine($"Job {jobContext.DataId}: The MSD [m^2] of {particle} is R = {msd} with the components {componentMSD}");
    }
}
```

### [EnsembleDiffusionEvaluation](#ensemblediffusionevaluation)

This evaluation performs an ensemble diffusion analysis for each `IParticle` and provides the MSD and tracer diffusion coefficient $D^*$ for $(x,y,z)$ directions and the room averaged radial value. The data is provided as a `ReadOnlyList<EnsembleDiffusion>`.

```csharp
var eval = new EnsembleDiffusionEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var particle = item.Particle;
        var (msdX, msdY, msdZ, msdR) = item.Msd;
        var (dX, dY, dZ, dR) = item.DiffusionCoefficient;
        Console.WriteLine($"Job {jobContext.DataId}: {particle} has D* = (x={dX},y={dY},z={dZ},r={dR}) [m^2/s]");
    }
}
```

### [EnsembleCorrelationEvaluation](#ensemblecorrelationevaluation)

This evaluation is useful if the phenomenological coefficients $L_{ii},L_{ij}$ (Onsager coefficient) or the movement correlations $\langle R_i \cdot R_i \rangle, \langle R_i \cdot R_j \rangle$ of `IParticle` ensembles are required. The correlations are $\sum_i\langle \vec{r}_i \cdot \vec{r}_i \rangle$ for the same ensemble and cross-correlation $\sum_{i\neq j}\langle \vec{r}_i \cdot \vec{r}_j \rangle$ between or within the ensemble. The values $L_{ii},L_{ij}$ are obtained according to [A. R. Allnat](https://www.doi.org/10.1088/0022-3719/15/27/016). The data is provided as a `ReadOnlyList<EnsembleCorrelationData>`.

```csharp
var eval = new EnsembleCorrelationEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var particle1 = item.Particle1;
        var particle2 = item.Particle2;

        var vRiRi = item.RiRi; // Only if particle1 equals particle2
        var vRiRj = item.RiRj;
        var transportCoefficient = item.OnsagerCoefficient;

        Console.WriteLine($"Job {jobContext.DataId}: {particle1} and {particle2} have RiRi = {vRiRi} and RiRj = {vRiRj}");
    }
}
```

### [MobileTrackingEvaluation](#mobiletrackingevaluation)

This evaluation translates the mobile trackers for each mobile `IParticle` into a `Cartesian3D` displacement vector and provides the current `PositionId` in the lattice. The data is provided as a `ReadOnlyList<MobileTrackerResult>`.

```csharp
var eval = new MobileTrackingEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var positionId = item.PositionId;
        var particle = item.Particle;
        var shift = item.Displacement;
        Console.WriteLine($"Job {jobContext.DataId}: Shift [m] of {particle} at position {positionId} is {shift}.");
    }
}
```

### [GlobalTrackingEvaluation](#globaltrackingevaluation)

This evaluation provides the raw displacement data for `TransitionMobilityEvaluation` and thus gives access to the total `EnsembleDisplacement` of each `IParticle` and `ITransition` combination. The data is provided as a `ReadOnlyList<GlobalTrackerResult>`.

```csharp
var eval = new GlobalTrackingEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var displacementData = item.DisplacementData;
        var particle = item.TrackerModel.TrackedParticle;
        var transition = item.TrackerModel.KineticTransitionModel.Transition;
        Console.WriteLine($"Job {jobContext.DataId}: Total shift vector of {particle} via {transition} is {displacementData.VectorR}");
    }
}
```

### [CellSiteCoordinationEvaluation](#cellsitecoordinationevaluation)

This evaluation performs a radial sampling around each position of the lattice and averages the results for each symmetry equivalent `ICellSite`. The data for each site is further separated so that each center occupation `IParticle` has its own data set for each possible surrounding `IParticle` type. The data is provided as a `IDictionary<ICellSite, IDictionary<IParticle, SiteCoordination[]>>` where `ICellSite` is the center Wyckoff position, `IParticle` is the occupation of the center, and `SiteCoordination[]` provides the coordination and shell information for each `IParticle` that was found in the vicinity till a cut-off range in Å.

```csharp
var eval = new CellSiteCoordinationEvaluation(jobSet, maxDistance: 5.0);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var (site, occupationOptions) in data)
    {
        foreach (var (centerParticle, siteCoordinations) in occupationOptions)
        {
            foreach (var coordination in siteCoordinations)
            {
                // Get the surrounding particle and the affiliated shells
                var surroundingParticle = coordination.Particle;
                var shells = coordination.Shells;

                // E.g. get the distance of the first shell and the coordination number
                var firstShell = shells.First();
                var distance = firstShell.DistanceInAng;
                var cn = firstShell.CoordinationNumber;
            }
        }
    }
}
```

### [LocalSiteCoordinationEvaluation](#localsitecoordinationevaluation)

This evaluation is similar to `CellSiteCoordinationEvaluation` however the site coordination data is created for each position of the lattice and no average is created. The data is provided as an `IuNitCellProvider<SiteCoordination[]>` which can be used to lookup local coordination data by `Vector4I` or `Fractional3D` coordinates.

**Important:** The memory requirement of this evaluation is significantly higher than for `CellSiteCoordinationEvaluation`. It should not be used to evaluate many lattices at once.

```csharp
var eval = new LocalSiteCoordinationEvaluation(jobSet, maxDistance: 5.0);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    // Get a coordination of a lattice position either by 4D or fractional coordinates
    var coordination1 = data.GetCellEntry(0,0,0,0).Content;
    var coordination2 = data.GetEntryValueAt(new Fractional3D(0.25, 0.25, 0.25));

    // E.g. get the distance of the first shell and the coordination number
    var firstShell = coordination1.First().Shells.First();
    var distance = firstShell.DistanceInAng;
    var cn = firstShell.CoordinationNumber;
}
```

### [LatticeMetaEvaluation](#latticemetaevaluation)

A simple evaluation class that provides the size of the supercell as a `Vector4I` and the supercell volume. The data is provided as a `LatticeMetaData` object for each `JobContext`.

```csharp
var eval = new LatticeMetaEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    var latticeSize = data.SizeInfo;
    var latticeVolume = data.Volume;
    Console.WriteLine($"Job {jobContext.DataId}: Lattice size is {latticeSize} and the volume [m^3] is {latticeVolume}");
}
```

### [EnsembleMetaEvaluation](#ensemblemetaevaluation)

A simple evaluation class that combines the results of `ParticleCountEvaluation` and `LatticeMetaEvaluation` to provide the count and particle density for each `IParticle` ensemble. The data is provided as a `ReadOnlyList<EnsembleMetaData>`.

```csharp
var eval = new EnsembleMetaEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        var particle = item.Particle;
        var count = item.ParticleCount;
        var density = item.ParticleDensity;
        Console.WriteLine($"Job {jobContext.DataId}: Ensemble {particle} has {count} ions and a density of {density} [1/m^3]");   
    }
}
```

### [EnsembleJumpStatisticsEvaluation](#ensemblejumpstatisticsevaluation)

This evaluation creates convenience wrappers around the raw jump histograms of the simulation that exist for each `IGlobalTrackerModel`. The result allows to estimate the actual migration rate for specific `IParticle` and `ITransition` combinations and allows to load the histogram contents as `(double Energy, long Count)[]` arrays for further evaluation. The data is provided as a `ReadOnlyList<JumpStatisticsData>` and the evaluation class allows to pre-group the results by their affiliated `IParticle`.

```csharp
var eval = new EnsembleJumpStatisticsEvaluation(jobSet);
var dataWithJobZip = eval.Result.Zip(jobSet);

// Optional: The evaluation can group the result by the affiliated IParticle
var grouped = eval.GetGroupedEnsembles();

foreach (var (data, jobContext) in dataWithJobZip)
{
    foreach (var item in data)
    {
        // E.g. Get the energies and counts of the total migrtion barrier histogram
        var particle = item.GlobalTrackerModel.TrackedParticle;
        var transition = item.GlobalTrackerModel.KineticTransitionModel.Transition;
        var migEntries = item.GetMigrationBarrierEntries();
    }
}
```
