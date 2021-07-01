# Introduction - Preface

## What is Mocassin?

Mocassin stands for "Monte Carlo for Solid State Ionics" and is a Markov Chain Monte Carlo program for defect transport and defect distribution simulations in crystalline solids, mainly solid electrolytes, based on an Metropolis Monte Carlo (MMC) and kinetic Monte Carlo (KMC) solver implementation, respectively. It was originally created as a PhD project in cooperation between Forschungszentrum Jülich GmbH, IEK-12, Helmholtz Institute Muenster (HIMS) / Germany and RWTH Aachen University / Germany for ionic conductivity simulations in solid electrolyte bulk materials.

Mocassin aims for removing the need of redundant per-system code development using the idealized crystal geometry with fixed positions as a model parameter, which allows to process geometry and permutations based on space group symmetry operations. This way, simulation of many quantities can be done for arbitrary crystal systems without the need to write a single line of Monte Carlo code, including:

- MMC:
  - Defect distribution in thermal equilibrium
  - Simulated annealing with calculation of the free energy of interaction of the system
- KMC:
  - Ionic conductivity and mobility in an external electric field, including adiabatic small polaron hopping
  - Ionic diffusion, diffusion coefficients, and correlation factors
  - Average activation energy of ionic transport
  - Migration barrier distributions
  - ...

The model system supports multiple complex components in mostly arbitrary combinations to coexists within a single model to ensure broad applicability of the model system, including:

- Up to 63 custom mobile species per model
- Multiple mechanisms per model (vacancy, interstitialcy, vehicle movement, small polar hopping)
- Automatic detection and symmetry reduction of pair interactions based on a cutoff radius and filters
- Custom multi-body interactions clusters with up to 9 positions
- Local site energies
- Custom attempt rates for each symmetry reduced migration event in KMC
- ...

In combination with a job template system that allows to define an build simulations databases containing thousands of simulations in quickly and allowing direct modelling of interactions with data from first principles energy calculations, Mocassin especially targets use for large parameter studies of common variation parameters, such as doping fractions or temperature, or material screenings on HPC computing clusters where molecular dynamics simulations are either too expensive or proper potential sets are not available. Please refer to one of the affiliated publications for example use cases:

- [MOCASSIN release paper](http://dx.doi.org/10.1002/jcc.26418)
- [Proton conduction in BZY](https://doi.org/10.1038/s41563-019-0561-7)
- [Oxygen conduction in Sr/La Melilites](https://doi.org/10.1021/acs.chemmater.9b04599)

## License and legal disclaimer

Mocassin is distributed as free software under [GPL3+](https://www.gnu.org/licenses/gpl-3.0.de.html) without any warranties. The original copyright of the source code lies with Forschungszentrum Jülich GmbH Germany.

The original authors request that any use of Mocassin in scientific publications acknowledges the work of the original authors by an appropriate citation of the [release paper](http://dx.doi.org/10.1002/jcc.26418) without abbreviated author names.
