using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Custom.Mmcfe;
using Mocassin.Tools.Evaluation.Custom.Mmcfe.Importer;
using Mocassin.Tools.Evaluation.Extensions;

namespace Mocassin.Framework.QuickTest
{
    public static class MmcfeEvalScript
    {
        public static void Run()
        {
            var rootPath =
                @"C:\Users\Sebastian\Documents\Promotion\HO_Backup_Corona\HO_Backup_Corona\Promotions_Unterlagen\Projekte\BaZrO3\Simulation\Model.650PM.Y030\";
            var mslName = "bzo_y030_650pm_mmcfe.msl";
            var evalDbName = "mmcfe.eval.db";

            var extractArgs = MmcfeTemperatureDefectExtractArgs.CreateForRoot(rootPath, mslName, evalDbName);
            extractArgs.Temperatures = Enumerable.Range(0, 13).Select(e => 273.0 + e * 100).ToList();
            extractArgs.DefectSelector = x => x.Symbol == "H";
            extractArgs.FixedDopingSelector = x => x.Index == 0;
            extractArgs.VariableDopingSelector = x => x.Index == 1;
            extractArgs.FixedDopingValue = 0.3;
            ExtractHyperSurfaceDataByTemperatures(extractArgs);
        }

        private static void ExtractHyperSurfaceDataByTemperatures(MmcfeTemperatureDefectExtractArgs args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var evalContext = MmcfeEvaluationContext.Open(args.EvalDbPath, args.MslPath);

            var evaluator = new MmcfeEnergyDataPointEvaluator(evalContext.LogCollectionContext.AsReadOnly());
            evaluator.LoadDataPoints();

            var data = evaluator.GroupDataPointsByTemperatureByDoping();
            var particles = evalContext.ProjectContext.GetModelObjects<IParticle>().ToList();
            var dopings = evalContext.ProjectContext.GetModelObjects<IDoping>().ToList();

            var fixedDopingId = dopings.First(d => args.FixedDopingSelector.Invoke(d)).Index;
            var variableDopingId = dopings.First(d => args.VariableDopingSelector.Invoke(d)).Index;
            var defectId = particles.Single(p => args.DefectSelector.Invoke(p)).Index;

            foreach (var temperature in args.Temperatures)
            {
                var rawData = data.First(x => x.Key <= temperature);
                Console.Write("Doing temperature target {0} (found {1}) K ...", temperature, rawData.Key);

                var plotData = args.UseRelativeMode
                    ? evaluator.GetRelativeChangePerDefectPlotData2D(rawData.Value, fixedDopingId, args.FixedDopingValue, variableDopingId, defectId)
                    : evaluator.GetAbsoluteChangePlotData2D(rawData.Value, fixedDopingId, args.FixedDopingValue, variableDopingId, defectId);

                evaluator.WriteEnergyStateOverConcentrationPlotData2DToFile(plotData,
                    string.Format(args.PlotPathFormat, rawData.Key, args.UseRelativeMode ? "rel" : "abs"));
                Console.Write("Done!\n");
            }
        }

        public static void RunCreateMmcfeEvalDatabasesFromJobs(string rootPath, string mslFileName, bool createEvalDb = true, bool zipDeleteRaw = true)
        {
            void WriteProgress(ref int counter, object lockObj)
            {
                lock (lockObj)
                {
                    Console.Write($"Done: {++counter:D5}\r");
                }
            }

            var rawPath = $"{rootPath}\\mmcfe.raw.db";
            var evalPath = $"{rootPath}\\mmcfe.eval.db";
            var libraryPath = $"{rootPath}\\{mslFileName}";
            var importer = new MmcfeJobFolderImporter(libraryPath, rootPath) {ImportsPerSave = 100};
            var exceptions = new List<Exception>();
            var importCount = 0;
            var lockObject = new object();
            using (var context = SqLiteContext.OpenDatabase<MmcfeLogCollectionDbContext>(rawPath, true))
            {
                Console.WriteLine("Creating raw import database ... ");
                importer.ImportDbContext = context;
                importer.JobImportedNotification.Subscribe(x => WriteProgress(ref importCount, lockObject), e => exceptions.Add(e));
                importer.Import();
                Console.WriteLine($"\nDone import with {exceptions.Count} errors.");
                if (!createEvalDb) return;

                Console.WriteLine("Creating minimal eval database ... ");
                context.CopyDatabaseWithoutRawData(evalPath);
                Console.WriteLine("Done");

                if (!zipDeleteRaw) return;
                ZipAndDelete(rawPath, $"{rootPath}\\mmcfe.raw.zip");
            }
        }

        private static void ZipAndDelete(string source, string target)
        {
            Console.WriteLine("Compressing {0} into {1}", source, target);
            using (var targetStream = new FileStream(target, FileMode.Create))
            {
                using (var archive = new ZipArchive(targetStream, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntryFromFile(source, Path.GetFileName(source));
                    var sourceInfo = new FileInfo(source);
                    var targetInfo = new FileInfo(target);
                    Console.WriteLine("Done @ {0} KB ==> {1} KB ({2} %)", sourceInfo.Length / 1024, targetInfo.Length / 1024,
                        targetInfo.Length / sourceInfo.Length);
                }
            }

            File.Delete(source);
        }

        private class MmcfeEvaluationContext
        {
            public MslEvaluationContext MslContext { get; set; }

            public IProjectModelContext ProjectContext { get; set; }

            public MmcfeLogCollectionDbContext LogCollectionContext { get; set; }

            public static MmcfeEvaluationContext Open(string dataPath, string mslPath, int mainProjectContextId = 1)
            {
                var result = new MmcfeEvaluationContext
                {
                    MslContext = MslEvaluationContext.Create(mslPath),
                    LogCollectionContext = SqLiteContext.OpenDatabase<MmcfeLogCollectionDbContext>(dataPath)
                };
                result.ProjectContext = result.MslContext.GetProjectModelContext(mainProjectContextId);
                return result;
            }
        }

        private class MmcfeDataExtractArgs
        {
            public string EvalDbPath { get; set; }

            public string MslPath { get; set; }
        }

        private class MmcfeTemperatureDefectExtractArgs : MmcfeDataExtractArgs
        {
            public string PlotPathFormat { get; set; }

            public IEnumerable<double> Temperatures { get; set; }

            public Func<IParticle, bool> DefectSelector { get; set; }

            public Func<IDoping, bool> VariableDopingSelector { get; set; }

            public Func<IDoping, bool> FixedDopingSelector { get; set; }

            public double FixedDopingValue { get; set; }

            public bool UseRelativeMode { get; set; }

            public static MmcfeTemperatureDefectExtractArgs CreateForRoot(string rootPath, string mslName, string evalDbName,
                string plotPathFormat = "exports\\ufs.{0:F2}.{1}.txt") =>
                new MmcfeTemperatureDefectExtractArgs
                {
                    EvalDbPath = $"{rootPath}\\{evalDbName}",
                    MslPath = $"{rootPath}\\{mslName}",
                    PlotPathFormat = $"{rootPath}\\{plotPathFormat}"
                };
        }
    }
}