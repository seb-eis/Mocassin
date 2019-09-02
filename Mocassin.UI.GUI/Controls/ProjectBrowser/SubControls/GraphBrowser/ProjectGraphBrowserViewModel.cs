using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.GraphBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ProjectGraphBrowserView" />
    /// </summary>
    public class ProjectGraphBrowserViewModel : PrimaryControlViewModel
    {
        private MocassinProjectGraph selectedProject;
        private ObservableCollection<MocassinProjectGraph> projectGraphs;

        /// <summary>
        ///     Get a <see cref="RelayCommand"/> to remove the currently selected <see cref="MocassinProjectGraph"/>
        /// </summary>
        public RelayCommand DeleteSelectedProjectCommand { get; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="MocassinProjectGraph" /> for the browser
        /// </summary>
        public ObservableCollection<MocassinProjectGraph> ProjectGraphs
        {
            get => projectGraphs;
            private set => SetProperty(ref projectGraphs, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="MocassinProjectGraph" />
        /// </summary>
        public MocassinProjectGraph SelectedProject
        {
            get => selectedProject;
            set => SetProperty(ref selectedProject, value);
        }

        /// <inheritdoc />
        public ProjectGraphBrowserViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            DeleteSelectedProjectCommand =
                new RelayCommand(() => DeleteProjectWithConfirmation(SelectedProject), () => SelectedProject != null);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ProjectGraphs = ProjectControl.ProjectGraphs;
        }

        /// <summary>
        ///     Deletes a project from the project library with a confirmation request
        /// </summary>
        /// <param name="project"></param>
        protected void DeleteProjectWithConfirmation(MocassinProjectGraph project)
        {
            if (project == null) return;
            var caption = $"Confirmation - Delete project!";
            var message =
                $"Are you sure you want to delete the project '{project.ProjectName}' (ID = {project.ProjectGuid}) and all affiliated content? This action is irreversible as soon as the project is saved!";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            projectGraphs.Remove(SelectedProject);
            SendCallInfoMessage($"Project {project.ProjectName} (ID = {project.ProjectGuid}) was deleted.");
            SelectedProject = null;
        }
    }
}