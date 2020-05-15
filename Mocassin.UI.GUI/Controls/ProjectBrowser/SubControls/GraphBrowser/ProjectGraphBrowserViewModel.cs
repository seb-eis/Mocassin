using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.GraphBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ProjectGraphBrowserView" />
    /// </summary>
    public class ProjectGraphBrowserViewModel : PrimaryControlViewModel
    {
        private ObservableCollection<MocassinProject> projectGraphs;
        private MocassinProject selectedProject;

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to remove the currently selected <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand DeleteSelectedProjectCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to duplicate the selected <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand DuplicateSelectedProjectCommand { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> of <see cref="MocassinProject" /> for the browser
        /// </summary>
        public ObservableCollection<MocassinProject> ProjectGraphs
        {
            get => projectGraphs;
            private set => SetProperty(ref projectGraphs, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="MocassinProject" />
        /// </summary>
        public MocassinProject SelectedProject
        {
            get => selectedProject;
            set => SetProperty(ref selectedProject, value);
        }

        /// <inheritdoc />
        public ProjectGraphBrowserViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            DeleteSelectedProjectCommand = new RelayCommand(() => DeleteProjectWithConfirmation(SelectedProject), () => SelectedProject != null);
            DuplicateSelectedProjectCommand = new RelayCommand(() => DuplicateProject(SelectedProject), () => SelectedProject != null);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ProjectGraphs = ProjectControl.ProjectGraphs;
            SelectedProject = null;
        }

        /// <summary>
        ///     Deletes a project from the project library with a confirmation request
        /// </summary>
        /// <param name="project"></param>
        protected void DeleteProjectWithConfirmation(MocassinProject project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            const string caption = "Confirmation - Delete project!";
            var message = $"Are you sure you want to delete the project '{project.ProjectName}' (ID = {project.ProjectGuid}) and all affiliated content?";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            projectGraphs.Remove(SelectedProject);
            PushInfoMessage($"Project {project.ProjectName} (ID = {project.ProjectGuid}) was deleted.");
            SelectedProject = null;
        }

        /// <summary>
        ///     Creates a deep copy of the provided <see cref="MocassinProject" /> and adds it to the project collection
        /// </summary>
        /// <param name="project"></param>
        protected void DuplicateProject(MocassinProject project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));

            const string caption = "Confirmation - Duplicate project!";
            var message =
                $"Should extended data (customizations/job definitions/build instructions) be deleted when duplicating '{project.ProjectName}' (ID = {project.ProjectGuid})?";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return;

            var projectCopy = (MocassinProject) project.DeepCopy();
            projectCopy.ProjectName = $"{projectCopy.ProjectName}(copy)";
            projectCopy.ProjectGuid = Guid.NewGuid().ToString();
            projectCopy.ProjectModelData.Parent = projectCopy;

            if (result == MessageBoxResult.Yes)
            {
                projectCopy.CustomizationTemplates.Clear();
                projectCopy.JobSetTemplates.Clear();
                projectCopy.SimulationDbBuildTemplates.Clear();
            }
            else
            {
                foreach (var item in projectCopy.CustomizationTemplates.Cast<ProjectChildEntity<MocassinProject>>()
                    .Concat(projectCopy.JobSetTemplates).Concat(projectCopy.SimulationDbBuildTemplates)) item.Parent = projectCopy;
            }

            ExecuteOnAppThread(() => ProjectGraphs.Add(projectCopy));
            PushInfoMessage($"Project {project.ProjectName} (ID = {project.ProjectGuid}) was copied.");
        }
    }
}