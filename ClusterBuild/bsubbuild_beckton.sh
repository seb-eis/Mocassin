#!/usr/bin/env zsh
rm -f build_job
cpu_model="Beckton"
cat > build_job <<end_B
### Job name
#BSUB -J $cpu_model.build

### File / path where STDOUT & STDERR will be written
###    %J is the job ID, %I is the array ID
#BSUB -o $PWD/Logs/$cpu_model.%J.%I

### Request the time you need for execution in minutes
### The format for the parameter is: [hour:]minute,
### that means for 80 minutes you could also use this: 1:20
#BSUB -W 1:00

#BSUB -R "select[model==$cpu_model]"

### Request memory you need for your job in TOTAL in MB
#BSUB -M 2048

### Change to the work directory
cd $HOME/Mocassin/Simulator/ClusterBuild/Logs

### Execute your application
$HOME/Mocassin/Simulator/clusterbuild.sh $cpu_model $@
end_B
bsub < build_job