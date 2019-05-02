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

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}"/> for all selectable <see cref="MocassinProjectBuildGraph"/> instances
        /// </summary>
        public CollectionControlViewModel<MocassinProjectBuildGraph> ProjectBuildGraphCollectionViewModel { get; }

        /// <summary>
        ///     Get a <see cref="ICommand"/> to request a file selection through the <see cref="UserFileSelectionSource"/>
        /// </summary>
        public ICommand GetFileSelectionCommand { get; }

        public ICommand WriteDatabaseCommand { get; }

        /// <summary>
        ///     Get or set the file name <see cref="string"/> that is used for project building
        /// </summary>
        public string FilePath
        {
            get => filePath;
            set => SetProperty(ref filePath, value);
        }

        /// <inheritdoc />
        public LocalProjectDeployControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ProjectBuildGraphCollectionViewModel = new CollectionControlViewModel<MocassinProjectBuildGraph>();
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

        private ICommand GetWriteDatabaseCommand()
        {
            bool CanExecute()
            {
                return !string.IsNullOrWhiteSpace(filePath) && ProjectBuildGraphCollectionViewModel.SelectedCollectionItem != null;
            }

            async Task Execute()
            {
                var builder = new MocassinSimulationLibraryBuilder();
                builder.LibraryBuildStatusNotifications.Subscribe(x => { }, e => SendCallErrorMessage(e));
                await Task.Run(() => builder.BuildLibrary(ProjectBuildGraphCollectionViewModel.SelectedCollectionItem, FilePath,
                    ProjectControl.CreateModelProject()));
                SendCallInfoMessage($"Wrote simulation library to {FilePath}");
            }
            return new AsyncRelayCommand(Execute, CanExecute);
        }
    }
}