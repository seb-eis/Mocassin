using System;
using System.Diagnostics;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Selection;
using Mocassin.Tools.Evaluation.Selection.Counting.Selectors;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var mslFilename = @"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\GuiTesting\GdCeO.Filled.msl";
            var evalContext = MslEvaluationContext.Create(mslFilename);
            var watch = Stopwatch.StartNew();

            var data = evalContext.EvaluationJobSet();

            var jobContexts = evalContext.LoadJobContexts(data);

            var countQuery = new ParticleCountQuery(jobContexts);
            var r1Query = new EnsembleMovementQuery(jobContexts) {ParticleCountSource = countQuery};

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