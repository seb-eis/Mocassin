#!/usr/bin/env zsh
module unload intel
module load intel/19.1
module unload gcc
module load gcc/10
buildScriptPath="$HOME/Mocassin/Simulator/Scripts/Builders/MocassinBuildScript.py"
python3 $buildScriptPath