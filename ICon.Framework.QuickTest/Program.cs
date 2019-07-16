using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
            var onsagerEval = new CubicOnsagerEvaluation(evaluableSet);

            var q2 = Math.Pow(Equations.Constants.ElementalCharge * 2, 2);
            var histogram = new int[10000];
            var step = 0.01;

            foreach (var item in onsagerEval)
            {
                var cond = item[1, 1] * q2 / 1.602e-19;
                var index = (int) (cond / step);
                histogram[index]++;
            }

            File.AppendAllLines(@"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\GuiTesting\onsager_cond",
                histogram.Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture)));

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