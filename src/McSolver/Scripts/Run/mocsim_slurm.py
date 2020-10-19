import re
import os
import sqlite3
from pathlib import Path

class ArgumentProvider:

    def ArgumentSet(self, slurmJob):
        db = self.GetDatabase(slurmJob.ExeArguments)
        jobs = self.GetJobIndices(slurmJob.ExeArguments)
        jobs = self.FilterJobIndicesByCompletionTag(jobs, db)
        groups = self.GroupByTaskAndCores(jobs, self.TargetJobCountPerScript(slurmJob))
        for jobIds in groups.values():
            self.OverwriteMpi(slurmJob, jobIds)
            value = "{} {}".format(self.GetDbArgument(db), self.GetJobsArgument(jobIds))
            yield value

    def GetJobsArgument(self, jobIds):
        return "jobs=" + ",".join([str(x) for x in jobIds])

    def GetDbArgument(self, db):
        return "db={}".format(db)

    def TargetJobCountPerScript(self, slurmJob):
        return slurmJob.CoreCountPerTask * slurmJob.TaskCount

    def GetDatabase(self, argString):
        dbStr = self.FindParameterValue("db", argString)
        return dbStr

    def FilterJobIndicesByCompletionTag(self, jobIds, dbPath):
        submitList = list()
        filtered = list()
        basePath = str(Path(dbPath).parent)
        for jobId in jobIds:
            logPath = "{0:s}/Job{1:05d}/stdout.log".format(basePath, jobId)
            if not os.path.exists(logPath):
                submitList.append(jobId)
                continue
            with open(logPath) as logFile:
                if "ABORT_REASON" not in logFile.read():
                    submitList.append(jobId)
                else:
                    filtered.append(jobId)
        if len(filtered) is not 0:
            print("submit: Found incomplete jobs [{}]".format(self.CompressJobIndexSequenceToString(submitList)), flush=True)
            print("submit: Found completed jobs  [{}]".format(self.CompressJobIndexSequenceToString(filtered)), flush=True)     
        return submitList

    def CompressJobIndexSequenceToString(self, jobIds):
        if len(jobIds) is 0:
            return
        if len(jobIds) is 1:
            return str(jobIds[0])
        result = ""
        firstId = jobIds[0]
        secondId = jobIds[0]
        for id in jobIds[1:]:
            if id is (secondId + 1):
                secondId = id
                if secondId is not jobIds[-1]:
                    continue
            if firstId is secondId:
                    result = result + ("," if result is not "" else "") + str(firstId)
            else:
                result = result + ("," if result is not "" else "") + str(firstId) + "-" + str(secondId)
            firstId = secondId = id

        return result

    def GetJobIndices(self, argString):
        valueStr = self.FindParameterValue("jobs", argString)
        if re.match(r"[Aa]ll", valueStr):
            with sqlite3.connect(self.GetDatabase(argString)) as db:
                values = db.cursor().execute("select Id from JobModels order by Id").fetchall()
                return [int(x[0]) for x in values]
        return self.ParseJobIdString(valueStr)

    def FindParameterValue(self, name, string):
        pattern = r"{}\s*=\s*\"?(?P<val>[^\"\s]+)\"?".format(name)
        return re.search(pattern, string).group("val")

    def ParseJobIdString(self, string):
        singleRegex = re.compile(r"(?<![0-9\-])([0-9]+)(?![0-9\-])")
        rangeRegex = re.compile(r"([0-9]+)-([0-9]+)")
        values = []
        for match in singleRegex.finditer(string):
            values.append(int(match.group(1)))
        for match in rangeRegex.finditer(string):
            minValue = min(int(match.group(1)), int(match.group(2)))
            maxValue = max(int(match.group(1)), int(match.group(2))) + 1
            values.extend([i for i in range(minValue, maxValue)])
        return list(set(values))

    def GroupByTaskAndCores(self, jobIds, packSize):
        packSizeInt = int(packSize+1) if int(packSize) < packSize else int(packSize)
        jobGroups = {}
        for startId in range(0, len(jobIds), packSizeInt):
            jobSet = []
            for i in range(startId, startId + packSizeInt):
                jobSet.append(jobIds[i])
                if i+1 == len(jobIds):
                    break
            jobGroups[len(jobGroups)] = jobSet
        return jobGroups

    def OverwriteMpi(self, slurmJob, jobGroup):
        if len(jobGroup) < self.TargetJobCountPerScript(slurmJob):
            slurmJob.OverwriteCookie(slurmJob.TaskTag(), len(jobGroup), self)
            slurmJob.OverwriteCookie(slurmJob.CoreTag(), 1, self)
