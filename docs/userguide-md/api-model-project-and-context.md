# The ModelProject and ProjectModelContext classes

## Description

When working with the Mocassin API it is usually required to get data from the symmetry reduced model description created by the user and the affiliated model context created by Mocassin that connects the symmetry-reduced model, the symmetry-extended model, and the encoded/indexed unmanaged data used in the simulations. The former is managed by the `ModelProject` class providing the `IModelProject` interface and the later is provided by the `ProjectModelContext` class through the `IProjectModelContext` interface. Since `IProjectModelContext` instances are always created from a model description, the affiliated `IModelProject` can be accessed through the `IProjectModelContext`.

While it is not necessary to understand how the `ModelProject` class manages the data in detail, there are several API calls that need to be known to make full use of the existing functionality. The `IProjectModelContext` serves primarily as a data source for API users and its components should be known when working with the API. In the following, it will be explained how to get and work with ready `IModelProject` and `IProjectModelContext` using filled simulation databases. The topic of how to build them to create and translate projects programmatically without the GUI will not be covered.

## Usage

### [The object and the indexed world](#working-with-the-model-and-model-context-instances)

When working with `IModelProject` and `IProjectModelContext` it is fundamental to understand that the C simulator uses indices to identify and access unmanaged data structures while the C# model system works with objects and interfaces. The `IProjectModelContext` creates the bridge between these two worlds and allows to translate between the two representations. In the following guide pages terms like `ParticleId` or `PositionId` will refer to the index of a `IParticle` model object or `Position` definitions.

The most important indexing principle to understand is the `Vector4I` structure that encodes 3D coordinate information (`Cartesian3D` or `Fractional3D` structures) as a 4D integer tuple $(z_a,z_b,z_c,z_p)$. Here, $z_a,z_b,z_c$ are the integer unit cell offsets in $a,b,c$ direction and $z_p$ is the `PositionId` (or id delta) within the unit cell. The index $z_p$ is defined by a sorted set $\{p_0,p_1,...,p_n\}$ of all positions $i$ and their fractional coordinates $\vec{x}_{0,i}(z_i)$ that exist within a $P1$ extended unit cell. Thus, any `Vector4I` can be trivially converted into a `Fractional3D` according to (1).

$$
\vec{x}(z_a,z_b,z_c,z_p) = z_a\vec{a}+z_b\vec{b}+z_c\vec{c}+\vec{x}_{0,i}(z_p)
\tag{1}
$$

The backwards conversion is more complex. In general, the Mocassin API provides two interfaces for transforming between different coordinate systems and encoding/decoding the `Vector4I` information as described [below](#getting-the-most-relevant-manager-functionalities).

Additionally, since computer memory is linear the `Vector4I` actually maps to a 1D buffer of a 4D row-major array implementation using the dimension mappings $(z_a,z_b,z_c,z_p)\rightarrow(d_0,d_1,d_2,d_3)$ where all indices $\in\N$. The ".mcs" file lattice is thus provided as an 1D array and must be decoded before usage. While the Mocassin API again provides helpers to decode the 1D lattice representation, it is important to understand how to covert the `Vector4I` to a linear index and how to translate $(z_a,z_b,z_c)$ into its `UnitCellId` within a supercell:

```csharp
public Vector4I MapIndexToVector4(int index, in Vector4I latticeSize)
{
    // Calculate the block sizes of the row-mayor 4D array, the block size of dim3 is 1
    var blockLengthC = latticeSize.P;
    var blockLengthB = blockLengthC * latticeSize.C;
    var blockLengthA = blockLengthB * latticeSize.B;

    // Stepwise decode of the index to the 4D components using `index` to store the division remainder
    var a = Math.DivRem(index, blockLengthA, out index);
    var b = Math.DivRem(index, blockSLength, out index);
    var c = Math.DivRem(index, blockSLength, out index);
    return new Vector4I(a, b, c, index);
}

public int MapVector4ToUnitCellIndex(in Vector4I vector, in Vector4I latticeSize)
{
    // Calculate the skip lengths and return the offset
    var lengthA = latticeSize.B * latticeSize.C;
    var lengthB = latticeSize.C;
    vector.A * lengthA + vector.B * lengthB + vector.C;
}
```

### [Restoring the context instance from a simulation database](#restoring-the-context-instance-from-a-simulation-database)

There are usually two scenarios in which the `IProjectModelContext` must be restored from an ".msl" file: (i) for data evaluation; and (ii) for manipulating simulation databases after deploy, e.g. to inject a custom simulation lattice.

Assuming that a `MslEvaluationContext` is created to generate `JobContext` instances for evaluation, the affiliated `IProjectModelContext` instances are automatically restored when data is loaded since they are needed for most evaluations. Thus, the `IProjectModelContext` is a public property of each `JobContext` and can be accessed directly:

```csharp
// Open a context that will be disposed if the scope is left
using var mslContext = MslEvaluationContext.Create("./myproject.msl");

// Load the first job of the database
var jobContext = mslContext.LoadJobsAsEvaluable(query => query).First();

// Get the IProjectModelContext and IModelProject interfaces for the job
var modelContext = jobContext.ModelContext;
var modelProject = jobContext.ModelContext.ModelProject
```

Sometimes, it is required to access a simulation database that has no result data yet. Here, querying against the `MslEvaluationContext` using `EvaluationJobSet()` yields no results. In these cases the simulation database can be accessed using the `SimulationDbContext` and the `IProjectModelContext` can be restored manually. The `SimulationDbContext` context also allows to manipulate the job data stored in the database so it can be used to inject data into the database when required. The access is simple but requires additional `using` includes:

```csharp
// Some usings are required here (place outside namespace!)
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.Translator;
using Mocassin.UI.Data.Helper;

// Open the database
using var dbContext = SqLiteContext.OpenDatabase<SimulationDbContext>(mslPath);

// Load a SimulationJobModel from the context
var jobModel = dbContext.JobModels.Include(x => x.SimulationJobPackageModel)

// Call the helper method to restore the IProjectModelContext
var modelContext = MslHelper.RestoreModelContext(jobModel.SimulationJobPackageModel);
```

### [Accessing services and the model managers](#accessing-model-managers)

The model data objects, such as, `IParticle` or `ICellSite`, that store the user-provided data and their affiliated functionality can be found on a set of `IModelManager` interfaces that are registered with the `IModelProject`. There are currently six managers registered with a default `ModelProject`:

```csharp
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.Model.Simulations;
using Mocassin.Model.Energies;
using Mocassin.Model.Lattices;

var particleManager = modelProject.Manager<IParticleManager>();
var structureManager = modelProject.Manager<IStructureManager>();
var transitionManager = modelProject.Manager<ITransitionManager>();
var simulationManager = modelProject.Manager<ISimulationManager>();
var energyManager = modelProject.Manager<IEnergyManager>();
var latticeManager = modelProject.Manager<ILatticeManager>();
```

In the vast majority of cases, a user will only require the data access port of the manager to query stored data and functionalities:

```csharp
var particleDataAccess = modelProject.Manager<IParticleManager>().DataAccess;
```

The `IModelProject` further provides a access to the space group and space group operations through the `ISpaceGroupService` and two `INumericService` interfaces that supply numeric `IComparer<double>` instances that handle almost equal comparisons for floating point numbers. By default these services will automatically have loaded the data that was defined when the `IModelProject` was restored from the simulation database.

```csharp
// Get the space group
var sgService = modelProject.SpaceGroupService

// Get the numeric comparison provider used for geometric comparisons
var geoemtryComparers = modelProject.GeometryNumeric

// Get the numeric comparison provider used for non-geometric comparisons
var commonComparers = modelProject.CommonNumeric
```

### [Querying model objects from the model project](#querying-model-objects-from-the-model-project)

The most commonly required data from the `IModelProject` are the `IModelObject` implementations, such as, `IParticle`, `ICellSite`, etc., which have been created based on the user input and are managed by different `IModelManager` instances registered on the `IModelProject`. One way to get the model objects is to get the affiliated manager and then query data from it directly. This is done in the following for an `IParticle` and `IParticleSet` by locating the `IParticleManager`.

```csharp
var dataAccess = modelProject.Manager<IParticleManager>().DataAccess;
var particles = dataAccess.Query(x => x.GetParticles());
var particleSets = dataAccess.Query(x => x.GetParticleSets());
```

Since this approach is tedious and requires to first locate the affiliated `IModelManager`, there is a more convenient approach that uses the `IModelDataTracker` interface of the `IModelProject`. This service manages the location of all `IModelObject` types that any manager registers and allows to directly lookup any model object. This is especially useful since the objects are indexed within a simulation and these functions directly provide an array `T[]` that can be accessed by this index or allow to lookup a specific index/key:

*Note: Key based lookup of dynamically created `IModelObject` instances is usually not feasible if the model was created using the GUI as the keys will be `GUIDs`.*

```csharp
// Get the data tracker interface
var dataTracker = modelProject.DataTracker;

// Enumerates all IModelObject of a type T
var particlesEnum = dataTracker.Objects<IParticle>();

// Maps all IModelObject of a type T to an array T[] where each entry is placed at the index of the object 
var particles = dataTracker.MapObjects<IParticle>();

// Find an IModelObject of a type T by index or key
var particle1 = dataTracker.FindObject<IParticle>(1); // Get the particle with index 1
var particle0 = dataTracker.FindObject<IParticle>("Particle.Void"); // Get the 'void' particle by its key
```

### [Getting the most relevant manager functionalities](#getting-the-most-relevant-manager-functionalities)

A common data evaluation will not require many functionalities of the `IModelManager` objects aside from the `IModelObject` instances. There are however a two key exceptions to this rule when it comes to handling structural information: (i) getting the `IUnitCellVectorEncoder` and `IVectorTransformer` that handle conversions between `Vector4I`, `Cartesian3D`, `FRactional3D`, and `Spherical3D` coordinate representation; and (ii) the `IUnitCellProvider<ICellSite>` that serves as a $1\times1\times1$ dummy supercell. The can be obtained from the `IStructureManager` data access:

```csharp
var dataAccess = modelProject.Manager<IStructureManager>().DataAccess;

// Get the unit cell vector encoder and transformer that handles coordinate transformations
var encoder = dataAccess.Query(x => x.GetVectorEncoder()); // Handles the 4D integer encode/decode
var transformer = encoder.Transformer; // Handles the cartesian, fractional, and spherical transforms

// Get the unit cell provider that serves as an 1x1x1 supercell dummy of ICellSites
var ucp = dataAccess.Query(x => x.GetFullUnitCellProvider());
```

Again, since these are important and needed for many evaluation purposes, there are two extension methods for the `IProjectModelContext` that look them up directly:

```csharp
using Mocassin.Tools.Evaluation.Extensions; // Provides the extension methods

// Get the unit cell provider
var ucp = modelContext.GetUnitCellProvider();

// Get the vector encoder
var encoder = modelContext.GetUnitCellVectorEncoder();
```

### [Using the model context components](#using-the-model-context-components)

Aside from the `IModelProject` instance, the `IProjectModelContext` provides four sub-context instances that provide the symmetry extended information of the model:

- `IStructureModelContext`: Contains all information related to structure, symmetry, and position environments
- `IEnergyModelContext`: Contains all information related to clusters, pair interactions, and defect energies
- `ITransitionModelContext`: Contains all information related to KMC and MMC transitions
- `ISimulationModelContext`: Contains all information about which data is required for which simulation and how the data is indexed/encoded for the simulator

What the properties and sub-models of these contexts describe is well documented within the source code. Here, only the often required mappings and indexing information shall be explained in more detail. When accessing an `IKineticSimulationModel` or `IMetropolisSImulationModel` from the `ISImulationModelContext` there are two important sub-models that contain additional indexing information:

- `ISimulationEncodingModel`: Contains encoding and indexing information for transition collections and transition directions
- `ISimulationTrackingModel`: Contains the information which global and static trackers exist and how they are indexed

The can be accessed as follows:

```csharp
// Get the first KMC and MMC simulation model
var kmcSimModel = projectContext.SimulationModelContext.KineticSimulationModels.First();
var mmcSimModel = projectContext.SimulationModelContext.MetropolisSimulationModels.First();

// Get the encoding model and tracking model
var encodingModel = kmcSimModel.SimulationEncodingModel
var trackingModel = kmcSimModel.SimulationTrackingModel
```

Properties of the the `ISImulationEncdoingModel` potentially important for a user writing an evaluation are the index mappings of the `ITransitionModel` instances (describing the user defined transition collection) and their affiliated relative index mappings for `ITransitionMappingModel` instances (describing the individual directions). They are simple dictionaries that can be accessed by:

```csharp
// Get a dictionary that maps ITransitionModel instances to their jump collection index
var jumpCollectionIdMapping = encodingModel.TransitionModelToJumpCollectionId;

// Get a dictionary that maps ITransitionMappingModel instnaces to their relative jump direction index
var jumpDirectionIdMapping = encodingModel.TransitionMappingToJumpDirectionId;
```

The `ISimulationTrackingModel` will be required more often as it is required to process the static and global tracking information of the simulation. Aside from the information what they track, the also provide a `ModelId` property which is their index in the simulation code:

```csharp
// Get the first of each tracker model from the ISImulationTrackingModel
var staticTracker = trackingModel.StaticTrackerModels.First();
var gloablTracker = trackingModel.GlobalTrackerModels.First();

// Get the global tracker index, the tracked transition, and the tracked particle
var gTrackerIndex = globalTracker.ModelId;
var gTrackedParticle = gloablTracker.TrackedParticle;
var gTrackedTransition = globalTracker.KineticTransitionModel;

// Get the same from the static tracker model
var sTrackerIndex = globalTracker.ModelId;
var sTrackedParticle = gloablTracker.TrackedParticle;
var sTrackedTransition = globalTracker.KineticTransitionModel;
```

**Important:** The static trackers are indexed per unit cell to make them independent of the supercell size. When using these indices always remember to add $n_t\times id_{\text{cell}}$ where $n_t$ is the length of the static tracker list and $id_{\text{cell}}$ is the linearized index of the unit cell as calculated [above](#the-object-and-the-indexed-world).