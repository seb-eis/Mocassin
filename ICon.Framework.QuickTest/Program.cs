using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.Custom.Mmcfe;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static double FBoltz { get; } = 8.617e-5;

        private static double Temp { get; } = 273.15;

        private static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var logFile = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\Tests\test1\mmcfelog.db";
            var outFile = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\Tests\test1\Histograms\hist";
            using (var context = MmcfeEvaluationContext.OpenFile(logFile))
            {
                var readers = context.FullReaderSet().ToList();
                var evaluator = new MmcfeEnergyEvaluator();
                var energyStates = evaluator.CalculateEnergyStates(readers, 273);

                var interpols = Enumerable.Range(0, 20).Select(x => evaluator.LinearInterpolateEnergyState(energyStates, 273 + x * 100)).ToList();   
            }
            //WriteHistogramFileCollection(outFile, readers);
            ExitOnKeyPress("Finished successfully...");
        }

        private static void WriteEnergiesToFile(IList<double> energies)
        {
            using (var streamWriter =
                File.AppendText(@"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\Tests\Histograms\Fint.txt"))
            {
                for (var i = 0; i < energies.Count; i++) streamWriter.WriteLine($"{i} {energies[i]}");
            }
        }

        private static void WriteHistogramFileCollection(string baseFileName, IEnumerable<MmcfeLogReader> logReaders)
        {
            foreach (var logReader in logReaders)
            {
                var fileName = $"{baseFileName}_{logReader.ReadParameters().AlphaCurrent:0.00}.txt";
                WriteHistogramToFile(fileName, logReader);
            }
        }

        private static void WriteHistogramToFile(string filename, MmcfeLogReader logReader)
        {
            var header = logReader.EnergyHistogramReader.ReadHeader();
            var energy = header.MinValue;
            var sum = 0L;
            using (var stream = File.AppendText(filename))
            {
                foreach (var counter in logReader.EnergyHistogramReader.ReadCounters())
                {
                    sum += counter;
                    if (counter >= 100) stream.WriteLine($"{0.5 * energy:E7} {counter:E7}");
                    energy += header.Stepping;
                }
            }
        }

        private static void WriteHistogramCollectionToFile(string filename, IList<MmcfeLogReader> logReaders)
        {
            var header = logReaders.First().EnergyHistogramReader.ReadHeader();
            var energyOffset = 0.0;

            var matrix = logReaders.Select(x => x.EnergyHistogramReader.ReadCounters().ToArray()).ToList();
            var baseEnergies = logReaders.Select(x => x.EnergyHistogramReader.ReadHeader().MinValue).ToList();
            var stringBuilder = new StringBuilder(10000);
            using (var stream = File.AppendText(filename))
            {
                for (var i = 0; i < header.EntryCount; i++)
                {
                    stringBuilder.Clear();
                    var sum = 0L;
                    for (var j = 0; j < logReaders.Count; j++)
                    {
                        sum += matrix[j][i];
                        stringBuilder.Append($"{baseEnergies[j] + energyOffset:E5} {matrix[j][i]:E5} ");
                    }

                    stringBuilder.PopBack(1);
                    if (sum >= 100) stream.WriteLine(stringBuilder.ToString());
                    energyOffset += header.Stepping;
                }
            }
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