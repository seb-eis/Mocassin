import os as os
import re as re
import subprocess as subprocess
import threading as threading
import sys as sys
import glob as glob

class MocassinJobRunner:

    def __init__(self):
        self.ExecutableName = "Mocassin.Simulator"
        self.Executable = "./{0}".format(self.ExecutableName)

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
            return

        raise Exception("Could not locate simulation executable")

    @staticmethod
    def SplitFilenameIntoPathAndName(dbPath):
        regex = re.compile(r"(?P<pathName>.+)/(?P<fileName>[^/]+)")
        match = regex.match(dbPath)
        return match.group("pathName"), match.group("fileName")

    def MakeJobDirectory(self, basePath, id):
        jobDir = "{0:s}/Job{1:04d}".format(basePath, int(id))
        if not os.path.exists(jobDir):
            os.makedirs(jobDir)
        return jobDir

    def CallSimulator(self, args):
        self.FindExecutable()
        return subprocess.call(executable=self.Executable, args=args)

    def RunSimulatorAsThread(self, args):
        thread = threading.Thread(target=self.CallSimulator, args=(args,))
        thread.start()
        return thread

    def RunMultiple(self, sequence, dbPath):
        print("Start sequence is: {0}".format(sequence))
        threads = []
        split = self.SplitFilenameIntoPathAndName(dbPath)
        for i in sequence:
            executionPath = self.MakeJobDirectory(split[0], i)
            stdRedirect = "{0}/stdout.log".format(executionPath)
            args = [executionPath, "-dbPath", dbPath, "-dbQuery", str(i), "-stdRedirect", stdRedirect]
            print("Running: {}".format(args))
            threads.append(self.RunSimulatorAsThread(args))
        for thread in threads:
            thread.join()

runner = MocassinJobRunner()
runner.RunMultiple(sys.argv[1:], os.path.expandvars("$HOME/Mocassin/icc_test/ceria.msl"))