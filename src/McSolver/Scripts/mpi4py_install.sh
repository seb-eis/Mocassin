#!/usr/bin/env zsh
module unload gcc
module unload python
module load gcc/9
module load python/3.6.0
python3.6 -m pip install --user --upgrade pip
python3.6 -m pip install --user mpi4py