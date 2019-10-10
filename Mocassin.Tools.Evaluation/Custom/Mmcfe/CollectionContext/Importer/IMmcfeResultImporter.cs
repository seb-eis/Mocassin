using System;
using System.Linq.Expressions;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;

namespace Mocassin.Tools.Evaluation.Custom.Mmcfe.Importer
{
    /// <summary>
    ///     Represents an importer for populating <see cref="MmcfeLogCollectionDbContext" /> from raw simulation results
    /// </summary>
    public interface IMmcfeResultImporter
    {
        /// <summary>
        ///     Get a <see cref="IObservable{T}" /> that informs about
        /// </summary>
        IObservable<int> JobImportedNotification { get; }

        /// <summary>
        ///     Get or set the <see cref="MmcfeLogCollectionDbContext" /> used for data collection
        /// </summary>
        MmcfeLogCollectionDbContext ImportDbContext { get; set; }

        /// <summary>
        ///     Get a boolean flag if the system is currently importing
        /// </summary>
        bool IsImporting { get; }

        /// <summary>
        ///     Get a boolean flag if the system is currently saving data
        /// </summary>
        bool IsSaving { get;}

        /// <summary>
        ///     Collects the data into the set <see cref="MmcfeLogDbContext" /> with an optional acceptance predicate expression
        /// </summary>
        void Import(Expression<Func<JobMetaDataEntity, bool>> predicate = null);
    }
}