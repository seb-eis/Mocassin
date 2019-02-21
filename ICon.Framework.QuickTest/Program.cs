using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Mocassin.Framework.Xml;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.EntityBuilder;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Model.Translator.Optimization;
using Mocassin.UI.Xml.CreationData;
using Mocassin.UI.Xml.CustomizationData;
using Mocassin.UI.Xml.ProjectData;
using Newtonsoft.Json;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static string _basePath = "";
        private static string _baseFile = "";

        private static void Main(string[] args)
        {
            while (!Directory.Exists(_basePath))
            {
                Console.WriteLine($"Base directory does not exist: {_basePath}");
                Console.WriteLine($"Enter new base directory:");
                _basePath = Console.ReadLine() + "\\";
                Console.WriteLine($"Enter new base file name:");
                _baseFile = Console.ReadLine();
            }

            try
            {
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                var package = TestXmlInputSystem();
                TestParameterSets(package);
                var jobCollections = TestJobSystem(package);
                TestDbCreation(package, jobCollections);
            }
            catch (Exception e)
            {
                ExitOnKeyPress($"Fatal exception during execution:\n{e}");
            }

            ExitOnKeyPress("Finished successfully...");
        }

        private static ManagerPackage TestXmlInputSystem()
        {
            var filePath = _basePath + _baseFile + ".Input.xml";
      
            Console.WriteLine($"Reading project XML description from: {filePath}");
            if (!XmlStreamService.TryDeserialize(filePath, null, out XmlProjectModelData data, out var exception)) 
                ExitOnKeyPress($"Failed to read the XML file ...\nException: {exception.Message}");

            Console.WriteLine("Creating new model project ...");
            var package = CreateManagerPackage();

            Console.WriteLine("Pushing input data to model project...");
            var reports = package.ModelProject.InputPipeline.PushToProject(data.GetInputSequence());

            foreach (var report in reports)
            {
                if (!report.IsGood)
                {
                    Console.WriteLine(report.ToString());
                    ExitOnKeyPress("A validation failed, please refer to the report set...");
                }

                if (report?.ConflictReport.GetWarnings().FirstOrDefault() == null)
                    continue;

                Console.WriteLine(report.ToString());
            }

            Console.WriteLine("Model project input system done!");
            return package;
        }

        private static void TestParameterSets(ManagerPackage package)
        {
            var filePath = _basePath + _baseFile + ".Custom.xml";
            var customizationData = XmlProjectCustomization.Create(package.ModelProject);
            if (!File.Exists(filePath))
            {
                var outTest = XmlStreamService.TrySerialize(Console.OpenStandardOutput(), customizationData, out var exception);
                ExitOnKeyPress($"New customization file written to target: {filePath}");
            }
            Console.WriteLine($"Reading project XML customization data from: {filePath}");
            if (!XmlStreamService.TryDeserialize(filePath, null, out XmlProjectCustomization readData, out var exception2))
                ExitOnKeyPress($"Failed to read the XML file ...\nException: {exception2.Message}");

            readData.PushToModel(package.ModelProject);
            Console.WriteLine("Parameter input system done!");
        }

        private static IList<IJobCollection> TestJobSystem(ManagerPackage package)
        {
            var filePath = _basePath + _baseFile + ".Jobs.xml";

            Console.WriteLine($"Reading job XML description from: {filePath}");
            if (!XmlStreamService.TryDeserialize(filePath, null, out XmlDbCreationInstruction readData, out var exception))
                ExitOnKeyPress($"Failed to read the XML file ...\nException: {exception.Message}");

            Console.WriteLine("Job input system done!");
            return readData.ToInternals(package.ModelProject).ToList();
        }

        private static void TestDbCreation(ManagerPackage package, IList<IJobCollection> jobCollections)
        {
            var filePath = _basePath + _baseFile + ".db";

            Console.WriteLine($"Starting job database creation system with target: {filePath}");
            Console.WriteLine("Building project model context...");
            var contextBuilder = new ProjectModelContextBuilder(package.ModelProject);
            var modelContext = contextBuilder.BuildNewContext().Result;

            Console.WriteLine("Translating XML job descriptions to database entities...");
            var dbLatticeBuilder = new CeriaLatticeDbBuilder(modelContext);
            var dbJobBuilder = new JobDbEntityBuilder(modelContext)
            {
                LatticeDbEntityBuilder = dbLatticeBuilder
            };


            var result = jobCollections.Select(x => dbJobBuilder.BuildJobPackageModel(x)).ToList();

            Console.WriteLine("Adding entities to database context and saving changes...");
            var dbContext = new SimulationDbContext(filePath, true);
            dbContext.AddRange(result);
            dbContext.SaveChangesAsync().Wait();
            dbContext.Dispose();

            ExitOnKeyPress("Database creation system done!");
        }


        private static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }

        private static void ExitOnKeyPress(string message)
        {
            Console.WriteLine(message + "\nPress button to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static ManagerPackage CreateManagerPackage()
        {
            var filePath = _basePath + "Mocassin.Settings.xml";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Settings missing, created new @ {filePath}");
                WriteDefaultSettingsContract(filePath);
            }

            var projectSettings = ProjectSettings.Deserialize(filePath);
            return ManagerFactory.DebugFactory.CreateSimulationManagementPackage(projectSettings);
        }

        private static void WriteDefaultSettingsContract(string filePath)
        {
            var settings = ProjectSettings.CreateDefault();
            settings.Serialize(filePath);
        }
    }
}