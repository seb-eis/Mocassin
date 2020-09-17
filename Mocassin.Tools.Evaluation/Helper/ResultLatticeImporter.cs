using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Events;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;
using Mocassin.Tools.UAccess.Readers;

namespace Mocassin.Tools.Evaluation.Helper
{
    /// <summary>
    ///     Provides methods to recycle results from one simulation database as initial lattices in another
    /// </summary>
    public class ResultLatticeImporter : IDisposable
    {
        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> that reports how many jobs have been imported
        /// </summary>
        public ReactiveEvent<int> ImportCountEvent { get; }

        /// <summary>
        ///     Get a <see cref="IObservable{T}" /> that notifies how man jobs have been imported
        /// </summary>
        public IObservable<int> ImportCountNotifications => ImportCountEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IComparer{T}" /> for numeric values
        /// </summary>
        public IComparer<double> NumericComparer { get; }

        /// <summary>
        ///     Creates new <see cref="ResultLatticeImporter" />
        /// </summary>
        /// <param name="numericComparer"></param>
        public ResultLatticeImporter(IComparer<double> numericComparer)
        {
            NumericComparer = numericComparer ?? throw new ArgumentNullException(nameof(numericComparer));
            ImportCountEvent = new ReactiveEvent<int>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ImportCountEvent.OnCompleted();
        }

        /// <summary>
        ///     Checks if two <see cref="IJobMetaData" /> entries should have matching data required for simulation lattice
        ///     importing
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool AreCompatibleForLatticeImport(IJobMetaData first, IJobMetaData second) =>
            first.ConfigName == second.ConfigName &&
            first.CollectionName == second.CollectionName &&
            first.JobIndex == second.JobIndex &&
            first.DopingInfo == second.DopingInfo &&
            first.LatticeInfo == second.LatticeInfo;

        /// <summary>
        ///     Enumerates the source/target import pairs based on <see cref="JobMetaDataEntity" /> collections. Throws if at least
        ///     one item cannot be mapped or is mapped twice
        /// </summary>
        /// <param name="exportSet"></param>
        /// <param name="importSet"></param>
        /// <returns></returns>
        public IEnumerable<(JobMetaDataEntity Source, JobMetaDataEntity Target)> ZipExportWithImport(ICollection<JobMetaDataEntity> exportSet,
            ICollection<JobMetaDataEntity> importSet)
        {
            if (exportSet.Equals(importSet)) throw new ArgumentException("Source and target collection cannot be identical");
            if (exportSet.Count != importSet.Count) throw new InvalidOperationException("Source and target collection have unequal size.");

            var usedImportTargets = new HashSet<JobMetaDataEntity>();
            foreach (var exportTarget in exportSet)
            {
                var importTarget = importSet.First(x => AreCompatibleForLatticeImport(x, exportTarget));
                if (usedImportTargets.Contains(importTarget)) throw new InvalidOperationException("A target was used twice, data cannot be zipped together.");
                usedImportTargets.Add(importTarget);
                yield return (exportTarget, importTarget);
            }

            usedImportTargets.Clear();
        }

        /// <summary>
        ///     Imports the lattice results from the source and exports them to the target as new initial lattices
        /// </summary>
        /// <param name="exportList"></param>
        /// <param name="importList"></param>
        public void ImportFinalLatticesAsInitialLattices(IList<JobMetaDataEntity> exportList, IList<JobMetaDataEntity> importList)
        {
            var counter = 0;
            using var marshalService = new MarshalService();
            foreach (var (source, target) in ZipExportWithImport(exportList, importList))
            {
                ImportFinalLatticeAsInitialLattice(source, target, marshalService);
                ImportCountEvent.OnNext(++counter);
            }
        }

        /// <summary>
        ///     Imports the result lattice from one job to another using the provides <see cref="JobMetaDataEntity" /> items and
        ///     <see cref="IMarshalService" />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="marshalService"></param>
        public void ImportFinalLatticeAsInitialLattice(JobMetaDataEntity source, JobMetaDataEntity target, IMarshalService marshalService)
        {
            using var mcsReader = McsContentReader.Create(source.JobModel.JobResultData.SimulationStateBinary);
            var sourceLattice = mcsReader.ReadLattice();
            target.JobModel.SimulationLatticeModel.ChangePropertyStatesToObjects(marshalService);
            target.JobModel.SimulationLatticeModel.Lattice.ImportDataFrom(sourceLattice);
            target.JobModel.SimulationLatticeModel.ChangePropertyStatesToBinaries(marshalService);
            source.JobModel.JobResultData.SimulationStateBinary = null;
        }

        /// <summary>
        ///     Convenience function to import the result lattices from an exporting msl database as initial lattices for an
        ///     importing msl database
        /// </summary>
        /// <param name="pathToExportMsl"></param>
        /// <param name="pathToImportMsl"></param>
        public void ImportFinalLatticesAsInitialLattices(string pathToExportMsl, string pathToImportMsl)
        {
            try
            {
                using var exportContext = SqLiteContext.OpenDatabase<SimulationDbContext>(pathToExportMsl);
                var exportList = LoadAsExporting(exportContext.JobMetaData);
                exportContext.Dispose();

                using var importContext = SqLiteContext.OpenDatabase<SimulationDbContext>(pathToImportMsl);
                var importList = LoadAsImporting(importContext.JobMetaData);

                ImportFinalLatticesAsInitialLattices(exportList, importList);
                importContext.SaveChanges();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "Error while importing data, the databases are most likely not compatible to each other or result data is missing.", exception);
            }
        }

        /// <summary>
        ///     Performs a check if the data in export and import path can be mapped without errors and reports how many jobs have
        ///     been mapped
        /// </summary>
        /// <param name="pathToExportMsl"></param>
        /// <param name="pathToImportMsl"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool DataCanBeMapped(string pathToExportMsl, string pathToImportMsl, out int count)
        {
            count = 0;
            try
            {
                using var exportContext = SqLiteContext.OpenDatabase<SimulationDbContext>(pathToExportMsl);
                var exportList = LoadAsExporting(exportContext.JobMetaData);
                exportContext.Dispose();

                using var importContext = SqLiteContext.OpenDatabase<SimulationDbContext>(pathToImportMsl);
                var importList = LoadAsImporting(importContext.JobMetaData);

                if (exportList.Any(x => x.JobModel.JobResultData.SimulationStateBinary == null)) return false;
                count = ZipExportWithImport(exportList, importList).Count();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        ///     Prepares and loads the <see cref="IQueryable{T}" /> of <see cref="JobMetaDataEntity" /> for lattice import
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public IList<JobMetaDataEntity> LoadAsImporting(IQueryable<JobMetaDataEntity> metaData)
        {
            return metaData
                   .Include(x => x.JobModel)
                   .ThenInclude(x => x.SimulationLatticeModel)
                   .ToList();
        }

        /// <summary>
        ///     Prepares and loads the <see cref="IQueryable{T}" /> of <see cref="JobMetaDataEntity" /> for lattice export
        /// </summary>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public IList<JobMetaDataEntity> LoadAsExporting(IQueryable<JobMetaDataEntity> metaData)
        {
            return metaData
                   .Include(x => x.JobModel)
                   .ThenInclude(x => x.JobResultData)
                   .ToList();
        }
    }
}