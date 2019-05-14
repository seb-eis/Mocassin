#!/usr/bin/env zsh
runScript="$HOME/Mocassin/Simulator/Scripts/Run/RunMocassin.py"
dbPath=$1
scriptName=".mocjob_seq.sh"

shift 1
jobCount=$#

rm -f $scriptName

cat > $scriptName  << _endOfJobScript
#!/usr/bin/env zsh
module load DEVELOP python/3.6.0
python3.6 $runScript $dbPath $@
_endOfJobScript

chmod 770 $scriptName
rehash
sbatch --time=0-00:15:00 --ntasks=1 --cpus-per-task=$jobCount --mem-per-cpu=3800M --output=mocjob_seq.log $scriptName