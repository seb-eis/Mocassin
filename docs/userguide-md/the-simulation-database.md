# The simulation database file ("msl")

## Description

The simulation database or "simulation library" file ".msl" stores simulation startup data, simulation result data, and the entire project XML required to restore the C# model definition. It is the data context that enables the unmanaged simulator to query marshalled/encoded simulation data structures and the C# evaluation system to process simulation results in the context of the underlying model.

## Information

**The only table in the msl file that is entirely human readable is the JobMetData table. It can be used to identify the indexed jobs outside of the C# model context. It is however recommended to use the C# evaluation API to write programs for data analysis as most model information is not easily available outside of the model context.**

### [The meta data table](#the-meta-data-table)

The meta data table (JobMetaData) helps to identify jobs outside of the C# model context. The following important columns are defined:

- **JobModelId**
  - The id of the job model which is identical with the "-jobId" argument on simulator startup
- **CollectionName**
  - The name of the job collection the job belongs to as defined by the user in the job template system
- **ConfigName**
  - The name of the job config the job belongs to as defined by the user in the job template system
- **CollectionIndex**
  - The index of the job collection in the translation process
- **ConfigIndex**
  - The index of the job config within its affiliated job collection
- **JobIndex**
  - The "duplicate" index of a job within its affiliated job config
- **Temperature**
  - The temperature of the simulation in [K]
- **ElectricFieldModulus**
  - The strength of the electric field in [V/m]
- **BaseFrequency**
  - The highest attempt frequency existing within the simulation
- **Mcsp**
  - The target steps per particle of the main simulation run
- **PreRunMcsp**
  - The traget setps per particle of a KMC simulation prerun
- **NormFactor**
  - The normalization factor as calculated from the normalization energy and temperature of the simulation
- **TimeLimit**
  - The internal time limit of the simulation in [s]
- **Flags**
  - The simulation flags set by the system and the user
- **DopingInfo**
  - A string that describes the doping of the supercell in a "[Index@Value]" pattern
- **LatticeInfo**
  - A string that describes the supercell size as an "a,b,c" vector

### [The job result table](#the-job-result-table)

The job result table (JobResultData) is a placeholder table that can be filled with simulation result data. It can hold the stdout text dump, the main run state, and the prerun state that are created by a simulator run. Please refer to [the state file page](./simulation-state-file.md) for further information on how-to read the contents of the state files.