# The simulation model

## Description

The simulation model allows to define the different simulation types to be performed for a model. Objects defined here serve as a reference for later building of large quantified simulation collections. Thus all possible combinations of transitions can be defined here if required, each simulation job receives only the required information during simulation building. Note that simulation base types can either be MMC or KMC, they can however never be mixed.

## The simulation model objects

### [Basics](#basics)

The simulation definition in Mocassin has multiple hierarchy steps with inheritance in which the simulation model objects are the first. Each defined simulation can later be assigned to a job simulation collection which contains a set of individual simulation jobs. As shown in (1), the properties defined in the simulation model is inherited by the job collection unless the affiliated values are overwritten and then again inherited by each job unless explicitly overwritten there.

$$
[\text{Simulation Model Object}] \rightarrow [\text{Simulation Job Collection}] \rightarrow [\text{Simulation Job}]
\tag{1}
$$

The following model objects can be defined in the simulation model:

1. **Metropolis simulation**
   - Which sets of allowed MMC transitions exists?
2. **Kinetic simulation**
   - Which sets of allowed KMC transitions exists?

### [Metropolis simulation](#metropolis-simulation-base)

Each metropolis simulation is defined by assigning one or more [Metropolis transitions](./transition-model.md) objects and a set of fallback settings. It is usually recommended to leave the settings at the defaults defined by Mocassin as the common variation parameters are overwritten in the job collection anyway. The following general settings (both KMC & MMC) can be set:

- *Temperature* 
  - Defines the simulation temperature
- *Target steps per particle (MCSP)*
  - The minimal successful steps per mobile particle that are performed before the simulation terminates. Note that each position that is mutable during the simulation counts as a mobile particle.
- *Random number generator seed*
  - The simulation wide random number generator seed. This should be left empty in all cases
- *Time limit*
  - Sets a hard time limit after which the solver automatically terminates the simulation.
- *Minimal step rate*
  - Sets a lower limit for successful steps per second a simulation has to reach or it is automatically terminated due to inactivity
- *Job duplication count*
  - Sets how many implicit duplicates with different random number seeds of each simulation will be created to enable a proper statistical evaluation

Additionally, the MMC simulation provides specific settings:

- *Break check sample*
  - Sets a break check sample length in MCS which is used to verify if the MMC simulation has reached thermal equilibrium
- *Relative energy break tolerance*
  - Sets the relative tolerance for two mean energy values from consecutive break samples to be considered equal 


### [Kinetic simulation](#kinetic-simulation-base)

Each kinetic simulation is defined by assigning one or more [kinetic transitions](./transition-model.md) objects and a set of fallback settings. It is usually recommended to leave the settings at the defaults defined by Mocassin as the common variation parameters are overwritten in the job collection anyway. The same general settings as for MMC can be set. Additionally, the KMC simulation provides specific settings:

- *Relaxation steps per particle (MCSP)*
  - Set the number of steps per particle that are performed during the autonormalization and relaxation prerun
- *Normalization energy*
  - Set an energy value that will be used as the new upper acceptance limit instead of the default 0 eV (no normalization). This value can be used to deliberately overnormalize a simulation in cases where a few low barrier jumps prevent proper autonormalization
- *Electric field modulus*
  - Set the electric field strength in V/m
- *Electric field vector*
  - Set the direction of the electric field as an $(a,b,c)$ vector. This value cannot be changed later

## Model examples

### [Ceria](#ceria)

Using the previously defined oxygen migration and polaron hopping for, it is possible to defined three types of KMC simulation bases for ceria:

1. Create an oxygen migration only simulation
2. Create a polaron only simulation
3. Create a simulation where both oxygen migration and polaron hopping are active simultaneously