using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Mathematics.Comparers;
using Mocassin.Model.Particles;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Custom.Mmcfe;
using Mocassin.Tools.Evaluation.Custom.Mmcfe.Importer;
using Mocassin.Tools.Evaluation.Extensions;
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
            TestHyperSurfaceEvaluator();
            //ImportMmcfeCollection();
        }

        private static void TestHyperSurfaceEvaluator()
        {
            var dataPath = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\model0_y005_y030\mmcfe_import_eval.db";
            var mslPath = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\model0_y005_y030\y005_y030.msl";

            var mslContext = MslEvaluationContext.Create(mslPath);
            var projectContext = mslContext.GetProjectModelContext(1);
            var particles = projectContext.GetModelObjects<IParticle>().ToList();
            using (var dataSource = SqLiteContext.OpenDatabase<MmcfeLogCollectionDbContext>(dataPath).AsReadOnly())
            {
                var comparer = NumericComparer.CreateRanged(1.0e-10);
                var evaluator = new MmcfeEnergyDataPointEvaluator(dataSource);
                evaluator.LoadDataPoints();
                var list0 = evaluator.GroupDataPointsByDopingByTemperature();
                var list1 = evaluator.GroupDataPointsByTemperatureByDoping();
                evaluator.Dispose();
            }
        }

        private static void ImportMmcfeCollection()
        {
            var rawPath = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\model0_y005_y030\mmcfe_import_raw.db";
            var evalPath = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\model0_y005_y030\mmcfe_import_eval.db";
            var rootPath = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\model0_y005_y030";
            var libraryPath = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\model0_y005_y030\y005_y030.msl";

            var importer = new MmcfeJobFolderImporter(libraryPath, rootPath) {ImportsPerSave = 100};
            var exceptions = new List<Exception>();
            var importCount = 0;
            var lockObject = new object();
            using (var context = SqLiteContext.OpenDatabase<MmcfeLogCollectionDbContext>(rawPath, true))
            {
                importer.ImportDbContext = context;
                importer.JobImportedNotification.Subscribe(x => WriteProgress(ref importCount, lockObject), e => exceptions.Add(e));
                importer.Import();
                Console.WriteLine($"\nDone import with {exceptions.Count} errors.");
                Console.Write("Copying to evaluation context ... ");
                context.CopyDatabaseWithoutRawData(evalPath);
                Console.Write("Done\n");
            }
        }

        private static void WriteProgress(ref int counter, object lockObject)
        {
            lock (lockObject)
            {
                Console.Write($"\rImporting logs ... {++counter:D5}");
            }
        }

        private static void WriteEnergiesToFile(IList<double> energies)
        {
            var path = @"C:\Users\hims-user\Documents\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\Tests\Histograms\Fint.txt";
            using (var streamWriter = File.AppendText(path))
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