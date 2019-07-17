using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries;
using Mocassin.Tools.Evaluation.Queries.Base;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var mslFilename = @"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\GuiTesting\GdCeO.Filled.msl";
            var mslFilename = @"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\GuiTesting\GdCeO.Onsager.msl";
            var evalContext = MslEvaluationContext.Create(mslFilename);
            var data = evalContext.EvaluationJobSet().Where(x => x.JobMetaData.ConfigName == "T1000");
            var evaluableSet = evalContext.MakeEvaluableSet(data);

            var watch = Stopwatch.StartNew();
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