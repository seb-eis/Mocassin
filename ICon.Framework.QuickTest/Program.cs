using System;
using System.Diagnostics;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries;
using Newtonsoft.Json;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = "C:\\Users\\Sebastian\\Documents\\Promotion\\Mocassin\\Testing\\Databases\\veh030.msl";
            using var mslContext = MslEvaluationContext.Create(path);
            var jobSet = mslContext.LoadJobsAsEvaluable(query => query.Where(x => x.Id == 1));
            var eval = new GlobalTrackingEvaluation(jobSet);
            var result = eval.Result.First();
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