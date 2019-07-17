using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
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
        private string filePath;
        private readonly UserFileSelectionSource fileSelectionSource;
        private int maxJobs;
        private int doneJobs;
        private LibraryBuildStatus buildStatus;

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> for all selectable <see cref="MocassinProjectBuildGraph" />
        ///     instances
        /// </summary>
        public CollectionControlViewModel<MocassinProjectBuildGraph> ProjectBuildGraphCollectionViewModel { get; }

        /// <summary>
        ///     Get a <see cref="ICommand" /> to request a file selection through the <see cref="UserFileSelectionSource" />
        /// </summary>
        public ICommand GetFileSelectionCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to write the translation database to the selected file target
        /// </summary>
        public ICommand WriteDatabaseCommand { get; }

        /// <summary>
        ///     Get or set the file name <see cref="string" /> that is used for project building
        /// </summary>
        public string FilePath
        {
            get => filePath;
            set => SetProperty(ref filePath, value);
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
        ///     Get or set the current <see cref="LibraryBuildStatus" />
        /// </summary>
        public LibraryBuildStatus BuildStatus
        {
            get => buildStatus;
            set => SetProperty(ref buildStatus, value);
        }

        /// <inheritdoc />
        public LocalProjectDeployControlViewModel(IMocassinProjectControl projectControl,
            CollectionControlViewModel<MocassinProjectBuildGraph> projectBuildGraphCollectionViewModel)
            : base(projectControl)
        {
            ProjectBuildGraphCollectionViewModel = projectBuildGraphCollectionViewModel;
            fileSelectionSource = UserFileSelectionSource.CreateForJobDbFiles();
            GetFileSelectionCommand = new RelayCommand(() => FilePath = fileSelectionSource.GetFileSelection());
            WriteDatabaseCommand = GetWriteDatabaseCommand();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            ProjectBuildGraphCollectionViewModel.SetCollection(contentSource?.ProjectBuildGraphs);
        }

        /// <summary>
        ///     Builds an <see cref="AsyncRelayCommand" /> to create and write the job database
        /// </summary>
        /// <returns></returns>
        private AsyncRelayCommand GetWriteDatabaseCommand()
        {
            bool CanExecute()
            {
                return !string.IsNullOrWhiteSpace(filePath) 
                       && ProjectBuildGraphCollectionViewModel.SelectedCollectionItem?.ProjectCustomizationGraph != null
                       && ProjectBuildGraphCollectionViewModel.SelectedCollectionItem.ProjectJobTranslationGraph != null;
            }

            async Task Execute()
            {
                var builder = new MocassinSimulationLibraryBuilder();
                builder.LibraryBuildStatusNotifications.Subscribe(x => BuildStatus = x, e => SendCallErrorMessage(e));
                builder.JobBuildCounterNotifications.Subscribe(UpdateBuildCountsOnMayorStep);

                var buildGraph = ProjectBuildGraphCollectionViewModel.SelectedCollectionItem;
                var result = await Task.Run(() => builder.BuildLibrary(buildGraph, FilePath, ProjectControl.CreateModelProject()));
                if (result != null) SendCallInfoMessage($"Simulations deployed @ {FilePath}");
            }

            return new AsyncRelayCommand(Execute, CanExecute);
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
    }
}