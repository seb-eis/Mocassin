#!/usr/bin/env zsh
runScript="$HOME/Mocassin/Simulator/Scripts/Run/RunMocassin.py"
scriptName=".mocassin_job"
jobCount=$#

rm -f $scriptName

cat > $scriptName  << _endOfJobScript
#!/usr/bin/env zsh
module load DEVELOP python/3.6.0
#SBATCH --output=moc_%J.log
#SBATCH --job-name=moc_%J
#SBATCH --time=24:00:00
#SBATCH --ntasks $jobCount
#SBATCH --mem-per-cpu=1024M
python3.6 $runScript $@
_endOfJobScript
chmod 770 $scriptName
rehash
sbatch $scriptName