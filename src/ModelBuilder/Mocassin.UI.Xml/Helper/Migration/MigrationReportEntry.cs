using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Helper.Migration
{
    /// <summary>
    ///     Represents a single entry of a <see cref="MigrationReport" /> that informs about a single data migration success
    /// </summary>
    public class MigrationReportEntry
    {
        /// <summary>
        ///     Get a comment <see cref="string" /> that describes what was done in the migration
        /// </summary>
        public string Comment { get; }

        /// <summary>
        ///     Get the <see cref="ProjectDataObject" /> parent of the migration source if it exists
        /// </summary>
        public ProjectDataObject SourceParent { get; }

        /// <summary>
        ///     Get the <see cref="ProjectDataObject" /> parent of the migration target if it exists
        /// </summary>
        public ProjectDataObject TargetParent { get; }

        /// <summary>
        ///     Get the migration data source <see cref="ProjectDataObject" />
        /// </summary>
        public ProjectDataObject DataSource { get; }

        /// <summary>
        ///     Get the migration data target <see cref="ProjectDataObject" />
        /// </summary>
        public ProjectDataObject DataTarget { get; }

        /// <summary>
        ///     Creates a new <see cref="MigrationReportEntry" />
        /// </summary>
        /// <param name="sourceParent"></param>
        /// <param name="targetParent"></param>
        /// <param name="dataSource"></param>
        /// <param name="dataTarget"></param>
        /// <param name="comment"></param>
        public MigrationReportEntry(ProjectDataObject sourceParent, ProjectDataObject targetParent, ProjectDataObject dataSource, ProjectDataObject dataTarget,
            string comment)
        {
            Comment = comment;
            SourceParent = sourceParent;
            TargetParent = targetParent;
            DataSource = dataSource;
            DataTarget = dataTarget;
        }
    }
}