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
            MmcfeEvalScript.Run();
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