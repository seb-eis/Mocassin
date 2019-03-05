#!/usr/bin/env zsh
# Build submit script to submit the Mocassin compilation to the LSF system
# First specified argument is

rm -f mocassinBuild.lsf.job

cat > mocassinBuild.lsf.job << endOfScript
#BSUB -J mocassin.build
#BSUB -o $PWD/%J.%I.log
#BSUB -W 1:00
#BSUB -M 1024
#BSUB -n 4
python MocassinBuildScripty.py $@
endOfScript
bsub < mocassinBuild.lsf.job