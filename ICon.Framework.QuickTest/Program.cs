using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Random;
using Mocassin.Framework.Xml;
using Mocassin.Model.Basic;
using Mocassin.Model.Basic.Debug;
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

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private const string _basePath = "C:\\Users\\hims-user\\Documents\\Gitlab\\MocassinTestFiles\\MocassinCeriaInput";

        private static void Main(string[] args)
        { 
            var package = TestXmlUI();
            TestParameterSets(package);
            TestDbCreation(package);

            Console.ReadLine();
        }

        private static ManagerPackage TestXmlUI()
        {
            var inputFilePath = _basePath + ".xml";
            var data = new XmlMocassinProjectData() { ParticleModelData = new XmlParticleModelData()};

            var inTest = XmlStreamService.TryDeserialize(inputFilePath, null, out data, out var exception);
            var outTest = XmlStreamService.TrySerialize(Console.OpenStandardOutput(), data, out exception);

            var package = ManagerFactory.DebugFactory.CreateSimulationManagementPackage();
            var inputter = new ProjectDataInputSystem();
            inputter.AddMany(data.GetInputSequence());
            inputter.AutoInputData(package.ModelProject);

            var report = inputter.GetReportJson();
            Console.Write(report);


            return package;
        }

        private static void TestParameterSets(ManagerPackage package)
        {
            var outputFilePath =  _basePath + ".EnergyParam.xml";

            var energySetterProvider = package.EnergyManager.QueryPort.Query(port => port.GetEnergySetterProvider());
            var energyParameterSet = XmlEnergyParameterSet.Create(energySetterProvider);

            var outTest = XmlStreamService.TrySerialize(Console.OpenStandardOutput(), energyParameterSet, out var exception);
            var writeTest = XmlStreamService.TrySerialize(outputFilePath, energyParameterSet, out exception);

            var test = XmlKineticRule.Create(
                package.TransitionManager.QueryPort.Query(port => port.GetKineticTransitions().First().GetTransitionRules().First()));

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
                JobInfoFlags = default,
                JobId = 0,
                KmcJobFlags = default,
                RngIncreaseSeed = BitConverter.ToInt64(BitConverter.GetBytes(random.State), 0),
                RngStateSeed = BitConverter.ToInt64(BitConverter.GetBytes(random.Increment), 0),
                TargetMcsp = 200,
                TimeLimit = (long) TimeSpan.FromHours(24).TotalSeconds,
                StateFlags = default,
                StateSize = 0,
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