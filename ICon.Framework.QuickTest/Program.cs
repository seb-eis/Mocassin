using System;
using System.Diagnostics;
using Mocassin.Mathematics.Comparer;
using Mocassin.Tools.Evaluation.Helper;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var rootPath =
                @"C:\Users\Sebastian\Documents\Promotion\HO_Backup_Corona\HO_Backup_Corona\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\Model.4.23A.Y010\raw\Mmcfe";
            var dbName = @"mmcfe_y010_423pm.msl";
            MmcfeEvalScript.RunCreateMmcfeEvalDatabasesFromJobs(rootPath, dbName, true, false);
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