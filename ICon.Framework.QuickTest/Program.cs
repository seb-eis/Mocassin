using System;
using System.Diagnostics;
using System.Linq;
using Mocassin.Model.DataManagement;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Selection;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var mslFilename = @"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\GuiTesting\GdCeO.Filled.msl";
            var evalContext = MslEvaluationContext.Create(mslFilename, ModelProjectFactory.CreateDefault);
            var watch = Stopwatch.StartNew();

            var query = evalContext.EvaluationJobSet()
                .Where(x => x.JobMetaData.CollectionName == "T.Sampling" && x.JobMetaData.Temperature == 1000);

            var selector1 = new EnsembleMovementSelector();
            var selector2 = new SquaredEnsembleMovementSelector();
            var jobContextSet = query.LoadResultContexts(evalContext);
            DisplayWatch(watch);
            var results1 = selector1.MapResults(jobContextSet);
            DisplayWatch(watch);
            var results2 = selector2.MapResults(jobContextSet);
            DisplayWatch(watch);


            ExitOnKeyPress("Finished successfully...");
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
    }
}