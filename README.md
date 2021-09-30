# MOCASSIN Readme

## What is MOCASSIN?
MOCASSIN stands for "Monte Carlo for Solid State Ionics" and is a Markov Chain Monte Carlo program for simulations of defects in crystalline solids, primarily solid electrolytes. It supports both Kinetic (KMC) and Metropolis Monte Carlo (MMC) simulations in a unified manner based on ideal crystal structures with fixed positions and symmetry processing with space groups. This allows MOCASSIN to process arbitrary crystal geometries and greatly reduces the input effort by reducing the data to a symmetry reduced set. It is intended for highly customizable energy models where pair and cluster interactions can be directly modelled using first principles energy calculations.

The program was developed by Sebastian Eisele and Steffen Grieshammer at Helmholtz-Institute Münster (IEK-12) of Forschungs Zentrum Jülich GmbH / Germany and Institute of Physical Chemistry (Prof. M. Martin) of RWTH Aachen University / Germany. The authors thank John P. Arnold (Prof. Dr. Manfred Martin's group, RWTH Aachen University, Germany) and Lukas Eisele (Artiso Solutions GmbH, Blaustein, Germany) for their contributions.  

Please refer to the affiliated [open access publication](http://dx.doi.org/10.1002/jcc.26418) or the HTML documentation pages in release versions for further information. If you intend to use Mocassin for a research project and publish your work, we request/recommend that you cite [http://dx.doi.org/10.1002/jcc.26418](http://dx.doi.org/10.1002/jcc.26418) with all authors in full name. Example usages of MOCASSIN can be found in the following publications:

- [https://doi.org/10.4028/www.scientific.net/DF.29.117](https://doi.org/10.4028/www.scientific.net/DF.29.117)
- [https://doi.org/10.1038/s41563-019-0561-7](https://doi.org/10.1038/s41563-019-0561-7)
- [https://doi.org/10.1021/acs.chemmater.9b04599](https://doi.org/10.1021/acs.chemmater.9b04599)
- [https://doi.org/10.1039/D0CP06587K](https://doi.org/10.1039/D0CP06587K)
- [https://doi.org/10.1039/D1CP00925G](https://doi.org/10.1039/D1CP00925G)

<figure style="text-align: center">
    <img src ="./docs/userguide-md/figures/png/Logo.png" width="250">
</figure>

## What platforms are supported?
The system is developed for x86_64 only. 32-bit operating systems are not supported.

The C# model builder source code targets .NET Standard 2.0 and is Windows/Linux/MacOS portable. The C source is written for GNU GCC and Intel ICC compilers and can also be compiled for Windows/Linux using the MinGW64 toolchain, the MSVC compiler is not supported. The current graphical user interface (GUI) uses WPF and is Win64 only.

## How to build?

**Note: The full information on how to build can be found in the [documentation](./docs/userguide-md/building-mocassin.md). A complete step-by-step guide on setting up virtual machine for automated building can be found [here](./docs/build-vm-setup/readme.md)**

The WPF GUI comes with a Visual Studio solution file and should be compiled with Visual Studio or another appropriate IDE. The .NET Standard 2.0 libraries can also be compiled using the dotnet CLI command.

To compile the solver components on Linux systems ensure that CMake, Make, and a current GNU GCC or Intel ICC is installed. Than execute the following commands in the "McSolver" root directory:

```bash
mkdir build
cmake -B ./build -DCMAKE_BUILD_TYPE=Release
cmake --build ./build --config Release
```

To the compile the solver on Win64 systems, install CMake for Windows (https://cmake.org/download/) and an appropriate toolchain that supplies the Win64 ports of GCC and make. E.g., install MSYS2 (https://www.msys2.org/) and install the affiliated MinGW toolchain components using pacman:

```bash
pacman -S mingw-w64-x86_64-toolchain
```

Assuming that MSYS2 is used, the following commands build the solver using the PowerShell:
```PowerShell
mkdir ./build
cmake -B ./build -G "MinGW Makefiles" -DCMAKE_MAKE_PROGRAM:PATH=C:\msys64\mingw64\bin\mingw32-make.exe -DCMAKE_BUILD_TYPE=Release
cmake --build ./build --config Release
```

## Using the simulator
Using the simulator "Mocassin.Simulator" (Linux) or "Mocassin.Simulator.exe" (Windows) requires at least the mandatory arguments:
- "-dbPath": The path to the simulation job library (*.msl)
- "-ioPath": The path to the directory that will be used for I/O operations
- "-jobId": The ID of the job model data to be queried from the "*.msl" file
```bash
Mocassin.Simulator -dbPath <file> -jobId <integer> -ioPath <directory>
```
Additional optional arguments exist:
- "-stdout": Provide a filename for automated redirect of the stdout stream. The file is automatically written into the I/O directory specified by "-ioPath"

For scripted startup with parallelization using shared, distributed, or hybrid memory on HPC systems, please refer to the affiliated [script readme](./src/McSolver/Scripts/readme.md). A fully operational and convenient submit system for the SLURM workload manager is available, as well as a concurrency wrapper for usage with other workload management systems.
