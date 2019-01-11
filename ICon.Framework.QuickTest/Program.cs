using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Random;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Simulations;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.EntityBuilder;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Model.Translator.Optimization;
using Newtonsoft.Json;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                var package = ManagerFactory.DebugFactory.CreateManageSystemForCeria();

                var contextBuilder = new ProjectModelContextBuilder(package.ModelProject);
                var modelContext = contextBuilder.BuildNewContext().Result;
                var dbLatticeBuilder = new CeriaLatticeDbBuilder(modelContext);
                var dbJobBuilder = new JobDbEntityBuilder(modelContext)
                {
                    LatticeDbEntityBuilder = dbLatticeBuilder
                };

                dbJobBuilder.AddPostBuildOptimizer(new JumpSelectionOptimizer());

                var jobCollection = GetKmcTestCollection(package.ModelProject);
                var watch = Stopwatch.StartNew();

                var result = dbJobBuilder.BuildJobPackageModel(jobCollection);
                DisplayWatch(watch);
                var dbContext = new SimulationDbContext("C:\\Users\\hims-user\\Documents\\Gitlab\\MocassinTestFiles\\InteropTest.db", true);
                dbContext.Add(result);
                dbContext.SaveChangesAsync().Wait();
                dbContext.Dispose();
                
                DisplayWatch(watch);
            }

            Console.ReadLine();
        }


        private static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }

        private static IJobCollection GetKmcTestCollection(IModelProject project)
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
                JobFlags = default,
                JobId = 0,
                KmcJobFlags = default,
                RngIncreaseSeed = 0,
                RngStateSeed = 0,
                TargetMcsp = 200,
                TimeLimit = (long) TimeSpan.FromHours(24).TotalSeconds,
                StatusFlags = default,
                StateSize = 0,
                Temperature = 1000
            };

            var collection = new KmcJobCollection
            {
                Simulation = project.GetManager<ISimulationManager>().QueryPort.Query(port => port.GetKineticSimulation(0)),
                JobConfigurations = new List<KmcJobConfiguration>(1000)
            };

            for (var i = 0; i < 1000; i++)
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