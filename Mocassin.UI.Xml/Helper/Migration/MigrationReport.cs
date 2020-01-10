using System;
using System.Collections.Generic;

namespace Mocassin.UI.Xml.Helper.Migration
{
    /// <summary>
    ///     Stores and provides information about a project data migration operation
    /// </summary>
    public class MigrationReport
    {
        /// <summary>
        ///     Get the <see cref="IList{T}"/> of <see cref="MigrationReportEntry"/> items that store successful data migrations
        /// </summary>
        public IReadOnlyList<MigrationReportEntry> Entries { get; }

        /// <summary>
        ///     Creates a new <see cref="MigrationReport"/>
        /// </summary>
        /// <param name="reportEntries"></param>
        public MigrationReport(IReadOnlyList<MigrationReportEntry> reportEntries)
        {
            Entries = reportEntries ?? throw new ArgumentNullException(nameof(reportEntries));
        }
    }
}