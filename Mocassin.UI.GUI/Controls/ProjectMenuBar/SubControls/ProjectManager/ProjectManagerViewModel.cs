using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.SQLiteCore;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ProjectManagerView" /> that controls loading and
    ///     saving of <see cref="IMocassinProjectLibrary" />
    /// </summary>
    public class ProjectManagerViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="OpenDatabaseFilePath" /> backing field
        /// </summary>
        private string openDatabaseFilePath;

        /// <summary>
        ///     Get the file path <see cref="string" /> of the currently loaded <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public string OpenDatabaseFilePath
        {
            get => openDatabaseFilePath;
            private set => SetProperty(ref openDatabaseFilePath, value);
        }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to load a <see cref="IMocassinProjectLibrary" /> file location
        /// </summary>
        public ICommand OpenProjectLibraryCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to close the active <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public ICommand CloseProjectLibraryCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to create and open a <see cref="IMocassinProjectLibrary" /> file location
        /// </summary>
        public ICommand CreateProjectLibraryCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to save all pending changes on the open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public ICommand SaveProjectLibraryChangesCommand { get; }

        /// <inheritdoc />
        public ProjectManagerViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            OpenProjectLibraryCommand = new OpenProjectLibraryCommand(projectControl);
            CloseProjectLibraryCommand = new CloseProjectLibraryCommand(projectControl);
            CreateProjectLibraryCommand = new CreateProjectLibraryCommand(projectControl);
            SaveProjectLibraryChangesCommand = new SaveProjectLibraryChangesCommand(projectControl);
        }

        /// <summary>
        ///     Closes the currently loaded <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public void CloseActiveProjectLibrary()
        {
            if (TryCloseProjectLibrary(ProjectControl.OpenProjectLibrary))
            {
                ProjectControl.ChangeOpenProjectLibrary(null);
                PushInfoMessage("Project closed!");
            }
            else
                PushInfoMessage("Project close aborted!");
        }

        /// <summary>
        ///     Saves the current changes in the open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public void SaveActiveProjectLibraryChanges()
        {
            if (ProjectControl.OpenProjectLibrary == null)
            {
                PushWarningMessage("Cannot save changes when no project is loaded!");
                return;
            }

            try
            {
                ProjectControl.OpenProjectLibrary.SaveChanges();
                PushInfoMessage("Project changes saved!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var exception = new InvalidOperationException("Internal error on saving", e);
                PushErrorMessage(exception);
            }
        }

        /// <summary>
        ///     Load a <see cref="IMocassinProjectLibrary" /> from the passed filepath <see cref="string" /> and sets it as the
        ///     active project library
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadActiveProjectLibrary(string filePath)
        {
            if (!File.Exists(filePath))
            {
                PushErrorMessage(new FileNotFoundException("Requested file does not exist!", filePath));
                return;
            }

            if (!TryOpenProjectLibrary(filePath, false, out var exception))
                PushErrorMessage(exception);
            else
                PushInfoMessage($"Project loaded from: {filePath}");
        }

        /// <summary>
        ///     Creates a new <see cref="IMocassinProjectLibrary" /> at the passed filepath <see cref="string" /> and sets it as
        ///     the active project library
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateNewActiveProjectLibrary(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                PushWarningMessage("Aborted due to null or white space creation path!");
                return;
            }
            if (File.Exists(filePath))
            {
                PushErrorMessage(new FileNotFoundException("Requested file already exists!", filePath));
                return;
            }

            if (!TryOpenProjectLibrary(filePath, true, out var exception))
                PushErrorMessage(exception);
            else
                PushInfoMessage($"Project created at: {filePath}");
        }

        /// <summary>
        ///     Creates a new <see cref="MocassinProject" /> and adds it to the passed <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <param name="projectLibrary"></param>
        public void AddNewProjectGraphToProject(IMocassinProjectLibrary projectLibrary)
        {
            if (projectLibrary == null)
            {
                PushWarningMessage("Cannot add project graph. Library is missing.");
                return;
            }

            try
            {
                var projectGraph = MocassinProject.CreateNew();
                projectLibrary.Add(projectGraph);
                PushInfoMessage($"New project graph (ID = {projectGraph.ProjectGuid}) added to project");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var exception = new InvalidOperationException("Internal error during graph adding!", e);
                PushErrorMessage(exception);
            }
        }

        /// <summary>
        ///     Tries to open or drop create a <see cref="IMocassinProjectLibrary" /> at the given location. Returns null on
        ///     success
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dropCreate"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private bool TryOpenProjectLibrary(string filePath, bool dropCreate, out Exception exception)
        {
            IMocassinProjectLibrary newProjectLibrary = default;
            try
            {
                TryCloseProjectLibrary(ProjectControl.OpenProjectLibrary);
                newProjectLibrary = SqLiteContext.OpenDatabase<MocassinProjectContext>(filePath, dropCreate);
                LoadLibraryContents(newProjectLibrary);
                ProjectControl.ChangeOpenProjectLibrary(newProjectLibrary);
                OpenDatabaseFilePath = filePath;
                exception = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ForceCloseProjectLibrary(newProjectLibrary);
                ProjectControl.ChangeOpenProjectLibrary(null);
                OpenDatabaseFilePath = "";
                exception = new InvalidOperationException("Internal error on project loading!", e);
            }

            return exception == null;
        }

        /// <summary>
        ///     Closes the passed <see cref="IMocassinProjectLibrary" /> and requests a save confirmation from the user. Returns
        ///     false if the close was canceled by the user
        /// </summary>
        /// <param name="projectLibrary"></param>
        public bool TryCloseProjectLibrary(IMocassinProjectLibrary projectLibrary)
        {
            if (projectLibrary == null) return false;
            if (projectLibrary.HasUnsavedChanges())
            {
                var choice = GetSaveConfirmationFromUser(projectLibrary);
                switch (choice)
                {
                    case MessageBoxResult.Yes:
                        projectLibrary.SaveChanges();
                        break;

                    case MessageBoxResult.No:
                        break;

                    case MessageBoxResult.Cancel:
                        return false;
                }
            }

            projectLibrary.Dispose();
            return true;
        }

        /// <summary>
        ///     Enforces closing of the passed <see cref="IMocassinProjectLibrary" /> without checking for changes
        /// </summary>
        /// <param name="projectLibrary"></param>
        private void ForceCloseProjectLibrary(IMocassinProjectLibrary projectLibrary)
        {
            projectLibrary?.Dispose();
        }

        /// <summary>
        ///     Calls load on all <see cref="DbSet{TEntity}" /> of the passed <see cref="IMocassinProjectLibrary" /> and restores
        ///     the parent references
        /// </summary>
        /// <param name="projectLibrary"></param>
        private void LoadLibraryContents(IMocassinProjectLibrary projectLibrary)
        {
            projectLibrary?.MocassinProjectGraphs.Load();
        }

        /// <summary>
        ///     Requests a user input if the changes in the passed <see cref="IMocassinProjectLibrary" /> should be saved or not
        /// </summary>
        /// <returns></returns>
        private MessageBoxResult GetSaveConfirmationFromUser(IMocassinProjectLibrary projectLibrary)
        {
            const string caption = "Confirmation required";
            const string message = "Project entities have changed. Do you want to save the changes?";
            var choice = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            PushInfoMessage($"Choice: {choice}");
            return projectLibrary == null ? MessageBoxResult.No : choice;
        }
    }
}