# Job templates

## Description

Job or job translation templates describe which jobs to build into a Mocassin simulation library ".msl" file. Each library is a database that is accessed by the simulator to initialize the MC solver routines with the $P1$ extended and encoded model data available for the specified job.

## Template objects

### [Job collection](#job-collection)

A job collection is a set of jobs that share a common [simulation model object](./simulation-model.md), a common set of job settings, use one random number generator seed to generate the randomized contents of the collection, and share a common number of duplicates per included job configuration. as for the simulation model objects, both shared and MMC/KMC specific settings exist. The common MMC/KMC settings are:

- Temperature
  - Set the simulation temperature in Kelvin
- Time limit
  - Set the time limit for the simulation
- Steps per particle (MCSP)
  - Set the Monte Carlo steps per particle performed in the main run
- Minimal success rate
  - Set the minimal success rate as MCS per second that has to be reached for the simulation to continue
- Execution flags
  - Set one or more comma separated execution flags that affect how the simulator behaves
  - "NoSaving" causes the simulator to not generate and "mcs" state files
  - "NoJumpLogging" prevents the generation of transition energy histograms during KMC simulations
  - "UseFastExp" enables [approximation of the exponential function](https://nic.schraudolph.org/pubs/Schraudolph99.pdf) as published by N. N. Schraudolph for IEEE754 floating point numbers
- Instruction string
  - This allows to pass an instruction string to the database builder. Currently, the only option is to load and customize the [MMCFE custom routine](./mmcfe-routine.md)

The MMC only settings include:

- Relative energy break tolerance
  - Sets the relative tolerance for two mean energy values from consecutive break samples to be considered equal
- Break sample size
  - Sets a break check sample length in MCS which is used to verify if the MMC simulation has reached thermal equilibrium

The KMC only settings include:

- Relaxation steps per particle (MCSP)
  - The number of steps per particle performed in the KMC prerun
- Normalization energy
  - The new cutoff energy below which all transition attempts are automatically accepted. This value allows to deliberately overnormalize a simulation
- Electric filed modulus
  - The strength of the electric field in V/m
- Max attempt frequency
  - This values allows to defined a new max attempt frequency that replaces the max frequency found in the transition rules. All other frequencies are increased/decreased to maintain the original frequency ratios

#### [Selection optimizers](#selection-optimizers)

Each job collection allows to define selection optimizer objects. These optimizers define that a specific particle on a specific site should be exclude as a starting point from the transition selection process during simulation. This allows to greatly improve the simulation performance in cases where one ore more mobile species are in abundance and selecting them has a high chance of creating a site blocking event. For example, in the vast majority of vacancy migration cases, selecting only the minority species vacancy while excluding the majority species "mobile ion" has no effect on the validity of the simulation but greatly reduces the probability of site blocking events. Refer to [the simulator page](./the-simulator.md) for further information on the transition selection principle used during simulation.

**Important**: Complex transitions such as vehicles and interstitialcy can never be selected by their involved vacancy (see [transition model](./transition-model.md)). If your model contains these types of transitions, always remove all vacancies of all sublattices that serve as the starting point for these transitions using the optimizers.

### [Job configuration](#job-configuration)

A job configuration describes an individual simulation job for which the defined number of job duplicates will be created and placed into the simulation library. All settings that are available for a job collection exist for the job configuration as well and can overwrite the data inherited from the parent collection. Additionally, the job configuration defines the non yet set supercell $A,B,C$ site information and the quantifies each doping defined in the model. It is important to consider that a supercell has a finite number of atoms $N \in \N$ and thus one must be careful when choosing doping fractions and supercell sizes.

Each doping quantification $x_i$ is a fraction value $x_i\in[0...1]$ and describes the fraction of original particles replaced by the dopant as defined in the primary occupation exchange of the doping object. That is, each fraction affects the particles remaining after all dopings with a higher priority have already been applied. For example, defining an identical $A,B$ replacement twice and applying them as consecutive dopings $x_1,x_2$ to a total particle count $N_\mathrm{tot} \in \N$ yields particle replacements $\Delta N_i \in \N$ according to (1), where the $\mathrm{trunc(x)}$ function returns the closest integer that is less or equal to the floating point number $x$.

$$
\begin{aligned}
    \Delta N_1 &= \mathrm{trunc}(x_1 \cdot N_\mathrm{tot}) \\
    \Delta N_2 &= \mathrm{trunc}(x_2 \cdot (N_\mathrm{tot} - \Delta N_1))
\end{aligned}
\tag{1}
$$

In general, this gets a little more complex as many dopings define a dependent exchange that has a non integer exchange ratio relative to its primary exchange. For example, creating polarons $\mathrm{Ce_{Ce}'}$ in ceria creates oxygen vacancies $\mathrm{v_O^{\bullet \bullet}}$ in a two to one ratio, that is, relation (2) has to be satisfied in the generated lattice. Obviously, if $N_i \in N$ must be true, then $N_\mathrm{Ce_{Ce}'} \in 2n$ with $n \in \N$ must be fulfilled as well.

$$
N_\mathrm{Ce_{Ce}'}=\frac{1}{2}N_\mathrm{v_O^{\bullet \bullet}}
\tag{2}
$$

In order to achieve this, the supercell generation system takes the truncated value of the primary exchange $\Delta N_p$, here $\mathrm{Ce_{Ce}'}$, and decrements the integer until first reaching a point where the dependent exchange can also be expressed by an integer for the given supercell. This means, in many cases the actual doping applied to the lattice can be slightly lower than the value defined in the job configuration in order to maintain proper charge balancing of the supercell.