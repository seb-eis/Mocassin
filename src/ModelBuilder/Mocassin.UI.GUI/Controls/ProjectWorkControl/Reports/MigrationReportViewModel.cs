using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Data.Helper.Migration;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.Reports
{
    /// <summary>
    ///     A <see cref="ViewModelBase" /> implementation for the <see cref="MigrationReportView" /> that displays
    /// </summary>
    public class MigrationReportViewModel : ViewModelBase
    {
        private string description;
        private MigrationReport report;
        private MigrationReportEntry selectedEntry;

        /// <summary>
        ///     Get or set a short report description <see cref="string" />
        /// </summary>
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <summary>
        ///     Get or set the active <see cref="MigrationReport" />
        /// </summary>
        public MigrationReport Report
        {
            get => report;
            set => SetProperty(ref report, value, OnMigrationReportChanged);
        }

        /// <summary>
        ///     Get or set the currently selected <see cref="MigrationReportEntry" />
        /// </summary>
        public MigrationReportEntry SelectedEntry
        {
            get => selectedEntry;
            set => SetProperty(ref selectedEntry, value);
        }

        /// <summary>
        ///     Action that is called after the <see cref="Report" /> property changed
        /// </summary>
        private void OnMigrationReportChanged()
        {
            Description = $"Migrated {Report.Entries.Count} items from [{Report.MigrationSource}] to [{Report.MigrationTarget}]:";
        }
    }
}