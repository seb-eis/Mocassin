#!/usr/bin/env zsh
runScript="$HOME/Mocassin/Simulator/Scripts/Run/RunMocassin.py"
dbPath=$1
scriptName=".mocjob.sh"
jobCount=0
shift 1
jobSequence=$(sqlite3 $dbPath "select Id from JobModels")
for i in $jobSequence; do
    jobCount=$(($jobCount+1))
    done


rm -f $scriptName

cat > $scriptName  << _endOfJobScript
#!/usr/bin/env zsh
module load DEVELOP python/3.6.0
python3.6 $runScript $dbPath $(echo $jobSequence)
_endOfJobScript

chmod 770 $scriptName
rehash
echo Jobcount: \\t $jobCount
echo JobIds: \\t $jobSequence
sbatch --time=0-01:00:00 --ntasks=$jobCount --mem-per-cpu=1024M --output=mocjob.log $scriptName