import os as os
import sys as sys
import re as re
import xml.etree.ElementTree as xml
import uuid as uuid

class Dynamic:
    pass

class SlurmCookie:

    StringFormat="#SBATCH --{}={}"
    TaskTag = None
    CoreTag = None

    def __init__(self, tag, value):
        self.Tag = tag
        self.Value = value

    def __str__(self):
        if self.Value is None or self.Tag is None:
            return ""
        return SlurmCookie.StringFormat.format(self.Tag, self.Value)

class SlurmModule:

    StringFormat = "module load {} {}"

    def __init__(self, group, name):
        self.Group = group
        self.Name = name

    def __str__(self):
        if self.Group is None or self.Name is None:
            return ""
        return SlurmModule.StringFormat.format(self.Group, self.Name)

class SingleArgumentProvider:

    def ArgumentSet(self, slurmJob):
        yield slurmJob.ExeArguments

class SlurmJob:

    SubmitFormat = "sbatch {}"

    def __init__(self, templatePath=None):
        self.Shell = "zsh"
        self.BatchCookies = []
        self.Modules = []
        self.ScriptTemplate = ""
        self.Interpreter = ""
        self.Executable = ""
        self.ExeArguments = ""
        self.MpiExe = ""
        self.MpiFlags = ""
        self.TaskCount = 1
        self.CoreCountPerTask = 1
        self.ArgumentProvider = SingleArgumentProvider()
        self.DeleteScripts = True
        self.TestMode = False
        self.OverwritesDisabled = False
        if templatePath is not None:
            self.LoadTemplateFromXml(templatePath)

    def _LoadModulesFromXml(self, root):
        for item in root.find("Modules").findall("Module"):
            self.Modules.append(SlurmModule(item.get("Group"), item.get("Value")))

    def _LoadExecutionDataFromXml(self, root):
        element = root.find("Execution")
        self.Interpreter = element.get("Interpreter")
        self.Executable = element.get("Executable")
        argProvider = element.get("ArgumentProvider")
        if argProvider is not None:
            split = argProvider.split(':')
            module = __import__(os.path.expandvars(split[0]))
            self.ArgumentProvider = getattr(module, split[1])()

    def _LoadMiscFromXml(self, root):
        shell = root.get("Shell")
        autoDelete = root.get("DeleteScripts")
        testMode = root.get("TestMode")
        disableOverwrites = root.get("DisableProviderOverwrites")
        self.DeleteScripts = False if autoDelete == "False" else True
        self.Shell = shell if shell is not None else self.Shell
        self.TestMode = True if testMode == "True" else False
        self.OverwritesDisabled = True if disableOverwrites == "True" else False

    def _LoadScriptTemplateFromXml(self, root):
        self.ScriptTemplate = re.sub(r"\s{2,}", " ", root.find("ScriptTemplate").text).strip()

    def _LoadBatchCookiesFromXml(self, root):
        element = root.find("BatchCookies")
        children = element.findall("Cookie")
        self.BatchCookies = [SlurmCookie(x.get("Tag"), x.get("Value")) for x in children]
        SlurmCookie.TaskTag = element.get("TaskTag") if not None else SlurmCookie.TaskTag
        SlurmCookie.CoreTag = element.get("CoreTag") if not None else SlurmCookie.CoreTag

    def _DetectMpiAndCoreSettings(self):
        if SlurmCookie.TaskTag is None or SlurmCookie.CoreTag is None:
            raise Exception("")
        for cookie in self.BatchCookies:
            if cookie.Tag == SlurmCookie.TaskTag:
                self.TaskCount = int(cookie.Value)
                self.MpiExe = "$MPIEXEC" if self.TaskCount > 1 else ""
                self.MpiFlags = "$FLAGS_MPI_BATCH" if self.TaskCount > 1 else ""
            if cookie.Tag == SlurmCookie.CoreTag:
                self.CoreCountPerTask = int(cookie.Value) if int(cookie.Value) > 1 else 1

    def LoadTemplateFromXml(self, templatePath):
        xmlTree = xml.parse(templatePath)
        xmlRoot = xmlTree.getroot()
        self._LoadMiscFromXml(xmlRoot)
        self._LoadModulesFromXml(xmlRoot)
        self._LoadExecutionDataFromXml(xmlRoot)
        self._LoadScriptTemplateFromXml(xmlRoot)
        self._LoadBatchCookiesFromXml(xmlRoot)
        self._DetectMpiAndCoreSettings()

    def _SubShell(self, template):
        return re.sub(r"\$shell", "#!/usr/bin/env {}".format(self.Shell), template)

    def _SubCookies(self, template):
        cookieStr = "".join(["{} $nl".format(x) for x in self.BatchCookies])
        return re.sub(r"\$cookies", cookieStr, template)

    def _SubNewline(self, template):
        return re.sub(r"\s*\$nl\s*", "\n", template)

    def _SubModules(self, template):
        moduleStr = "".join(["{} $nl".format(x) for x in self.Modules])
        return re.sub(r"\$modules", moduleStr, template)

    def _SubExecution(self, template):
        template = template.strip()
        template = re.sub(r"\$mpiexe", self.MpiExe, template)
        template = re.sub(r"\$mpiflags", self.MpiFlags, template)
        template = re.sub(r"\$interpreter", self.Interpreter, template)
        template = re.sub(r"\$executable", self.Executable, template)
        return template

    def _SubExeArgs(self, template, args):
        return re.sub(r"\$args", args, template)

    def MakeScript(self, args):
        template = self.ScriptTemplate
        for func in [self._SubShell, self._SubCookies, self._SubModules, self._SubExecution, self._SubNewline]:
            template = func(template)
        return self._SubExeArgs(template, args)

    def GetJobScript(self, args):
        text = self.MakeScript(args)
        tmpName = "./{}.sh".format(uuid.uuid4())
        file = open(tmpName, "x")
        file.write(text)
        file.close()
        script = Dynamic()
        setattr(script, "File", tmpName)
        setattr(script, "Delete", lambda: os.remove(tmpName))
        return script

    def _SubmitInternal(self, script):
        os.system(SlurmJob.SubmitFormat.format(script.File))
        if self.DeleteScripts:
            script.Delete()

    def Submit(self):
        index = 0
        for args in self.ArgumentProvider.ArgumentSet(self):
            self._DetectMpiAndCoreSettings()
            script = self.GetJobScript(args)
            print("submit: [{}] [PROVIDER DATA]: {}".format(index, args))
            print("submit: [{}] [RANKS: {:3d} CORES: {:3d}]: {}".format(index, self.TaskCount, self.CoreCountPerTask, script.File), flush=True)
            if not self.TestMode:
                self._SubmitInternal(script)
            index = index+1
        print("submit: Completed!", flush=True)

    def OverwriteCookie(self, tag, value, sender=None):
        if tag is None or str(tag).isspace():
            raise Exception("Cannot overwrite a None or whitespace tag.")
        if self.OverwritesDisabled:
            print("{}: [OVERWRITE BLOCKED: {}]".format(sender if not None else "Unknown", tag), flush=True)
            return
        for cookie in self.BatchCookies:
            if cookie.Tag == tag:
                cookie.Value = value
                print("{}: [OVERWRITE: {}]".format(sender if not None else "Unknown", cookie), flush=True)
                return

    def TaskTag(self):
        return SlurmCookie.TaskTag

    def CoreTag(self):
        return SlurmCookie.CoreTag

    def ParseScriptAgruments(self, args):
        try:
            templatePath = args[1]
            print("submit: Parsing template: {}".format(templatePath), flush=True)
            self.LoadTemplateFromXml(templatePath)
            print("submit: Argument provider: {}".format(self.ArgumentProvider), flush=True)
            print("submit: Auto script deletion: [{}]".format(self.DeleteScripts))
            print("submit: Parallel default: [Ranks: {:3d} Cores: {:3d}]".format(self.TaskCount, self.CoreCountPerTask), flush=True)
        except Exception as e:
            raise Exception("Error while parsing template:\n {}".format(e))

        self.ExeArguments = " ".join(args[2:])


job = SlurmJob()
job.ParseScriptAgruments(sys.argv)
job.Submit()
