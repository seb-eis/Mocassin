#!/usr/bin/env zsh
runScript="$HOME/Mocassin/Simulator/Scripts/Run/mocassin_mt.py"
dbPath=$1
mpiRanks=$2
jobsPerRank=$3
pkSize=$((mpiRanks * jobsPerRank))
rmScripts="True"
scriptName="sm_mocjob"
shift 3

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
timeLimit="0-24:00:00"
memPerCpu="3800M"
localScriptName=${scriptName}.$1-${acEnd}.sh
localOutputFile=${scriptName}.$1-${acEnd}.log
jobSequence=$(sqlite3 $dbPath "select Id from JobModels where Id >= $1 and Id < $2")
localJobCount=$(CountJobs $jobSequence)
tmp=$(echo $jobSequence)
jobString=${tmp// /,}

if [ "$localJobCount" = "0" ]; then
    echo "Finished!"
    exit 0
fi

rm -f $localScriptName

cat > $localScriptName  << _endOfJobScript
#!/usr/bin/env zsh
module load DEVELOP python/3.6.0
if [ "$mpiRanks" = "1" ]; then
	python3.6 $runScript "db=$dbPath" "jobs=$jobString"
else
	mpiexec -n $mpiRanks python3.6 $runScript "db=$dbPath" "jobs=$jobString"
fi
_endOfJobScript

chmod 777 $localScriptName
rehash
if [ "$mpiRanks" = "1" ]; then
	echo "Sending package with $localJobCount entries as shared memory job [Cores=$localJobCount]($1 - $acEnd, $localScriptName)!"
	sbatch --time=$timeLimit --ntasks=1 --cpus-per-task=$localJobCount --mem-per-cpu=$memPerCpu --output=$localOutputFile $localScriptName
else
	echo "Sending package with $localJobCount entries as MPI/Hybrid job [MPI_RANKS=$mpiRanks, CoresPerRank=$jobsPerRank]($1 - $acEnd, $localScriptName)!"
	sbatch --time=$timeLimit --ntasks=$mpiRanks --cpus-per-task=$jobsPerRank --mem-per-cpu=$memPerCpu --output=$localOutputFile $localScriptName
fi

if [ "$rmScripts" = "True" ]; then
	rm -f $localScriptName
fi
}

for ((i=1;i>0;i=i+pkSize)); do
	SendPackage $i $((i+pkSize))
done
