using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.Content.Commands;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Customization;
using Mocassin.UI.Data.Jobs;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.ComponentBrowser
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ProjectComponentBrowserView" />
    /// </summary>
    public class ProjectComponentBrowserViewModel : PrimaryControlViewModel
    {
        private MocassinProject activeProject;
        private ObservableCollection<MocassinProject> mocassinProjects;

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to remove the passed <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<MocassinProject> DeleteProjectCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to duplicate the passed <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<MocassinProject> DuplicateProjectCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to set a <see cref="MocassinProject" /> as the active work project
        /// </summary>
        public RelayCommand<MocassinProject> SetAsActiveProjectCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to delete a <see cref="ProjectCustomizationTemplate" /> from its parent
        ///     <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<ProjectCustomizationTemplate> DeleteCustomizationTemplateCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to duplicate a <see cref="ProjectCustomizationTemplate" /> on its parent
        ///     <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<ProjectCustomizationTemplate> DuplicateCustomizationTemplateCommand { get; }

        /// <summary>
        ///     Get a <see cref="MigrateCustomizationCommand" /> to migrate an existing <see cref="ProjectCustomizationTemplate" />
        ///     on its parent <see cref="MocassinProject" />
        /// </summary>
        public MigrateCustomizationCommand MigrateCustomizationTemplateCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to delete a <see cref="ProjectJobSetTemplate" /> from its parent
        ///     <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<ProjectJobSetTemplate> DeleteJobSetTemplateCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to duplicate a <see cref="ProjectJobSetTemplate" /> on its parent
        ///     <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<ProjectJobSetTemplate> DuplicateJobSetTemplateCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to add a new <see cref="ProjectCustomizationTemplate" /> to a
        ///     <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<MocassinProject> AddCustomizationTemplateToProjectCommand { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand" /> to add a new <see cref="ProjectJobSetTemplate" /> to a
        ///     <see cref="MocassinProject" />
        /// </summary>
        public RelayCommand<MocassinProject> AddJobSetTemplateToProjectCommand { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> of <see cref="MocassinProject" /> for the browser
        /// </summary>
        public ObservableCollection<MocassinProject> MocassinProjects
        {
            get => mocassinProjects;
            private set => SetProperty(ref mocassinProjects, value);
        }

        /// <summary>
        ///     Get or set the active <see cref="MocassinProject" />
        /// </summary>
        public MocassinProject ActiveProject
        {
            get => activeProject;
            set => SetProperty(ref activeProject, value);
        }

        /// <inheritdoc />
        public ProjectComponentBrowserViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            DeleteProjectCommand = RelayCommand
                .Create((MocassinProject project) => DeleteProjectWithConfirmation(project), project => project != null);
            DuplicateProjectCommand = RelayCommand
                .Create((MocassinProject project) => DuplicateProject(project), project => project != null);
            SetAsActiveProjectCommand = RelayCommand
                .Create((MocassinProject project) => SetProjectAsActive(project), project => project != null);
            DeleteCustomizationTemplateCommand = RelayCommand
                .Create((ProjectCustomizationTemplate template) => RemoveTemplateFromParent(template), template => template != null);
            DeleteJobSetTemplateCommand = RelayCommand
                .Create((ProjectJobSetTemplate template) => RemoveTemplateFromParent(template), template => template != null);
            DuplicateJobSetTemplateCommand = RelayCommand
                .Create((ProjectJobSetTemplate template) => DuplicateTemplateOnParent(template), template => template != null);
            DuplicateCustomizationTemplateCommand = RelayCommand
                .Create((ProjectCustomizationTemplate template) => DuplicateTemplateOnParent(template), template => template != null);
            MigrateCustomizationTemplateCommand = new MigrateCustomizationCommand(ProjectControl);
            AddCustomizationTemplateToProjectCommand = RelayCommand
                .Create((MocassinProject project) => AddNewCustomizationTemplateToProject(project), project => project != null);
            AddJobSetTemplateToProjectCommand = RelayCommand
                .Create((MocassinProject project) => AddNewJobSetTemplateToProject(project), project => project != null);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            MocassinProjects = ProjectControl.ProjectGraphs;
            ActiveProject = MocassinProjects?.FirstOrDefault(project => project.IsActiveProject);
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

            mocassinProjects.Remove(project);
            PushInfoMessage($"Project {project.ProjectName} (ID = {project.ProjectGuid}) was deleted.");
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
            projectCopy.IsActiveProject = false;

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

            ExecuteOnAppThread(() => MocassinProjects.Add(projectCopy));
            PushInfoMessage($"Project {project.ProjectName} (ID = {project.ProjectGuid}) was copied.");
        }

        /// <summary>
        ///     Removes a <see cref="ProjectCustomizationTemplate" /> from its parent <see cref="MocassinProject" />
        /// </summary>
        /// <param name="template"></param>
        protected void RemoveTemplateFromParent(ProjectCustomizationTemplate template)
        {
            template.Parent.CustomizationTemplates.Remove(template);
        }

        /// <summary>
        ///     Removes a <see cref="ProjectJobSetTemplate" /> from its parent <see cref="MocassinProject" />
        /// </summary>
        /// <param name="template"></param>
        protected void RemoveTemplateFromParent(ProjectJobSetTemplate template)
        {
            template.Parent.JobSetTemplates.Remove(template);
        }

        /// <summary>
        ///     Duplicates a <see cref="ProjectCustomizationTemplate" /> on its parent <see cref="MocassinProject" />
        /// </summary>
        /// <param name="template"></param>
        protected void DuplicateTemplateOnParent(ProjectCustomizationTemplate template)
        {
            template.Parent.CustomizationTemplates.Add(template.Duplicate());
        }

        /// <summary>
        ///     Duplicates a <see cref="ProjectJobSetTemplate" /> on its parent <see cref="MocassinProject" />
        /// </summary>
        /// <param name="template"></param>
        protected void DuplicateTemplateOnParent(ProjectJobSetTemplate template)
        {
            template.Parent.JobSetTemplates.Add(template.Duplicate());
        }

        /// <summary>
        ///     Adds a new <see cref="ProjectCustomizationTemplate" /> to a <see cref="MocassinProject" /> if possible
        /// </summary>
        /// <param name="project"></param>
        protected void AddNewCustomizationTemplateToProject(MocassinProject project)
        {
            var command = new AddNewCustomizationCommand(ProjectControl, () => project);
            try
            {
                command.Execute(project);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Adds a new <see cref="ProjectJobSetTemplate" /> to a <see cref="MocassinProject" /> if possible
        /// </summary>
        /// <param name="project"></param>
        protected void AddNewJobSetTemplateToProject(MocassinProject project)
        {
            var command = new AddNewJobSetTemplateCommand(ProjectControl, () => project);
            try
            {
                command.Execute(project);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     Sets a new <see cref="MocassinProject" /> as tze active project and deactivates the old active one
        /// </summary>
        /// <param name="project"></param>
        protected void SetProjectAsActive(MocassinProject project)
        {
            if (ReferenceEquals(activeProject, project)) return;
            if (ActiveProject != null) ActiveProject.IsActiveProject = false;
            project.IsActiveProject = true;
            ActiveProject = project;
        }
    }
}