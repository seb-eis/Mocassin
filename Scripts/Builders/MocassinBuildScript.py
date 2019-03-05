import os as os
import re as re
import sys as sys
import glob as glob
import subprocess as subprocess
import shutil as shutil

class MocassinBuilder:

    def __init__(self):
        self.Arguments = {}
        self.ArgRegex = re.compile(r"(?P<argName>.+)=\"(?P<argValue>.*)\"")
        self.ConfigFileName="Mocassin.config"
        self.ExeName="Mocassin.Simulator"

    def GetOsHomeDirectory(self):
        if os.name == "nt":
            return "%USERPROFILE%"
        if os.name == "posix":
            return "$HOME"

    def SearchConfigFile(self, startDir):
        print("Trying to find the config file recursively: Warning, this may take a while, abort with STRG+C")

        if startDir == "" or not os.path.exists(startDir):
            start = self.GetOsHomeDirectory()
        else:
            start = startDir

        searchPath = os.path.expandvars('{0}/**/{1}'.format(start, self.ConfigFileName))
        print("Searching for: {0}".format(searchPath))
        for filename in glob.iglob(searchPath, recursive=True):
            print("First match at: {0}".format(filename))
            return filename

        raise Exception("Failed to locate the config file...")

    def SearchSourceDirectory(self, startDir):
        print("Trying to find the source file recursively: Warning, this may take a while, abort with STRG+C")
        regex = re.compile(r"(?P<pathName>.+){0}".format("CMakeLists.txt"))

        if startDir == "" or not os.path.exists(startDir):
            start = self.GetOsHomeDirectory()
        else:
            start = startDir

        searchPath = os.path.expandvars('{0}/**/{1}'.format(start, "CMakeLists.txt"))
        print("Searching for: {0}".format(searchPath))
        for filename in glob.iglob(searchPath, recursive=True):
            match = regex.match(filename)
            if match and self.CheckCMakeListsFile(filename):
                print("First match which seems to contain Mocassin content at: {0}".format(filename))
                return match.group("pathName")

        raise Exception("Failed to locate the source file directory...")

    def ReadConfigFromText(self, configText):
        self.Arguments["cfgFile"] = ""
        for value in self.ArgRegex.finditer(configText):
            self.Arguments[value.group("argName")] = os.path.expandvars(value.group("argValue"))

    def ReadConfigFromFile(self, filePath):
        try:
            if not os.path.isfile(filePath):
                print("Config file is invalid or not specified")
                filePath = self.SearchConfigFile(self.GetOsHomeDirectory())

            fhandle = open(filePath, 'r')
            configText = fhandle.read()
            fhandle.close()

        except Exception as e:
            raise Exception("Could not open or find config file! Inner Exception: {1}".format(filePath, e))

        self.ReadConfigFromText(configText)
        print("The following argument collection was read for the config file:")
        print(self.Arguments)

    def MakeMissingDirectories(self):
        regex = re.compile(r"(.+Directory)")
        for value in self.Arguments:
            if regex.match(value):
                path = os.path.expandvars(self.Arguments[value])
                if not os.path.exists(path):
                    os.makedirs(path)
                    print("Missing path: {0} has been created".format(path))

    def CheckCMakeListsFile(self, filePath):
        if not os.path.isfile(filePath):
            return False

        fhandle = open(filePath, 'r')
        text = fhandle.read()
        fhandle.close()
        regex = re.compile("Mocassin.Simulator")
        if len(regex.findall(text)) != 0:
            return True
        return False

    def ValidateSourceDirectory(self):
        cmakelist = "{0}/CMakeLists.txt".format(self.Arguments["sourceDirectory"])
        if self.CheckCMakeListsFile(cmakelist):
            print("The CMakeList file is: {0}".format(cmakelist))
            return True

        print("Source directory is invalid or not specified")
        self.Arguments["sourceDirectory"] = self.SearchSourceDirectory("")
        cmakelist = "{0}/CMakeLists.txt".format(self.Arguments["sourceDirectory"])
        if self.CheckCMakeListsFile(cmakelist):
            print("The CMakeList file is: {0}".format(cmakelist))
            return True

        raise Exception("The CMakeList file does not or could not be found!")

    def ValidateCleanRebuild(self):
        if self.Arguments["cleanBuild"] == "True":
            print("Cleaning build directory for clean rebuild")
            shutil.rmtree(self.Arguments["buildDirectory"])
            os.mkdir(self.Arguments["buildDirectory"])

    def ExecuteCMake(self):
        self.ValidateCleanRebuild()
        print("Changing to build directory: {0}".format(self.Arguments["buildDirectory"]))
        os.chdir(self.Arguments["buildDirectory"])

        cfgCommand = "CC={0} cmake -DCMAKE_BUILD_TYPE={1} {2}".format(self.Arguments["compilerName"], self.Arguments["buildType"], self.Arguments["sourceDirectory"])
        print("Invoking configuration command: {0}".format(cfgCommand))
        p = subprocess.Popen(cfgCommand, shell=True)
        p.wait()

        print("Building...")
        p = subprocess.Popen("make", shell=True)
        p.wait()
        print("Completed build!")

    def GetLibraryFileExtension(self):
        if os.name == "nt":
            return ".dll"
        if os.name == "posix":
            return ".so"
        raise Exception("OS is not supported")

    def CopyCompiledObjectsToDeploy(self):
        targetDir = self.Arguments["deployDirectory"]
        buildDir = self.Arguments["buildDirectory"]
        libExtension = self.GetLibraryFileExtension()
        print("Copying library data to deploy location: {0}".format(targetDir))
        for filename in glob.iglob("{0}/*{1}".format(buildDir, libExtension), recursive=True):
            print("Copying: {0}".format(filename))
            shutil.copy(filename, targetDir)

        exeName = "{0}/{1}".format(buildDir, self.ExeName)
        if os.path.isfile(exeName):
            print("Copying : {0}".format(exeName))
            shutil.copy(exeName, targetDir)
            return

        exeName += ".exe"
        if os.path.isfile(exeName):
            print("Copying : {0}".format(exeName))
            shutil.copy(exeName, targetDir)
            return

        raise Exception("The compiled executable was not found, has an invalid name or file extension!")



    def Run(self, cfgFile):
        self.ReadConfigFromFile(cfgFile)
        self.ValidateSourceDirectory()
        self.MakeMissingDirectories()
        self.ExecuteCMake()
        self.CopyCompiledObjectsToDeploy()


builder = MocassinBuilder()
cfgFile = ""
if len(sys.argv) == 2:
    cfgFile = sys.argv[1]
builder.Run(cfgFile)
exit()
