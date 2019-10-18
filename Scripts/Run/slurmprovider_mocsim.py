import re as re
import os as os
import sqlite3

class Provider:

    def ArgumentSet(self, slurmJob):
        db = self.GetDatabase(slurmJob.ExeArguments)
        jobs = self.GetJobIndices(slurmJob.ExeArguments)
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

    def GetJobIndices(self, argString):
        valueStr = self.FindParameterValue("jobs", argString)
        if re.match(r"[Aa]ll", valueStr):
            db = sqlite3.connect(self.GetDatabase(argString))
            values = db.cursor().execute("select Id from JobModels").fetchall()
            db.close()
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
