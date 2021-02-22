# The MslEvaluationContext class

## Description

Advanced data evaluation using the Mocassin API evolves around a context class for msl files that contain simulation results. From this context, job context instances can be queried that provides a reader for the binary state file (run.mcs or prerun.mcs). The evaluation API can be used easily from C# or F#, the following example codes are constraint to the C# usage.

**Note:** The simulation database file (.msl) is an SQLite database that is accessed from .NET via the object relation mapper Entity Framework Core. The MslEvaluationContext has the database context in the background but is not itself an EF Core DbContext class and opens the database as readonly to prevent accidental manipulation of raw data.

**Important: EF core does not use lazy loading by default. If C# is used to query data from a DbContext then `Microsoft.EntityFrameworkCore` is required as a `using` to have the `Include(...)` extension method for loading navigation properties when buidling an `IQueryable<T>`. The F# `query {...}` computation expression automatically includes navigation properties if required.**

## Usage

### [Creating a context and loading a jobset](#creating-a-context-and-loading-a-jobset)

Creating a `MslEvaluationContext` instance to load job sets is straightforward. The following examples shows how to create the context, create an `IQueryable<SimulationJobModel>` that queries all jobs with a temperature above 1000K in ascending order when evaluated, and instruct the context to load the data and supply an `IEvaluableJobSet` interface for the jobs.

```csharp
using Mocassin.Tools.Evaluation.Context;
using Microsoft.EntityFrameworkCore; // Important when IQueryable<T>.Include(...) is required

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // Open a context that will be disposed if the scope is left
            using var mslContext = MslEvaluationContext.Create("./myproject.msl");
            
            // Create a query to load some jobs
            var jobQuery = mslContext.EvaluationJobSet()
                .Where(x => x.JobMetaData.Temperature > 1000.0)
                .OrderBy(x => x.JobMetaData.Temperature);

            // Load the jobs as a set of JobContext instances
            var jobSet = mslContext.MakeEvaluableSet(jobQuery);
        }
    }
}
```

**Important:** Always use the `mslContext.MakeEvaluableSet(...)` method to load the data. This causes the context to register the provided job context information and disposes them once the `MslEvaluationContext` is disposed. Failing to do so will cause severe memory leaking as the binary state readers pin data in memory to prevent the data from being moved by the garbage collector. This is done to prevent memory access violations observed in multithreading scenarios when using the `ReadonlySpan<T>` reference structure with frequent garbage collection.

The `MslEvaluationContext` also provides a convenience wrapper that combines building the query using `EvaluationJobSet()` and `MakeEvaluableSet()` into a single functions that takes an expression to manipulate the query. The call above then becomes:

```csharp
// Open a context that will be disposed if the scope is left
using var mslContext = MslEvaluationContext.Create("./myproject.msl");

// Load the jobs as a set of JobContext instances
var jobSet = mslContext.LoadJobsAsEvaluable(query => query
                            .Where(x => x.JobMetaData.Temperature > 1000.0)
                            .OrderBy(x => x.JobMetaData.Temperature));
```

### [Using an evaluation class](#using-an-evaluation)

The `IEvaluableJobSet` is intended for usage with implementation of the `JobEvaluation<TResult>` class. Mocassin provides ready implementations for some of the basic tasks, e.g. evaluating the conductivity, diffusion, or displacements of ensembles.

**Note:** Careful, some of the evaluations, such as coordination number analysis for each lattice position, create huge amounts of data and evaluating thousands of jobs at once may cause memory issues.

```csharp
using var mslContext = MslEvaluationContext.Create("./myproject.msl");
var jobQuery = mslContext.EvaluationJobSet()
    .Where(x => x.JobMetaData.Temperature > 1000.0)
    .OrderBy(x => x.JobMetaData.Temperature);
var jobSet = mslContext.MakeEvaluableSet(jobQuery);

// Create a new evaluation that targets the job set
var evaluation = new EnsembleDiffusionEvaluation(jobSet);

// Run the evaluation on the task pool
var evalTask = evaluation.Run();

// Get the result. Calling this property will implicilty await the result or call the Run() method
var result = evaluation.Result;
```

### [Implementing an evaluation class](#using-an-evaluation)

Implementing a new `JobEvaluation<TResult>` is straightforward and requires only to implement `GetValue()` and the base constructor. Most modern IDEs or editors can build the raw skeleton structure automatically. It is optionally possible to set the `ExecuteParallel` property to automatically perform `GetValue()` in parallel. Shown below is a simple example implementation that collects the total cycle count from each simulation job in parallel.

```csharp
// Implement the base class and specify the type of TResult
class MyCycleCountEvaluation : JobEvaluation<long>
{
    // Implement the base constructor
    public MyCycleCountEvaluation(IEvaluableJobSet jobSet) : base(jobSet)
    {
        // Evaluations can be marked for automatic parallel execution on the task pool
        ExecuteParallel = true;
    }

    // Implement how to get the result from a single JobContext instance, here the cycle count
    protected override long GetValue(JobContext jobContext)
    {
        return jobContext.McsReader.ReadHeader().CycleCount;
    }

    // Optional: Prepare for execution, e.g. check if all required data is loaded
    protected override void PrepareForExecution()
    {
        base.PrepareForExecution();
    }
}
```