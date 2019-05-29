#!/usr/bin/env zsh
runScript="$HOME/Mocassin/Simulator/Scripts/Run/RunMocassin.py"
dbPath=$1
pkSize=$2
rmScripts="True"
scriptName="sm_mocjob"
shift 2

### arg1 = jobSequence
CountJobs()
{
jobCount=0
for i in $jobSequence; do
    jobCount=$((jobCount+1));
done
echo $jobCount
}

### arg1 = startId, arg2 = endId
SendPackage()
{
acEnd=$(($2-1))
timeLimit="0-00:15:00"
memPerCpu="3800M"
localScriptName=${scriptName}.$1-${acEnd}.sh
localOutputFile=${scriptName}.$1-${acEnd}.log
jobSequence=$(sqlite3 $dbPath "select Id from JobModels where Id >= $1 and Id < $2")
localJobCount=$(CountJobs $jobSequence)

if [ "$localJobCount" = "0" ]; then
    echo "Finished!"
    exit 0
fi

echo "Sending package with $localJobCount entries as shared memory job ($1 - $acEnd, $localScriptName)!"

rm -f $localScriptName

cat > $localScriptName  << _endOfJobScript
#!/usr/bin/env zsh
module load DEVELOP python/3.6.0
python3.6 $runScript $dbPath $(echo $jobSequence)
_endOfJobScript

chmod 777 $localScriptName
rehash
sbatch --time=$timeLimit --ntasks=1 --nodes=1 --cpus-per-task=$localJobCount --mem-per-cpu=$memPerCpu --output=$localOutputFile $localScriptName

if [ "$rmScripts" = "True" ]; then
	rm -f $localScriptName
fi
}

for ((i=1;i>0;i=i+pkSize)); do
	SendPackage $i $((i+pkSize))
done

