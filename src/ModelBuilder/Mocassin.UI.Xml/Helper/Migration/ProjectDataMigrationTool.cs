using System.Collections.Generic;
using Mocassin.Framework.Extensions;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Helper.Migration
{
    /// <summary>
    ///     Base class for implementations of project data migration helper tools
    /// </summary>
    public abstract class ProjectDataMigrationTool
    {
        /// <summary>
        ///     Defines the delegate for reporting data migrations of a known <see cref="ProjectDataObject" /> parent pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="comment"></param>
        public delegate void DataMigrationReporter(ProjectDataObject source, ProjectDataObject target, string comment);

        /// <summary>
        ///     Get the <see cref="List{T}" /> of <see cref="MigrationReportEntry" /> instances
        /// </summary>
        private List<MigrationReportEntry> ReportEntries { get; }

        /// <summary>
        ///     Get or set a boolean flag if the system should report redundant data migrations where target and source already
        ///     match
        /// </summary>
        public bool IsRedundantReportEnabled { get; set; }

        /// <summary>
        ///     Creates a new <see cref="ProjectDataMigrationTool" /> with empty report list
        /// </summary>
        protected ProjectDataMigrationTool()
        {
            ReportEntries = new List<MigrationReportEntry>();
        }

        /// <summary>
        ///     Generates a new <see cref="MigrationReport" /> from the stored report entries and a source and target information
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected MigrationReport GenerateReport(ProjectDataObject source, ProjectDataObject target) =>
            new MigrationReport(ReportEntries.ToList(ReportEntries.Count).AsReadOnly(), source, target);

        /// <summary>
        ///     Resets the tool and empties the stored report information
        /// </summary>
        public void Reset()
        {
            ReportEntries.Clear();
        }

        /// <summary>
        ///     Adds information about a migration success to the report entries
        /// </summary>
        /// <param name="sourceParent"></param>
        /// <param name="targetParent"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataTarget"></param>
        /// <param name="comment"></param>
        protected void AddReportEntry(ProjectDataObject sourceParent, ProjectDataObject targetParent, ProjectDataObject dataSource,
            ProjectDataObject dataTarget, string comment)
        {
            ReportEntries.Add(new MigrationReportEntry(sourceParent, targetParent, dataSource, dataTarget, comment));
        }

        /// <summary>
        ///     Get a reporter delegate associated with a specific pair of <see cref="ProjectDataObject" /> data parents
        /// </summary>
        /// <param name="sourceParent"></param>
        /// <param name="targetParent"></param>
        /// <returns></returns>
        protected DataMigrationReporter GetMigrationReporter(ProjectDataObject sourceParent, ProjectDataObject targetParent)
        {
            void ReportInternal(ProjectDataObject source, ProjectDataObject target, string comment)
            {
                AddReportEntry(sourceParent, targetParent, source, target, comment);
            }

            return ReportInternal;
        }
    }
}