﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.IO;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectBuilding;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> for <see cref="LocalProjectDeployControlView" /> that controls simulation database
    ///     file definition and creation
    /// </summary>
    public class LocalProjectDeployControlViewModel : ProjectGraphControlViewModel
    {
        private string buildTargetFilePath;
        private readonly UserFileSelectionSource fileSelectionSource;
        private int maxJobs;
        private int doneJobs;
        private LibraryBuildStatus buildStatus;
        private bool isManualLibrarySaving;

        /// <summary>
        ///     Get or set the last build <see cref="ISimulationLibrary" />
        /// </summary>
        private ISimulationLibrary BuildSimulationLibrary { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CancellationTokenSource" /> for the build process
        /// </summary>
        private CancellationTokenSource BuildCancellationTokenSource { get; set; }

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for all selectable <see cref="SimulationDbBuildTemplate" />
        ///     instances
        /// </summary>
        public CollectionControlViewModel<SimulationDbBuildTemplate> ProjectBuildGraphCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for selection of meta data information
        /// </summary>
        public ObservableCollectionViewModel<JobMetaDataEntity> JobMetaDataCollectionControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for progress console message <see cref="string" /> values
        ///     with time info
        /// </summary>
        public ObservableCollectionViewModel<Tuple<DateTime, string>> LogConsoleMessages { get; }

        /// <summary>
        ///     Get a <see cref="ICommand" /> to request a file selection through the <see cref="UserFileSelectionSource" />
        /// </summary>
        public ICommand GetFileSelectionCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to write the translation database to the selected file target
        /// </summary>
        public ICommand WriteDatabaseCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to write the translation database to the selected file target
        /// </summary>
        public ICommand CancelCurrentBuildCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to manually save the last build library
        /// </summary>
        public ICommand ManualSaveLastLibraryCommand { get; }

        /// <summary>
        ///     Get or set the file name <see cref="string" /> that is used for project building
        /// </summary>
        public string BuildTargetFilePath
        {
            get => buildTargetFilePath;
            set => SetProperty(ref buildTargetFilePath, value);
        }

        /// <summary>
        ///     Get or set the maximum number of jobs
        /// </summary>
        public int MaxJobs
        {
            get => maxJobs;
            set => SetProperty(ref maxJobs, value);
        }

        /// <summary>
        ///     Get or set the maximum number of translated jobs
        /// </summary>
        public int DoneJobs
        {
            get => doneJobs;
            set => SetProperty(ref doneJobs, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the library saving should be done manually
        /// </summary>
        public bool IsManualLibrarySaving
        {
            get => isManualLibrarySaving;
            set => SetProperty(ref isManualLibrarySaving, value);
        }

        /// <summary>
        ///     Get or set the current <see cref="LibraryBuildStatus" />
        /// </summary>
        public LibraryBuildStatus BuildStatus
        {
            get => buildStatus;
            set => SetProperty(ref buildStatus, value);
        }

        /// <inheritdoc />
        public LocalProjectDeployControlViewModel(IMocassinProjectControl projectControl,
            CollectionControlViewModel<SimulationDbBuildTemplate> projectBuildGraphCollectionViewModel)
            : base(projectControl)
        {
            ProjectBuildGraphCollectionViewModel = projectBuildGraphCollectionViewModel;
            LogConsoleMessages = new ObservableCollectionViewModel<Tuple<DateTime, string>>(1000);
            fileSelectionSource = UserFileSelectionSource.CreateForJobDbFiles(true);
            GetFileSelectionCommand = new RelayCommand(() => BuildTargetFilePath = fileSelectionSource.GetFileSelection());
            WriteDatabaseCommand = GetWriteDatabaseCommand();
            JobMetaDataCollectionControlViewModel = new ObservableCollectionViewModel<JobMetaDataEntity>();
            PropertyChanged += OnLibraryStatusChanged;
            CancelCurrentBuildCommand = GetCancelBuildCommand();
            ManualSaveLastLibraryCommand = GetManualSaveLibraryCommand();
            AddConsoleMessage(GetStartupMessage());
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            ProjectBuildGraphCollectionViewModel.SetCollection(contentSource?.SimulationDbBuildTemplates);
        }

        /// <summary>
        ///     Get a <see cref="AsyncRelayCommand" /> to manually save the last <see cref="ISimulationLibrary" /> to its target
        ///     file
        /// </summary>
        /// <returns></returns>
        private AsyncRelayCommand GetManualSaveLibraryCommand()
        {
            void Execute()
            {
                BuildStatus = LibraryBuildStatus.SavingLibraryContents;
                try
                {
                    BuildSimulationLibrary.SaveChanges();
                    AddConsoleMessage("Reloading meta information table.");
                    JobMetaDataCollectionControlViewModel.Clear();
                    JobMetaDataCollectionControlViewModel.AddItems(BuildSimulationLibrary.JobMetaData.Local);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    AddConsoleError(exception);
                }

                BuildStatus = LibraryBuildStatus.Unknown;
            }

            bool CanExecute()
            {
                return BuildSimulationLibrary != null;
            }

            return new AsyncRelayCommand(() => Task.Run(Execute), CanExecute);
        }

        /// <summary>
        ///     Get a startup message <see cref="string" /> for the console
        /// </summary>
        /// <returns></returns>
        private string GetStartupMessage()
        {
            return $"Async build with {Environment.ProcessorCount} cores.";
        }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to cancel the build process
        /// </summary>
        /// <returns></returns>
        private RelayCommand GetCancelBuildCommand()
        {
            bool CanExecute()
            {
                return BuildCancellationTokenSource != null && !BuildCancellationTokenSource.IsCancellationRequested;
            }

            return new RelayCommand(() => BuildCancellationTokenSource.Cancel(), CanExecute);
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRelayCommand" /> to create and write the job database
        /// </summary>
        /// <returns></returns>
        private AsyncRelayCommand GetWriteDatabaseCommand()
        {
            bool CanExecute()
            {
                return !string.IsNullOrWhiteSpace(buildTargetFilePath)
                       && BuildCancellationTokenSource == null
                       && ProjectBuildGraphCollectionViewModel.SelectedItem?.ProjectCustomizationTemplate != null
                       && ProjectBuildGraphCollectionViewModel.SelectedItem.ProjectJobSetTemplate != null;
            }

            return new AsyncRelayCommand(StartBuildProcess, CanExecute);
        }

        /// <summary>
        ///     Executes the build process with the current status of the object
        /// </summary>
        private async Task StartBuildProcess()
        {
            BuildCancellationTokenSource = new CancellationTokenSource();
            AddConsoleMessage($"Deployment start: {BuildTargetFilePath}");
            using var builder = new SimulationLibraryBuilder {IsAutoSaveAfterBuild = !IsManualLibrarySaving};
            BuildSimulationLibrary?.Dispose();
            BuildSimulationLibrary = null;

            builder.LibraryBuildStatusNotifications.Subscribe(x => BuildStatus = x, AddConsoleError, () => DoneJobs = MaxJobs);
            builder.JobBuildCounterNotifications.Subscribe(UpdateBuildCountsOnMayorStep);

            var buildGraph = ProjectBuildGraphCollectionViewModel.SelectedItem;
            var cancellationToken = BuildCancellationTokenSource.Token;
            BuildSimulationLibrary = await Task.Run(()
                => builder.BuildLibrary(buildGraph, BuildTargetFilePath, ProjectControl.CreateModelProject(), cancellationToken), cancellationToken);
            if (BuildSimulationLibrary != null)
            {
                AddConsoleMessage("Loading meta information table.");
                JobMetaDataCollectionControlViewModel.Clear();
                JobMetaDataCollectionControlViewModel.AddItems(BuildSimulationLibrary.JobMetaData.Local);
                AddConsoleMessage($"Successfully created at [{(IsManualLibrarySaving ? "MEMORY" : BuildTargetFilePath)}]");
            }
            else
                AddConsoleMessage($"Creation failed! ({(cancellationToken.IsCancellationRequested ? "Cancelled" : "Error")})");

            BuildCancellationTokenSource.Dispose();
            BuildCancellationTokenSource = null;
        }

        /// <summary>
        ///     Adds a standard formatted message to the progress console
        /// </summary>
        /// <param name="message"></param>
        private void AddConsoleMessage(string message)
        {
            LogConsoleMessages.AddItem(Tuple.Create(DateTime.Now, message));
        }

        /// <summary>
        ///     Add a standard formatted <see cref="Exception" /> message to the progress console
        /// </summary>
        /// <param name="exception"></param>
        private void AddConsoleError(Exception exception)
        {
            AddConsoleMessage($"Exception: {exception.GetType()}");
        }

        /// <summary>
        ///     Updates the build counter values if the passed set of done and total counts describes a mayor step
        /// </summary>
        /// <param name="counts"></param>
        private void UpdateBuildCountsOnMayorStep((int, int) counts)
        {
            var (done, total) = counts;
            var div = total / 100;
            div = div == 0 ? 1 : div;
            if (!(done == 1 || done == total || done % div == 0)) return;
            MaxJobs = total;
            DoneJobs = done;
        }

        /// <summary>
        ///     Action that is called when the <see cref="BuildStatus" /> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnLibraryStatusChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(BuildStatus)) AddConsoleMessage($"Build status changed to {BuildStatus}.");
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            JobMetaDataCollectionControlViewModel.Clear();
            BuildSimulationLibrary?.Dispose();
            base.Dispose();
        }
    }
}