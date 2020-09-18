# MOCASSIN Readme

## What is MOCASSIN?
MOCASSIN stands for "Monte Carlo for Solid State Ionics" and is a Markov Chain Monte Carlo program for simulations of defects in crystalline solids, primarily solid electrolytes. It supports both Kinetic (KMC) and Metropolis Monte Carlo (MMC) simulations in a unified manner based on ideal crystal structures with fixed positions and symmetry processing with space groups. This allows MOCASSIN to process arbitrary crystal geometries and greatly reduces the input effort by reducing the data to a symmetry reduced set. It is intended for highly customizable energy models where pair and cluster interactions can be directly modelled from first principles energy calculations.

The system was developed as a PhD project by RWTH Aachen University / Germany and Forschungszentrum Jülich GmbH / Germany. Please refer to the affiliated open access publication for further information.

## What platforms are supported?
The system is developed for x86-64 as this is the most common processor type in high performance computing (HPC). 32-bit operating systems are not supported.

The C# model builder source code targets .NET Standard 2.0 and is Windows/Linux/MacOS portable. The C source is written for GNU GCC and Intel ICC compilers and can also be compiled for Windows/Linux, the MSVC compiler is not supported. The graphical user interface (GUI) uses WPF and DirectX 10/11 and is Windows only.

## How to compile?
The WPF GUI comes with a Visual Studio solution file and should be compiled with an appropriate IDE. The .NET Standard 2.0 libraries can also be compiled using the dotnet CLI command.

The C source code can be compiled with current GCC or ICC compilers. For a Win64 compilation it is required to have CMake for Windows installed and a proper toolchain system 