#!/usr/bin/env zsh
# Build submit script to submit the Mocassin compilation to the SLURM system
# First specified argument is

rm -f mocassinBuild.job

cat > mocassinBuild.job << endOfScript
#!/usr/bin/env zsh
module load python/3.6.0
module load cmake/3.13.2
#SBATCH --job-name=mocassin.build
#SBATCH --output=mocassin.build_%J.log
#SBATCH --time=0-01:00:00
#SBATCH --mem-per-cpu=2048
#SBATCH --ntasks 4

python3.6 $HOME/Mocassin/Simulator/Scripts/Builders/MocassinBuildScript.py $@
endOfScript
chmod 770 mocassinBuild.job
rehash
sbatch mocassinBuild.job