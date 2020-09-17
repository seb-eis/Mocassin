using System;
using System.Collections.Generic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Helper.Migration
{
    /// <summary>
    ///     Stores and provides information about a project data migration operation
    /// </summary>
    public class MigrationReport
    {
        /// <summary>
        ///     Get the <see cref="IList{T}" /> of <see cref="MigrationReportEntry" /> items that store successful data migrations
        /// </summary>
        public IReadOnlyList<MigrationReportEntry> Entries { get; }

        /// <summary>
        ///     Get the <see cref="ProjectDataObject" /> that served as the data source
        /// </summary>
        public ProjectDataObject MigrationSource { get; set; }

        /// <summary>
        ///     Get the <see cref="ProjectDataObject" /> that served as the data target
        /// </summary>
        public ProjectDataObject MigrationTarget { get; set; }

        /// <summary>
        ///     Creates a new <see cref="MigrationReport" />
        /// </summary>
        /// <param name="reportEntries"></param>
        /// <param name="migrationSource"></param>
        /// <param name="migrationTarget"></param>
        public MigrationReport(IReadOnlyList<MigrationReportEntry> reportEntries, ProjectDataObject migrationSource, ProjectDataObject migrationTarget)
        {
            Entries = reportEntries ?? throw new ArgumentNullException(nameof(reportEntries));
            MigrationSource = migrationSource ?? throw new ArgumentNullException(nameof(migrationSource));
            MigrationTarget = migrationTarget ?? throw new ArgumentNullException(nameof(migrationTarget));
        }

        /// <inheritdoc />
        public override string ToString() => $"Source: {MigrationSource}, Target: {MigrationTarget}, Count: {Entries.Count}";
    }
}