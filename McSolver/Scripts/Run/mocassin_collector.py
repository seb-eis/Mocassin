import os as os
import re as re
import glob as glob
import sqlite3 as sqlite3
import sys as sys

class JobResult:

    def __init__(self):
        self.JobId = 0
        self.Directory = ""
        self.RunStatePath = ""
        self.PreStatePath = ""
        self.StdoutLogPath = ""
        self.RunStateData = None
        self.PreStateData = None
        self.StdoutData = ""

    def LoadData(self):
        if self.StdoutLogPath is not None:
            file = open(self.StdoutLogPath, "r")
            self.StdoutData = file.read()
            file.close()

        if self.RunStatePath is not None:
            file = open(self.RunStatePath, "rb")
            self.RunStateData = file.read()
            file.close()

        if self.PreStatePath is not None:
            file = open(self.PreStatePath, "rb")
            self.PreStateData = file.read()
            file.close()

    def NullData(self):
        self.RunStateData = None
        self.PreStateData = None
        self.StdoutData = None

    def ToString(self):
        return "JOB_ID = {0}\nRUN_MCS = {1}\nPRE_MCS = {2}\nSTD_LOG = {3}\n"\
            .format(self.JobId, self.RunStatePath, self.PreStatePath, self.StdoutLogPath)

class ResultCollector:

    def __init__(self, dbPath, stateName = None, preStateName = None, stdoutName = None):
        self.DbConnection = None
        self.DbPath = os.path.expandvars(dbPath)
        self.JobDirRegex = re.compile(r"(?P<base>Job(?P<rawId>[0-9]+))")
        self.StateName = "run.mcs" if stateName is None else stateName
        self.PreStateName = "prerun.mcs" if preStateName is None else preStateName
        self.StdoutPattern = "stdout.log" if stdoutName is None else stdoutName
        self.TableInfo = \
            {   "TableName" : "JobResultData",
                "IdColumn": "JobModelId",
                "RunStateColumn": "ResultState",
                "PreStateColumn": "PreRunState",
                "StdoutColumn": "Stdout"}
        self.Sql = self.GetRawUpdateSql()

    def ConnectToDb(self):
        if self.DbConnection is not None:
            return
        self.DbConnection = sqlite3.connect(self.DbPath)

    def CommitChanges(self):
        if self.DbConnection is None:
            return
        self.DbConnection.commit()

    def CloseDb(self):
        self.DbConnection.commit()
        self.DbConnection.close()
        self.DbConnection = None

    def PrepareJobResult(self, jobDirectory):
        jobResult = JobResult()
        jobResult.Directory = jobDirectory
        rootName = os.path.basename(jobDirectory)
        jobResult.JobId = int(self.JobDirRegex.match(rootName).group("rawId"))

        runState = jobDirectory + "/" + self.StateName
        jobResult.RunStatePath = runState if os.path.exists(runState) else None

        preState = jobDirectory + "/" + self.PreStateName
        jobResult.PreStatePath = preState if os.path.exists(preState) else None

        stdoutLog = jobDirectory + "/" + self.StdoutPattern
        jobResult.StdoutLogPath = stdoutLog if os.path.exists(stdoutLog) else None

        return jobResult

    def PrepareJobResultsFromDirectory(self, baseDirectory):
        results = []
        for path in glob.glob("{0}/*".format(baseDirectory)):
            match = self.JobDirRegex.match(os.path.basename(path))
            if match is not None:
                local = self.PrepareJobResult(path)
                results.append(local)
        return results

    def GetRawUpdateSql(self):
        rawSql = "update {0} set {1}=?, {2}=?, {3}=? where {4}=?" \
            .format(self.TableInfo["TableName"],
                    self.TableInfo["RunStateColumn"],
                    self.TableInfo["PreStateColumn"],
                    self.TableInfo["StdoutColumn"],
                    self.TableInfo["IdColumn"])
        return rawSql

    def FormatUpdateArg(self, jobResult):
        args = (None if jobResult.RunStateData is None else sqlite3.Binary(jobResult.RunStateData),
                None if jobResult.PreStateData is None else sqlite3.Binary(jobResult.PreStateData),
                jobResult.StdoutData,
                jobResult.JobId)
        return args

    def FormatUpdateArgSet(self, jobResults):
        argList = []
        for item in jobResults:
            argList.append(self.FormatUpdateArg(item))
        return argList

    def WriteJobResultsToDatabase(self, jobResults):
        sys.stdout.write("Setting database to WAL.\n")
        self.DbConnection.execute(r"pragma journal_mode=wal")
        sys.stdout.write("Writing ({0}) found jobs ... _____".format(len(jobResults)))
        jobResults.sort(key=lambda job: job.JobId)
        for jobResult in jobResults:
            sys.stdout.write("\b\b\b\b\b{0:05d}".format(jobResult.JobId))
            sys.stdout.flush()
            jobResult.LoadData()
            self.DbConnection.execute(self.Sql, self.FormatUpdateArg(jobResult))
            jobResult.NullData()
        sys.stdout.write("\b\b\b\b\bDone!\n")
        sys.stdout.write("Setting database to DELETE.\n")
        self.DbConnection.execute(r"pragma journal_mode=delete")

    def CollectAllResults(self, jobBaseDir, deleteFiles = False):
        print("Collecting results for:")
        print("Database:\t{0}\nJobRoot:\t{1}".format(self.DbPath, jobBaseDir))
        collector.ConnectToDb()
        jobResults = self.PrepareJobResultsFromDirectory(jobBaseDir)
        self.WriteJobResultsToDatabase(jobResults)
        collector.CloseDb()

        if deleteFiles:
            for jobResult in jobResults:
                os.rmdir(jobResult.Directory)


if len(sys.argv) < 2:
    raise Exception("Invalid number of script arguments")

dbPath = sys.argv[1]
baseDir = sys.argv[2] if len(sys.argv) >= 3 else os.path.dirname(dbPath)
collector = ResultCollector(dbPath)
collector.CollectAllResults(baseDir)

