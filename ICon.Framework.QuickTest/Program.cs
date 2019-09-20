using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var guidStr = "b7f2dded-daf1-40c0-a1a4-ef9b85356af8";
            var guid = new Guid(guidStr);
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