using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Mocassin.Tools.Evaluation.Custom.Mmcfe.Importer;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.IO;

namespace Mocassin.UI.GUI.Controls.Tools.MslTools.SubControls
{
    /// <summary>
    ///     The <see cref="ToolViewModel" /> for <see cref="MmcfeImportToolView" /> that performs the MMCFE routine raw data
    ///     import tasks
    /// </summary>
    public class MmcfeImportToolViewModel : ToolViewModel
    {
        private int importableCount;
        private int importedCount;
        private int importsPerSave = 50;
        private bool isAutoSelectRootByMslPath = true;
        private bool isDeleteJobFoldersActive;
        private bool isImporting;
        private bool isIndeterminateProgress;
        private bool isZipRawActive;
        private string jobFolderRootPath;
        private string mslFilePath;

        /// <summary>
        ///     Get the lock object for access
        /// </summary>
        private object AccessLock { get; } = new object();

        /// <summary>
        ///     Get the number of importable MMCFE entries
        /// </summary>
        public int ImportableCount
        {
            get => importableCount;
            private set => SetProperty(ref importableCount, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if an import is in progress
        /// </summary>
        public bool IsImporting
        {
            get => isImporting;
            set => SetProperty(ref isImporting, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the import is currently in state where progress cannot be reported
        /// </summary>
        public bool IsIndeterminateProgress
        {
            get => isIndeterminateProgress;
            set => SetProperty(ref isIndeterminateProgress, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the msl path is used tol determine the job root path
        /// </summary>
        public bool IsAutoSelectRootByMslPath
        {
            get => isAutoSelectRootByMslPath;
            set => SetProperty(ref isAutoSelectRootByMslPath, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the raw import database should be zipped
        /// </summary>
        public bool IsZipRawActive
        {
            get => isZipRawActive;
            set => SetProperty(ref isZipRawActive, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the job folders should be removed during importing
        /// </summary>
        public bool IsDeleteJobFoldersActive
        {
            get => isDeleteJobFoldersActive;
            set => SetProperty(ref isDeleteJobFoldersActive, value);
        }

        /// <summary>
        ///     Get the number of already imported MMCFE entries
        /// </summary>
        public int ImportedCount
        {
            get => importedCount;
            private set => SetProperty(ref importedCount, value);
        }

        /// <summary>
        ///     Get or set the number of imports between save actions
        /// </summary>
        public int ImportsPerSave
        {
            get => importsPerSave;
            set => SetProperty(ref importsPerSave, value < 0 ? 1 : value);
        }

        /// <summary>
        ///     Get or set the path to the simulation library that supplies the meta data
        /// </summary>
        public string MslFilePath
        {
            get => mslFilePath;
            set => SetProperty(ref mslFilePath, value, OnMslPathChanged);
        }

        /// <summary>
        ///     Get or set the root path of the job folders
        /// </summary>
        public string JobFolderRootPath
        {
            get => jobFolderRootPath;
            set => SetProperty(ref jobFolderRootPath, value);
        }

        /// <summary>
        ///     Get a <see cref="VoidParameterCommand" /> so select the msl path
        /// </summary>
        public VoidParameterCommand SelectMslPathCommand { get; }

        /// <summary>
        ///     Get a <see cref="VoidParameterCommand" /> to select the job root path
        /// </summary>
        public VoidParameterCommand SelectJobRootPathCommand { get; }

        /// <summary>
        ///     Get an <see cref="AsyncVoidParameterCommand" /> to run the import
        /// </summary>
        public AsyncVoidParameterCommand RunImportCommand { get; }

        /// <inheritdoc />
        public MmcfeImportToolViewModel()
        {
            SelectMslPathCommand = new RelayCommand(PromptMslPathSelection, () => !IsImporting);
            SelectJobRootPathCommand = new RelayCommand(PromptRootPathSelection, () => !IsImporting && !IsAutoSelectRootByMslPath);
            RunImportCommand = AsyncRelayCommand.Create(RunImport, CanRunImport);
        }

        /// <summary>
        ///     Check if the import can currently be done
        /// </summary>
        /// <returns></returns>
        public bool CanRunImport() => !IsImporting && File.Exists(MslFilePath) && Directory.Exists(JobFolderRootPath);

        /// <summary>
        ///     Runs the import action using the current settings
        /// </summary>
        public void RunImport()
        {
            if (IsImporting) throw new InvalidOperationException("Import already in progress");
            using var importer = new MmcfeJobFolderImporter(MslFilePath, JobFolderRootPath)
            {
                ImportsPerSave = ImportsPerSave,
                IsDeleteJobFolders = IsDeleteJobFoldersActive
            };
            try
            {
                var watch = Stopwatch.StartNew();
                IsImporting = true;
                IsIndeterminateProgress = false;
                ImportedCount = 0;
                ImportableCount = importer.CountImportableEntries();
                importer.MessageNotifications.Subscribe(LogMessage);
                importer.JobImportedNotification.Subscribe(OnJobImported, OnJobImportError);
                importer.RunImport(IsZipRawActive);
                LogMessage($"Imported completed! [{watch.Elapsed}]");
            }
            catch (Exception e)
            {
                LogMessage($"An unhandled exception occured while importing: {e}");
            }
            finally
            {
                IsImporting = false;
                IsIndeterminateProgress = false;
            }
        }

        /// <summary>
        ///     Action that is called when a job is imported
        /// </summary>
        /// <param name="jobId"></param>
        private void OnJobImported(int jobId)
        {
            lock (AccessLock)
            {
                ImportedCount++;
                IsIndeterminateProgress = ImportedCount == ImportableCount;
            }
        }

        /// <summary>
        ///     Action that is called if a job import fails
        /// </summary>
        /// <param name="e"></param>
        private void OnJobImportError(Exception e)
        {
            LogMessage($"Error on job import {e}.");
        }

        /// <summary>
        ///     Action that is called when the msl path changes
        /// </summary>
        private void OnMslPathChanged()
        {
            if (IsAutoSelectRootByMslPath && File.Exists(MslFilePath)) JobFolderRootPath = Directory.GetParent(MslFilePath)?.FullName;
        }

        /// <summary>
        ///     Prompts the user to select the msl path
        /// </summary>
        private void PromptMslPathSelection()
        {
            var source = UserFileSelectionSource.CreateForJobDbFiles(false);
            MslFilePath = source.RequestFileSelection(true);
        }

        /// <summary>
        ///     Prompts the user to select the job root folder path
        /// </summary>
        private void PromptRootPathSelection()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            JobFolderRootPath = dialog.SelectedPath;
        }
    }
}