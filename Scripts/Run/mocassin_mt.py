import os as os
import glob as glob
import re as re
import subprocess as subprocess
import time as time
import sys as sys
try:
    from mpi4py import MPI
except:
    print("MPI import failure, mocking .")


class MpiCommDummy:

    def __init__(self, rank=0, size=0):
        self.Rank = rank
        self.Size = size

    def Get_rank(self):
        return self.Rank

    def Get_size(self):
        return self.Size


class MocsimExecutionController:

    def __init__(self, config=None, mpiComm=None):
        self.Parameters = self.GetDefaultParameters()
        config = config if config is not None else self.AutoSelectConfigFile()
        if config is not None:
            self.ParseAndLoadParameters(config)
        self.TranslateAutoSettings()
        self.MpiComm = mpiComm

    def mpicomm(self):
        if self.MpiComm is None:
            try:
                self.MpiComm = MPI.COMM_WORLD
            except:
                self.MpiComm = MpiCommDummy()

        return self.MpiComm

    def mpirank(self):
        return self.mpicomm().Get_rank()

    def mpisize(self):
        return self.mpicomm().Get_size()

    def GetDefaultParameters(self):
        return {"ExecutablePath": "auto",
                "ExtensionPath": "auto",
                "JobFolderFormat": "{0:s}/Job{1:05d}",
                "SimulationStdoutLog": "stdout.log",
                "ExponentialMode": "exact",
                "ExecutionMode": "auto",
                "AutoSearchPath": "auto",
                "EnableTestMode": "True",
                "CmdArgsValueDelimiters": "None"}

    def GetExecutionModeDelegates(self):
        return {"sequential": self.ExecuteJobsSequential,
                "mpi": self.ExecuteJobsMPI,
                "shared": self.ExecuteJobsShared,
                "hybrid": self.ExecuteJobsHybrid,
                "auto": self.ExecuteJobsAuto}

    def ParseAndLoadParameters(self, config, valDel="\"\""):
        configStr = None
        if os.path.exists(config):
            stream = open(config)
            configStr = stream.read()
            stream.close()
        else:
            configStr = config

        pattern = r"(?P<var>[^=\s]+)\s*=\s*\{}(?P<val>[^\{}]+)\{}"
        if valDel == "None" or valDel == "":
            pattern = r"(?P<var>[^=\s]+)\s*=\s*(?P<val>[^\s]+)"
        else:
            pattern = pattern.format(valDel[0], valDel[1], valDel[1])
        regex = re.compile(pattern)
        for match in regex.finditer(configStr):
            self.Parameters[match.group("var")] = match.group("val")

    def GetSimulatorSearchPattern(self):
        exeName = "Mocassin.Simulator"
        if os.name == "nt":
            exeName = exeName + ".exe"
        return "{0:s}/**/{1:s}".format(os.path.expandvars(self.Parameters["AutoSearchPath"]), exeName)

    def FindExecutable(self):
        searchPattern = self.GetSimulatorSearchPattern()
        for item in glob.iglob(searchPattern, recursive=True):
            self.Parameters["ExecutablePath"] = item

    def FindExtensionPath(self):
        self.Parameters["ExtensionPath"] = os.path.dirname(self.Parameters["ExecutablePath"])

    def TranslateAutoSettings(self):
        if self.Parameters["AutoSearchPath"] == "auto":
            self.Parameters["AutoSearchPath"] = "$HOME" if os.name == "posix" else "%USERPROFILE%"
        if self.Parameters["ExecutablePath"] == "auto":
            self.FindExecutable()
        if self.Parameters["ExtensionPath"] == "auto":
            self.FindExtensionPath()

    def EnsureJobFolderCreated(self, jobId):
        basePath = os.path.dirname(self.Parameters["db"])
        jobDir = "{0:s}/Job{1:05d}".format(basePath, int(jobId))
        if not os.path.exists(jobDir):
            os.makedirs(jobDir)
        return jobDir

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

    def LoadDatabaseAndJobsFromArguments(self, args):
        args = [args] if isinstance(args, str) else args
        for item in args:
            self.ParseAndLoadParameters(item, self.Parameters["CmdArgsValueDelimiters"])
        jobIds = self.ParseJobIdString(self.Parameters["jobs"])
        if len(jobIds) == 0:
            raise Exception("No job ids defined.")
        self.Parameters["jobs"] = jobIds
        if not os.path.exists(self.Parameters["db"]):
            raise Exception("The provided database [{}] does not exist.".format(self.Parameters["db"]))

    def PrepareJobExecution(self, jobId):
        executionPath = os.path.dirname(self.Parameters["db"])
        jobFolder = self.EnsureJobFolderCreated(jobId)
        if self.Parameters["EnableTestMode"] == "True":
            return [executionPath, "--bcall"]

        return [executionPath,
                "-dbPath", self.Parameters["db"],
                "-jobId", str(jobId),
                "-ioPath", jobFolder,
                "-stdout", self.Parameters["SimulationStdoutLog"],
                "-fexp", "true" if self.Parameters["ExponentialMode"] == "fast" else "false",
                "-extDir", self.Parameters["ExtensionPath"]]

    def PopenSimulator(self, jobId, wait=False):
        args = self.PrepareJobExecution(jobId)
        process = subprocess.Popen(executable=self.Parameters["ExecutablePath"], args=args)
        if wait:
            process.wait()
        return process

    def GroupJobsForHybridExecution(self, jobIds):
        packSize = len(jobIds) / self.mpisize()
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

    def ExecuteJobsHybrid(self, jobIds = None):
        jobIds = self.Parameters["jobs"] if jobIds is None else jobIds
        jobGroup = self.GroupJobsForHybridExecution(jobIds)[self.mpirank()]
        print("[{}] [MPI_RANK {} / {}] - Execution group [Count={}] = {}".format(time.asctime(), self.mpirank(), self.mpisize(), len(jobGroup), jobGroup), flush=True)
        processes = [self.PopenSimulator(jobId) for jobId in jobGroup]
        for process in processes:
            print("[{}] [MPI_RANK {} / {}] - Simulation start: {}".format(time.asctime(), self.mpirank(), self.mpisize(), process.args), flush=True)
        returnCodes = [process.wait() for process in processes]
        print("[{}] [MPI_RANK {} / {}] - Simulations completed: {}".format(time.asctime(), self.mpirank(), self.mpisize(), returnCodes), flush=True)

    def ExecuteJobsSequential(self, jobIds = None):
        jobIds = self.Parameters["jobs"] if jobIds is None else jobIds
        for jobId in jobIds:
            self.ExecuteJobsShared([jobId])

    def ExecuteJobsMPI(self, jobIds = None):
        jobIds = self.Parameters["jobs"] if jobIds is None else jobIds
        if len(jobIds) > self.mpisize():
            raise Exception("The number of jobs exceeds the number of available MPI ranks.")

        jobId = jobIds[self.mpirank()]
        process = self.PopenSimulator(jobId)
        print("[{}] [MPI_RANK {} / {}] - Simulation started: {}".format(time.asctime(), self.mpirank(), self.mpisize(), process.args), flush=True)
        process.wait()
        print("[{}] [MPI_RANK {} / {}] - Simulator completed: [{}]".format(time.asctime(), self.mpirank(), self.mpisize(), process.returncode), flush=True)

    def ExecuteJobsShared(self, jobIds = None):
        jobIds = self.Parameters["jobs"] if jobIds is None else jobIds
        processes = [self.PopenSimulator(jobId) for jobId in jobIds]
        for process in processes:
            print("[{}] - Simulation start: {}".format(time.asctime(), process.args), flush=True)
        returnCodes = [process.wait() for process in processes]
        print("[{}] - Simulation completed: {}".format(time.asctime(), returnCodes), flush=True)

    def ExecuteJobsAuto(self, jobIds = None):
        jobIds = self.Parameters["jobs"] if jobIds is None else jobIds
        jobCount = len(jobIds)

        if jobCount == 1:
            return self.ExecuteJobsSequential(jobIds)

        if os.name == "nt":
            return self.ExecuteJobsShared(jobIds)

        if jobCount == self.mpisize():
            return self.ExecuteJobsMPI(jobIds)

        if self.mpisize() == 0:
            return self.ExecuteJobsShared(jobIds)

        if jobCount > self.mpisize():
            return self.ExecuteJobsHybrid(jobIds)

        raise Exception("Auto execution did not enter a proper execution mode.")

    def ExecuteJobs(self):
        mode = self.Parameters["ExecutionMode"]
        delegates = self.GetExecutionModeDelegates()
        func = delegates.get(mode, self.ExecuteJobsSequential)
        func()

    @staticmethod
    def AutoSelectConfigFile():
        cfgPath = "./mocassin_mt.cfg"
        if os.path.exists(cfgPath):
            return cfgPath
        cfgPath = "{}/mocassin_mt.cfg".format(os.path.dirname(sys.argv[0]))
        if os.path.exists(cfgPath):
            return cfgPath
        return None


controller = MocsimExecutionController()
if controller.mpirank() == 0:
    print("CMD: {}".format(sys.argv), flush=True)
controller.LoadDatabaseAndJobsFromArguments(sys.argv[1:])
controller.ExecuteJobs()
