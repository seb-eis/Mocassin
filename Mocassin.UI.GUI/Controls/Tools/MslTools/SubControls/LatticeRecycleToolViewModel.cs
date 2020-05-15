using System;
using System.Diagnostics;
using Mocassin.Mathematics.Comparer;
using Mocassin.Tools.Evaluation.Helper;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.IO;

namespace Mocassin.UI.GUI.Controls.Tools.MslTools.SubControls
{
    /// <summary>
    ///     The <see cref="ToolViewModel"/> for recycling result lattice data from one simulation database as initial states for another
    /// </summary>
    public class LatticeRecycleToolViewModel : ToolViewModel
    {
        private string pathToExportMsl;
        private string pathToImportMsl;
        private bool canRecycle;
        private int importableCount;
        private int importedCount;

        /// <summary>
        ///     Get the <see cref="UserFileSelectionSource"/>
        /// </summary>
        private UserFileSelectionSource FileSelectionSource { get; }

        /// <summary>
        ///     Get or set the path to the exporting database
        /// </summary>
        public string PathToExportMsl
        {
            get => pathToExportMsl;
            set => SetProperty(ref pathToExportMsl, value, () => CanRecycle = false);
        }

        /// <summary>
        ///     Get or set the path to the importing database
        /// </summary>
        public string PathToImportMsl
        {
            get => pathToImportMsl;
            set => SetProperty(ref pathToImportMsl, value, () => CanRecycle = false);
        }

        /// <summary>
        ///     Get a boolean flag if the recycling is currently possible
        /// </summary>
        public bool CanRecycle
        {
            get => canRecycle;
            private set => SetProperty(ref canRecycle, value);
        }

        /// <summary>
        ///     Get the number of jobs that can be imported
        /// </summary>
        public int ImportableCount
        {
            get => importableCount;
            private set => SetProperty(ref importableCount, value);
        }

        /// <summary>
        ///     Get the number of jobs that have been imported
        /// </summary>
        public int ImportedCount
        {
            get => importedCount;
            private set => SetProperty(ref importedCount, value);
        }

        /// <summary>
        ///     Get the <see cref="AsyncRelayCommand"/> to refresh if the data can be imported using the current settings
        /// </summary>
        public AsyncRelayCommand RefreshCanImportCommand { get; }

        /// <summary>
        ///     Get the <see cref="AsyncRelayCommand"/> to run the import
        /// </summary>
        public AsyncRelayCommand RunImportCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand"/> to select the import path
        /// </summary>
        public RelayCommand SelectImportPathCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand"/> to select the export path
        /// </summary>
        public RelayCommand SelectExportPathCommand { get; }

        /// <summary>
        ///     Creates a new <see cref="LatticeRecycleToolViewModel"/>
        /// </summary>
        public LatticeRecycleToolViewModel()
        {
            FileSelectionSource = UserFileSelectionSource.CreateForJobDbFiles(false);
            RefreshCanImportCommand = AsyncRelayCommand.Create(CheckImportIsPossible, () => !RunImportCommand.IsExecuting && PathToExportMsl != null && PathToImportMsl != null);
            RunImportCommand = AsyncRelayCommand.Create(RunImport, () => !RefreshCanImportCommand.IsExecuting && CanRecycle);
            SelectImportPathCommand = new RelayCommand(() => PromptUserForMsl(x => PathToImportMsl = x), () => !RunImportCommand.IsExecuting && !RefreshCanImportCommand.IsExecuting);
            SelectExportPathCommand = new RelayCommand(() => PromptUserForMsl(x => PathToExportMsl = x), () => !RunImportCommand.IsExecuting && !RefreshCanImportCommand.IsExecuting);
        }

        /// <summary>
        ///     Checks if the two currently set MSL file paths are compatible
        /// </summary>
        /// <returns></returns>
        private void CheckImportIsPossible()
        {
            ImportedCount = 0;
            if (PathToImportMsl != null && PathToImportMsl == PathToExportMsl)
            {
                CanRecycle = false;
                SendMessage("It is currently not supported to repopulate an MSL file by its own result contents.");
                return;
            }
            using var importer = CreateImporter();
            CanRecycle = importer.DataCanBeMapped(PathToExportMsl, PathToImportMsl, out var count);
            ImportableCount = count;
            SendMessage($"Data migration from '{PathToExportMsl}' to '{pathToImportMsl}' is {(CanRecycle ? "possible":"not possible")} ({count} items mapped).");
        }

        /// <summary>
        ///     Executes the actual data import
        /// </summary>
        private void RunImport()
        {
            if (!CanRecycle) throw new InvalidOperationException("Cannot start importing when the recycle flag is false.");
            using var importer = CreateImporter();
            SendMessage($"Migrating {ImportableCount} items from '{PathToExportMsl}' to '{pathToImportMsl}'");
            var watch = Stopwatch.StartNew();
            importer.ImportFinalLatticesAsInitialLattices(PathToExportMsl, PathToImportMsl);
            SendMessage($"Recycling complete [{watch.Elapsed}]!");
        }

        /// <summary>
        ///     Creates a new <see cref="ResultLatticeImporter"/>
        /// </summary>
        /// <returns></returns>
        private ResultLatticeImporter CreateImporter()
        {
            var result = new ResultLatticeImporter(NumericComparer.Default());
            result.ImportCountNotifications.Subscribe(x => ImportedCount = x);
            return result;
        }

        /// <summary>
        ///     Requests a file selection from the user and sets it using the provided setter
        /// </summary>
        /// <param name="setter"></param>
        private void PromptUserForMsl(Action<string> setter)
        {
            var value = FileSelectionSource.RequestFileSelection(true);
            setter.Invoke(value);
        }
    }
}