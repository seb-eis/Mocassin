# Understanding the contents of an "mcs" file

## Description

Mocassin stores all simulation raw data of a simulation into a binary state file with the extension "mcs". This file is a buffer dump as used by the C simulator and can be read from the C# API using the `McsContentReader` class. As all data is stored in linear buffers, the data has to be decoded using the indices created by the `ProjectModelContext`. It is thus recommended to use the reader that is implicitly provided by `JobContext` instances when loading simulation data using an `MslEvaluationContext`.

The file contains the following data:

1. A Header that marks start and end index of certain data in the buffer
2. Meta data, e.g. simulation runtime
3. The state of the simulation lattice
4. The counter information for attempts, rejected jumps, accepted jumps, etc. of each particle
5. The global trackers for each relevant particle and transition combination (KMC only)
6. The mobile trackers for each moving particle in the system (KMC only)
7. The static trackers for each relevant position and particle combination (KMC only)
8. The mobile tracker indexing that defines which mobile tracker belongs to which lattice position (KMC only)
9. The jump statistics histograms for each relevant transition and particle combination (KMC only)

**Note:** Some of the tasks shown in the following are also conveniently provided through a predefined `JobEvalaution<T>` implementation as described on the [next page](./api-ready-evaluation-classes.md).

## Usage

### [Accessing the "mcs" content](#accessing-the-mcs-content)

To open and read the contents of an "mcs" file (usually `run.mcs` or `prerun.mcs`) without the simulation context, the Mocassin API provides a simple reader class. It is important to call `Dispose()` on this reader as it reads binary data directly as structs through `ReadOnlySpan<T>` references structures and pins the buffer in the background to prevent relocation by the garbage collection.

```csharp
// Option 1: Create a reader from byte[]
var bytes = File.ReadAllBytes("run.mcs");
var reader = McsContentReader.Create(bytes);

// Directly read the mcs file
var reader2 = McsContentReader.Create("run.mcs");
```

In most scenarios the content is read as a part of a `JobEvaluation<T>` implementation and can simply be accessed using a `JobContext` instance provided by `MslEvaluationContext`. This also allows to read the pre-run state `prerun.mcs` if required.

```csharp
// Use the msl context to get a reader. An optional flag allows to target the secondary state (prerun.mcs)
var context = MslEvaluationContext.Create("./myproject.msl");
var job = context.LoadJobsAsEvaluable(x => x, targetSecondaryState: false)
                 .First();
var reader = job.McsReader;
```

In most cases the only property of the ".mcs" file header that should be manually read is `CycleCount` which provides the total number of performed simulation cycles. Correct access to other contents is provided by the affiliated reader functions.

```csharp
// Get the header
var header = reader.Header;

// Read the meta data
var metaData = reader.ReadMetaData();

// Read the lattice state
var lattice = reader.ReadLattice();

// Read all counter data
var counters = reader.ReadCycleCounters();

// Read all global tracker data
var globalTrackers = reader.ReadGlobalTrackers();

// Read all mobile tracker data
var mobileTrackers = reader.ReadMobileTrackers();

// Read all static trackers
var staticTRackers = reader.ReadStaticTrackers();

// Read the mobile tracker indexing
var trackerMapping = reader.ReadMobileTrackerMapping();

// Read all jump histogram data
var histograms = reader.ReadJumpStatistics();
```

**Note:** In C# script files it is not possible to define a local variable of a `ref struct` in the main script body as they are stored as a `field` or `property`. This can be circumvented by wrapping the main body into a function to have local variables.

### [Accessing the raw counter data](#accessing-the-raw-counter-data)

The raw counter data structure `McsCycleCounter` exists for each `IParticle` and is indexed by the affiliated `Index` property. Each entry can either be directly accessed by the index or the data can be converted into a `Dictionary` that uses `IParticle` as the key.

```csharp
// Get the required data
var counters = reader.ReadCycleCounters();
var particles = jobContext.ModelContext.ModelProject.DataTracker.MapObjects<IParticle>();

// Convert the raw data into a Dictionar<IParticle, McsCycleCounter> for easy access
var dictionary = new Dictionary<IParticle, McsCycleCounter>(counters.Length);
foreach (var particle in particles)
{
    dictionary.Add(particle, counters[particle.Index]);
}
```

### [Accessing the raw lattice data](#accessing-the-raw-lattice-data)

The raw lattice is a simple `byte[]` array where each `byte` is the index of the `IParticle` that occupies the position. The Mocassin API provides helpers through the `SimulationMapping` and `MslHelper` classes to decode the linear buffer into all available coordinate types.

```csharp
// Get the required data
var particles = jobContext.ModelContext.ModelProject.DataTracker.MapObjects<IParticle>();
var latticeSize = MslHelper.ParseLatticeSizeInfo(jobContext.JobModel.JobMetaData);
var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
var lattice = reader.ReadLattice();

// Make a decoder delegate (There are also versions that decode to a single ccoordinate type)
var decoder = SimulationMapping.GetPositionIndexToCoordinateMapper(latticeSize, vectorEncoder);

// Build a list of tuples of the coordinates and the affiliated particle
var result = new List<(IParticle, Vector4I, Fractional3D, Cartesian3D)>(lattice.Length);
for (var i = 0; i < lattice.Length; i++)
{
    var particleId = lattice[i];
    var particle = particles[particleId];
    var (vector4, fractional, cartesian) = decoder.Invoke(i);
    result.Add((particle, vector4, fractional, cartesian));
}
```

### [Accessing the raw tracker data](#accessing-the-raw-tracker-data)

The three types of tracking data collected in KMC simulations a describe [here](./movement-tracking.md) use distinct indexing and must be decoded accordingly. 

The `global tracker` raw data is indexed using the `ModelId` property of the affiliated `IGlobalTrackerModel` instances. Each tracker describes the total displacement vector of an `IParticle` ensemble due to a specific `ITransition` which are defined on the affiliated `IGlobalTrackerModel`.

```csharp
// Get the required data
var trackerModels = jobContext.SimulationModel.SimulationTrackingModel.GlobalTrackerModels;
var trackers = reader.ReadGlobalTrackers();

// Create a list of typles that provide the tracker model and the shift vector in fractional coordinates
var result = new List<(IGlobalTrackerModel, Fractional3D)>();
foreach (var model in trackerModels)
{
    // This just shows how to get the tracked particle and the transition model object that belongs to the tracker
    var trackedParticle = model.TrackedParticle;
    var transition = model.KineticTransitionModel.Transition;

    var tracker = trackers[model.ModelId];
    var shiftVector = tracker.AsVector();
    result.Add((model, shiftVector));
}
```

The `static tracker` raw data is indexed using the `PositionId` in the unit cell and the linear `UnitCellId` obtained from the $(a,b,c)$ offset of the unit cell. Since there is usually not a tracker for each position of the lattice it is important to not confuse the `StaticTrackerId` with the `PositionId` in the lattice.

```csharp
// Get the required data
var trackerModels = jobContext.SimulationModel.SimulationTrackingModel.StaticTrackerModels;
var trackers = jobContext.McsReader.ReadStaticTrackers();
var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
var latticeSize = MslHelper.ParseLatticeSizeInfo(jobContext.JobModel.JobMetaData);
var decoder = SimulationMapping.GetPositionIndexToFractionalMapper(latticeSize, vectorEncoder);

// Create a list of the tracked particle, the fractional position, and the total shift vector
var result = new List<(IParticle Particle, Fractional3D Position, Fractional3D Shift)>(trackers.Length);

// The outer loop handles the tracker index and position index offsets
var positionIndexOffset = 0;
for (var trackerIdOffset = 0; trackerIdOffset < trackers.Length; trackerIdOffset += trackerModels.Count)
{
    // The inner loop handles all trackers for the current unit cell
    foreach (var model in trackerModels)
    {
        var trackerId = model.ModelId + trackerIdOffset;
        var positionId = model.TrackedPositionIndex + positionIndexOffset;

        var shift = trackers[trackerId].AsVector();
        var trackedParticle = model.TrackedParticle;
        var position = decoder.Invoke(positionId);
        result.Add((trackedParticle, position, shift));
    }

    // Advance the position index offset by the length of the unit cell
    positionIndexOffset += vectorEncoder.PositionCount;
}
```

The `mobile tracker` raw data is indexed by the data provided in the ".mcs" file since it changes during simulation. The indexing is simply a buffer of `int` which mark the `PositionId` in the lattice. A tracker exists only for particles that can move during simulation; thus, there are usually far less trackers than lattice positions.

```csharp
// Get the required data
var particles = jobContext.ModelContext.ModelProject.DataTracker.MapObjects<IParticle>();
var vectorEncoder = jobContext.ModelContext.GetUnitCellVectorEncoder();
var latticeSize = MslHelper.ParseLatticeSizeInfo(jobContext.JobModel.JobMetaData);
var decoder = SimulationMapping.GetPositionIndexToFractionalMapper(latticeSize, vectorEncoder);
var lattice = reader.ReadLattice();
var trackers = reader.ReadMobileTrackers();
var mapping = reader.ReadMobileTrackerMapping();

// Create a list of all mobile particles, their current position, and their displacement vector
var result = new List<(IParticle Particle, Fractional3D Position, Fractional3D Shift)>(trackers.Length);
for (var i = 0; i < trackers.Length; i++)
{
    var positionId = mapping[i];
    var particleId = lattice[positionId];

    var shift = trackers[i].AsVector();
    var position = decoder.Invoke(positionId);
    var particle = particles[particleId];
    result.Add((particle, position, shift));
}
```
### [Accessing the raw jump statistics](#accessing-the-raw-jump-statistics)

The jump statistics histograms exist for each `IGlobalTrackerModel` and collect data about the migration energies that occurred during migration attempts. Each statistics entry provides four separate histograms (see the [energy model page](./energy-model.md) for details on the contributions). Each histogram samples in the range $[0, E_\text{max})$ with an accuracy of $\Delta E = E_\text{max} / 1000$ (see [here](./the-simulator.md#using-the-simulator) for overwriting the default $E_\text{max}=10 \text{eV}$) has an overflow and underflow counter. To increase the accuracy, the affiliated `#define` in the simulator source code has to be changed an then the source code has to be recompiled.

1. The raw transition state contribution $\sum_i E_i(S_1)$ of all involved unstable positions $i$, that is, the additive part of $E_{\text{mig}}$ that is influenced by the surroundings of the unstable transition positions only.
2. The interpolated configuration contribution $\Delta E_\text{conf}(S_0,S_2)$ from the stable states $S_0, S_2$ for $\Delta E_\text{conf} < 0 \text{eV}$
3. The interpolated configuration contribution $\Delta E_\text{conf}(S_0,S_2)$ from the stable states $S_0, S_2$ for $\Delta E_\text{conf} > 0 \text{eV}$
4. The total migration barrier $E_\text{mig}$ including the electric field contribution

Instead of trying to read the raw data manually, the Mocassin API provides a small wrapper class `JumpStatisticsData` that can be build an handles correct reading of the raw histogram information.

```csharp
// Get the relevant data
var globalTrackerModels = jobContext.SimulationModel.SimulationTrackingModel.GlobalTrackerModels;
var temperature = jobContext.JobModel.JobMetaData.Temperature;
var lattice = reader.ReadLattice().ToArray();

// Make a local helper function to get the ensemble size of an IParticle
int GetEnsembleSize(IParticle particle) => lattice.Count(x => x == particle.Index);
    
// Use a LINQ expression to build the JumpStatisticsData objects
var result = globalTrackerModels
             .Select(trackerModel => new JumpStatisticsData(jobContext.McsReader, trackerModel, temperature, GetEnsembleSize(trackerModel.TrackedParticle)))
             .ToList();
```