using System;
using System.Diagnostics;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.Translator.DbBuilder;
using Mocassin.Model.Translator.ModelContext;
using Newtonsoft.Json;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var package = ManagerFactory.DebugFactory.CreateManageSystemForCeria();
            var contextBuilder = new ProjectModelContextBuilder(package.ModelProject);
            var context = contextBuilder.BuildNewContext().Result;

            var dbStructureBuilder = new StructureDbModelBuilder(context);
            //var dbEnergyBuilder = new EnergyDbModelBuilder(context);
            var kmcDbModel = dbStructureBuilder.BuildModel(context.SimulationModelContext.KineticSimulationModels.First());
            //var mmcDbModel = dbStructureBuilder.BuildModel(context.SimulationModelContext.MetropolisSimulationModels.First());

            Console.ReadLine();
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