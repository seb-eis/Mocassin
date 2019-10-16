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
import sys as sys
import glob as glob
import time as time
from mpi4py import MPI

class MocassinJobRunner:

    def __init__(self):
        self.MpiRank = MPI.COMM_WORLD.Get_rank()
        self.MpiSize = MPI.COMM_WORLD.Get_size()
        self.MpiComm = MPI.COMM_WORLD
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
        for value in glob.iglob(searchPath, recursive=True):
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

    def AwaitExecutionFinish(self):
        if self.MpiRank != 0:
            send = self.MpiComm.isend(True, dest=0, tag=self.MpiRank)
            send.wait()
            return
        for rank in range(1, self.MpiSize):
            msg = self.MpiComm.irecv(source=rank, tag=rank)
            msg.wait()
            print("{} - Received OK @ MPI_Rank (0) from MPI_Rank ({} / {})".format(time.asctime(), rank, self.MpiSize), flush=True)
        print("{} - MPI-Rank 0 is finished".format(time.asctime()), flush=True)

    def WaitForAllRanks(self, root):
        if self.MpiRank == root:
            for rank in range(0, self.MpiSize):
                if rank == root:
                    continue
                self.MpiComm.isend(True, dest=rank, tag=rank).wait()
        else:
            self.MpiComm.irecv(source=root, tag=self.MpiRank).wait()

    def PrepareSimulatorArguments(self, sequence):
        dbPathSplit = self.SplitFilenameIntoPathAndName(self.DbPath)
        executionPath = self.MakeJobDirectory(dbPathSplit[0], self.MpiRank + 1)
        stdRedirect = "stdout.log"
        jobId = sequence[self.MpiRank]
        args = [executionPath, "-dbPath", self.DbPath, "-jobId", str(jobId), "-ioPath", executionPath, "-stdout", stdRedirect, "-fexp", "false", "-extDir", self.ExtensionPath]
        return args

    def RunAndAwaitMocassinProcess(self, root, sequence):
        if self.MpiRank == root:
            print("Started with {} MPI ranks (Root = {}).".format(self.MpiSize, root))
            print("Sequence is: {0}".format(sequence), flush=True)
        self.WaitForAllRanks(root)
        args = self.PrepareSimulatorArguments(sequence)
        print("{} - Start @ MPI_Rank ({}): {}".format(time.asctime(), self.MpiRank, args), flush=True)
        self.StartSimulator(args).wait()
        self.AwaitExecutionFinish()

    def SetDatabasePath(self, path):
        dbPath = os.path.expandvars(path)
        if not os.path.exists(dbPath):
            raise Exception("Db path does not point to a file")
        self.DbPath = dbPath


runner = MocassinJobRunner()
runner.SetDatabasePath(sys.argv[1])
runner.FindExecutable()
runner.RunAndAwaitMocassinProcess(0, sys.argv[2:])