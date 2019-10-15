# Shared memory execution wrapper for MOCASSIN simulator with recursive binary lookup
# Requires full database path as first argument
# Further arguments are treated as to be executed job context ids
# Example usage : python3.6 RunMocassin.py $HOME/Test/example.moc 1 2 3 4
# will start a shared memory process with 4 threads executing job context ids 1 2 3 and 4 from the source database
# IMPORTANT:    -Does currently not work correctly with relative database paths and the windows operating system
#               -Parallel execution with shared memory has a scaling limit of cores per computer/node
#               -This is not an MPI wrapper, all cores have to belong to the same physical computer (node)
#               -Mixing short and long jobs is not recommended as the interpreter awaits all threads before returning

import os as os
import re as re
import subprocess as subprocess
import threading as threading
import sys as sys
import glob as glob
import time as time
import multiprocessing as multiprocessing

class MocassinJobRunner:

    def __init__(self):
        self.ExecutableName = "Mocassin.Simulator"
        self.DbPath = ""
        self.Executable = "./{0}".format(self.ExecutableName)
        self.ExtensionPath = "."

    def GetBaseSearchPath(self):
        if os.name == "nt":
            return os.path.expandvars("%USERPROFILE%") + "/**/" + self.ExecutableName
        if os.name == "posix":
            return os.path.expandvars("$HOME") + "/**/" + self.ExecutableName

        raise Exception("OS is not supported")

    def FindExecutable(self):
        if os.path.exists(self.Executable):
            return

        searchPath = self.GetBaseSearchPath()
        print("Default executable {0} does not exist, searching for {1} ...".format(self.Executable, searchPath))

        for value in glob.iglob(searchPath, recursive=True):
            print("First match at: {0}".format(value))
            self.Executable = value
            self.ExtensionPath = self.SplitFilenameIntoPathAndName(value)[0]
            return

        raise Exception("Could not locate simulation executable")

    @staticmethod
    def SplitFilenameIntoPathAndName(dbPath):
        regex = re.compile(r"(?P<pathName>.+)/(?P<fileName>[^/]+)")
        match = regex.match(dbPath)
        return match.group("pathName"), match.group("fileName")

    def MakeJobDirectory(self, basePath, id):
        jobDir = "{0:s}/Job{1:05d}".format(basePath, int(id))
        if not os.path.exists(jobDir):
            os.makedirs(jobDir)
        return jobDir

    def CallSimulator(self, args):
        return subprocess.call(executable=self.Executable, args=args)

    def StartSimulator(self, args):
        return subprocess.Popen(executable=self.Executable, args=args)

    def RunAndAwaitMocassinProcesses(self, sequence):
        cpuCount = multiprocessing.cpu_count()
        print("System has {0} cores available.".format(cpuCount))
        print("Start sequence is: {0}".format(sequence))
        threads = []
        dbPathSplit = self.SplitFilenameIntoPathAndName(self.DbPath)
        processList = []
        for i in sequence:
            executionPath = self.MakeJobDirectory(dbPathSplit[0], i)
            stdRedirect = "stdout.log"
            args = [executionPath, "-dbPath", self.DbPath, "-jobId", str(i), "-ioPath", executionPath, "-stdout", stdRedirect, "-fexp", "false", "-extDir", self.ExtensionPath]
            print("Running: {}".format(args), flush=True)
            processList.append(self.StartSimulator(args))
        print("All Started at: {}".format(time.asctime()), flush=True)
        for process in processList:
            process.wait()
        print("All joined at: {}".format(time.asctime()), flush=True)

    def SetDatabasePath(self, path):
        dbPath = os.path.expandvars(path)
        if not os.path.exists(dbPath):
            raise Exception("Db path does not point to a file")
        self.DbPath = dbPath

runner = MocassinJobRunner()
runner.SetDatabasePath(sys.argv[1])
runner.FindExecutable()
runner.RunAndAwaitMocassinProcesses(sys.argv[2:])