using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Random;
using Mocassin.Framework.Xml;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.EntityBuilder;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Model.Translator.Optimization;
using Mocassin.UI.Xml.CreationData;
using Mocassin.UI.Xml.CustomizationData;
using Mocassin.UI.Xml.ParticleData;
using Mocassin.UI.Xml.ProjectData;
using Newtonsoft.Json;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private const string _basePath = "C:\\Users\\hims-user\\Documents\\Gitlab\\MocassinTestFiles\\MocassinCeria";

        private static void Main(string[] args)
        { 
            var package = TestXmlUI();
            TestParameterSets(package);
            var jobCollections = TestJobSystem(package);
            TestDbCreation(package, jobCollections);

            Console.ReadLine();
        }

        private static ManagerPackage TestXmlUI()
        {
            var inputFilePath = _basePath + ".Input.xml";

            var inTest = XmlStreamService.TryDeserialize(inputFilePath, null, out XmlProjectModelData data, out var exception);
            var outTest = XmlStreamService.TrySerialize(Console.OpenStandardOutput(), data, out exception);

            var package = ManagerFactory.DebugFactory.CreateSimulationManagementPackage();
            var reports = package.ModelProject.InputPipeline.PushToProject(data.GetInputSequence());

            foreach (var report in reports)
            {
                Console.WriteLine(report.ToString());
            }

            return package;
        }

        private static void TestParameterSets(ManagerPackage package)
        {
            Exception exception;
            var outputFilePath =  _basePath + ".Custom.xml";
            var customizationData = XmlProjectCustomization.Create(package.ModelProject);

            var outTest = XmlStreamService.TrySerialize(Console.OpenStandardOutput(), customizationData, out exception);
            var writeTest = XmlStreamService.TrySerialize(outputFilePath, customizationData, out exception);

            var inTest = XmlStreamService.TryDeserialize(outputFilePath, null, out XmlProjectCustomization readData, out exception);
            readData.PushToModel(package.ModelProject);
            Console.Write("Parameters done!");
        }

        private static IList<IJobCollection> TestJobSystem(ManagerPackage package)
        {
            Exception exception;
            var outputFilePath =  _basePath + ".Jobs.xml";
            
            var inTest = XmlStreamService.TryDeserialize(outputFilePath, null, out XmlDbCreationInstruction readData, out exception);
            var outTest = XmlStreamService.TrySerialize(Console.OpenStandardOutput(), readData, out exception);

            Console.Write("Job system done!");
            return readData.ToInternals(package.ModelProject).ToList();
        }

        private static void TestDbCreation(ManagerPackage package, IList<IJobCollection> jobCollections)
        {
            var filePath = _basePath + ".db";

            var contextBuilder = new ProjectModelContextBuilder(package.ModelProject);
            var modelContext = contextBuilder.BuildNewContext().Result;
            var dbLatticeBuilder = new CeriaLatticeDbBuilder(modelContext);
            var dbJobBuilder = new JobDbEntityBuilder(modelContext)
            {
                LatticeDbEntityBuilder = dbLatticeBuilder
            };

            dbJobBuilder.AddPostBuildOptimizer(new JumpSelectionOptimizer());

            var watch = Stopwatch.StartNew();

            var result = jobCollections.Select(x => dbJobBuilder.BuildJobPackageModel(x)).ToList();
            DisplayWatch(watch);
            var dbContext = new SimulationDbContext(filePath, true);
            dbContext.AddRange(result);
            dbContext.SaveChangesAsync().Wait();
            dbContext.Dispose();
            
            DisplayWatch(watch);
        }


        private static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }
    }
}