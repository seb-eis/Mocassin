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
using Mocassin.UI.Xml.CustomizationData;
using Mocassin.UI.Xml.ParticleData;
using Mocassin.UI.Xml.ProjectData;
using Newtonsoft.Json;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private const string _basePath = "C:\\Users\\hims-user\\Documents\\Gitlab\\MocassinTestFiles\\MocassinBaZrO3";

        private static void Main(string[] args)
        { 
            var package = TestXmlUI();
            TestParameterSets(package);
            TestDbCreation(package);

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
            Console.Write("");
        }

        private static void TestDbCreation(ManagerPackage package)
        {
            for (int i = 0; i < 1; i++)
            {
                var contextBuilder = new ProjectModelContextBuilder(package.ModelProject);
                var modelContext = contextBuilder.BuildNewContext().Result;
                var dbLatticeBuilder = new CeriaLatticeDbBuilder(modelContext);
                var dbJobBuilder = new JobDbEntityBuilder(modelContext)
                {
                    LatticeDbEntityBuilder = dbLatticeBuilder
                };

                dbJobBuilder.AddPostBuildOptimizer(new JumpSelectionOptimizer());

                var jobCollection = GetKmcTestCollection(package.ModelProject, 5);
                var watch = Stopwatch.StartNew();

                var result = dbJobBuilder.BuildJobPackageModel(jobCollection);
                DisplayWatch(watch);
                var dbContext = new SimulationDbContext("C:\\Users\\hims-user\\Documents\\Gitlab\\MocassinTestFiles\\InteropTestJohn.db", true);
                dbContext.Add(result);
                dbContext.SaveChangesAsync().Wait();
                dbContext.Dispose();
                
                DisplayWatch(watch);
            }
        }


        private static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }

        private static IJobCollection GetKmcTestCollection(IModelProject project, int jobCount)
        {
            var random = new PcgRandom32("AkMartin12345");
            var baseJob = new KmcJobConfiguration
            {
                ElectricFieldModulus = 1e8,
                BaseFrequency = 1e13,
                FixedNormalizationFactor = 1.0,
                MinimalSuccessRate = 1.0,
                LatticeConfiguration = new LatticeConfiguration
                {
                    SizeA = 10, SizeB = 10, SizeC = 10
                },
                JobId = 0,
                RngIncreaseSeed = BitConverter.ToInt64(BitConverter.GetBytes(random.State), 0),
                RngStateSeed = BitConverter.ToInt64(BitConverter.GetBytes(random.Increment), 0),
                TargetMcsp = 200,
                TimeLimit = (long) TimeSpan.FromHours(24).TotalSeconds,
                Temperature = 1000
            };

            var collection = new KmcJobCollection
            {
                Simulation = project.GetManager<ISimulationManager>().QueryPort.Query(port => port.GetKineticSimulation(0)),
                JobConfigurations = new List<KmcJobConfiguration>(jobCount)
            };

            for (var i = 0; i < jobCount; i++)
            {
                var job = new KmcJobConfiguration();
                baseJob.CopyTo(job);
                job.Temperature += i;
                collection.JobConfigurations.Add(job);
            }

            return collection;
        }
    }
}