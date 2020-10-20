# Scripts - readme

## Basics

Scripts in this directory are intended for parallelization of jobs and collection of results on HPC backends. They generally create and use a job folder structure according to the following pattern and supports up to 99999 jobs per database:

```text
### General job folder structure for HPC startup and collection scripts

projectdir
|   project.msl
|───Job00001
|   |   run.mcs
|   |   prerun.mcs
|   |   sterr.log
|   |   stdout.log
|───Job00002
|   |   ...
|───Job00003
|   |   ...
...
|───Job99999
    ...
```

## Script: mocassin_collector

This python 3 script queries all job ids from an "msl" and traverses the job folder structure to collect all available ".mcs" and "stdout.log" files into the placeholders in the result table of the simulation database. The general startup syntax is shown below. If the [projectdir] is not provided, the parent directory of the [project.msl] is used. It is recommended to run this script once you have ensured that all jobs have successfully completed.

```shell
python3 mocassin_collector.py [project.msl] [projectdir]
```

## Script: mocassin_mt

This python 3 script serves as as startup wrapper for concurrent execution of the simulator on HPC backends. It automatically handles shared, distributed, or hybrid memory startup for a huge collection of jobs based on the currently set MPI rank and size information. To enable the use of MPI, first install MPI for python by running the following commands in a command shell:

```shell
python3 -m pip install --user --upgrade pip
python3 -m pip install --user mpi4py
```

Within the config file "mocassin_mt.cfg", the lookup path for the simulator binary files has to be set to the directory where "Mocassin.Simulator" is located on your host machine:

```shell
## Define the base path for automatic simulator lookup [auto causes $HOME or %USERPROFILE%] to be used
AutoSearchPath="[mocassinbin]"
```

The general script usage syntax requires the **absolute path** to the target simulation library [project.msl] and a job [sequence] that describes the jobs to be executed. The sequence supports comma separated values, ranges, and mixed versions of the former two.

```shell
python3 mocassin_mt.py db=[project.msl] jobs=[sequence]

## Start the jobs 1,2,3
python3 mocassin_mt.py db=[project.msl] jobs=1,2,3

## Start all jobs with ids from 1 to 10
python3 mocassin_mt.py db=[project.msl] jobs=1-10

## Start all jobs with ids from 1 to 10, job 15, and all from 20 to 30
python3 mocassin_mt.py db=[project.msl] jobs=1-10,15,20-30
```

For example, assuming that mocassin_mt.py was is configurated to use auto execution mode, was started as an MPI process with the job sequence "1-48" and 4 MPI ranks with 12 cores each, then each of the four MPI ranks will start 12 simulators to process the data assigned to it.

## Script: slurmsubmit

This python 3 script is a submit wrapper for the SLURM batch job system. It uses an XML script template and a provider class to control automatic job script generation and submit of jobs based on a single command line string. To use it for Mocassin, place the scripts mocassin_mt.py, mocassin_mt.cfg and mocsim_slurm.py into the same directory and configurate a copy of "jobtemplate_mocsim.xml" to your liking:

```xml
<SlurmTemplate Shell="zsh" DeleteScripts="True" DisableProviderOverwrites="False">
    <!-- Defines if an interpreter is required, the executable and the argument provider -->
    <Execution Interpreter="python3.6" Executable="./mocassin_mt.py" ArgumentProvider="mocsim_slurm:ArgumentProvider"/>
    <!-- Defines module loads that should be added to the job script -->
    <Modules>
        <Module Group="DEVELOP" Value="python/3.6.0"/>
    </Modules>
    <!-- Defines the batch job settings, settings format, and which tags describe the core and rank settings -->
    <BatchCookies Format="#SBATCH --{}={}" TaskTag="ntasks" CoreTag="cpus-per-task">
        <Cookie Tag="job-name" Value="moc_%J"/>
        <Cookie Tag="output" Value="moc_%J.log"/>
        <Cookie Tag="time" Value="24:00:00"/>
        <Cookie Tag="account" Value="myaccount"/>
        <Cookie Tag="mem-per-cpu" Value="3800"/>
        <Cookie Tag="ntasks" Value="2"/>
        <Cookie Tag="cpus-per-task" Value="4"/>
    </BatchCookies>
    <!-- The script template that is used to build the job script for SLURM -->
    <ScriptTemplate>
        $shell $nl $nl
        $cookies $nl
        $modules $nl
        $mpiexe $mpiflags $interpreter $executable $args
    </ScriptTemplate>
</SlurmTemplate>
```

The general script usage syntax for a SLURM submit is then as showed below. Note that the [sequence] also accepts "all" or "All" to instruct the provider to query all available job ids from the simulation database. Additionally, the provider scripts looks for completion tags in the stdout.log files of existing jobs folders and filters jobs that are already complete. 

```shell
python3 slurmsubmit.py [jobtemplate.xml] db=[project.msl] jobs=[sequence]
```