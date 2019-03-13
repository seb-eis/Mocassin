﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.SQLiteCore;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ProjectManager.Commands;
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

        /// <inheritdoc />
        public ProjectManagerViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
            OpenProjectLibraryCommand = new OpenProjectLibraryCommand(this);
            CloseProjectLibraryCommand = new CloseProjectLibraryCommand(this);
            CreateProjectLibraryCommand = new CreateProjectLibraryCommand(this);
        }

        /// <summary>
        ///     Closes the currently loaded <see cref="IMocassinProjectLibrary" />
        /// </summary>
        public void CloseActiveProjectLibrary()
        {
            if (TryCloseProjectLibrary(MainProjectControl.OpenProjectLibrary))
            {
                MainProjectControl.SetOpenProjectLibrary(null);
                SendCallInfoMessage("Project closed!");
            }
            else
            {
                SendCallInfoMessage("Project close aborted!");
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
                SendCallErrorMessage(new FileNotFoundException("Requested file does not exist!", filePath));
                return;
            }

            if (!TryOpenProjectLibrary(filePath, false, out var exception))
                SendCallErrorMessage(exception);
            else
                SendCallInfoMessage($"Project loaded from: {filePath}");
        }

        /// <summary>
        ///     Creates a new <see cref="IMocassinProjectLibrary" /> at the passed filepath <see cref="string" /> and sets it as
        ///     the active project library
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateActiveProjectLibrary(string filePath)
        {
            if (File.Exists(filePath))
            {
                SendCallErrorMessage(new FileNotFoundException("Requested file already exists!", filePath));
                return;
            }

            if (!TryOpenProjectLibrary(filePath, true, out var exception))
                SendCallErrorMessage(exception);
            else
                SendCallInfoMessage($"Project created at: {filePath}");
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
                TryCloseProjectLibrary(MainProjectControl.OpenProjectLibrary);
                newProjectLibrary = SqLiteContext.OpenDatabase<MocassinProjectContext>(filePath, dropCreate);
                LoadLibraryContents(newProjectLibrary);
                MainProjectControl.SetOpenProjectLibrary(newProjectLibrary);
                OpenDatabaseFilePath = filePath;
                exception = null;
            }
            catch (Exception e)
            {
                ForceCloseProjectLibrary(newProjectLibrary);
                MainProjectControl.SetOpenProjectLibrary(null);
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
        ///     Calls load on all <see cref="DbSet{TEntity}" /> of the passed <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <param name="projectLibrary"></param>
        private void LoadLibraryContents(IMocassinProjectLibrary projectLibrary)
        {
            if (projectLibrary == null) return;
            projectLibrary.MocassinProjectGraphs.Load();
            projectLibrary.ProjectModelGraphs.Load();
            projectLibrary.MocassinProjectBuildGraphs.Load();
            projectLibrary.ProjectCustomizationGraphs.Load();
            projectLibrary.ProjectJobTranslationGraphs.Load();
        }

        /// <summary>
        ///     Requests a user input if the changes in the passed <see cref="IMocassinProjectLibrary" /> should be saved or not
        /// </summary>
        /// <returns></returns>
        private MessageBoxResult GetSaveConfirmationFromUser(IMocassinProjectLibrary projectLibrary)
        {
            const string caption = "Close Confirmation";
            const string message = "Project library has unsaved changes. Save changes before closing?";
            var choice = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel);
            SendCallInfoMessage($"Choice: {choice}");
            return projectLibrary == null ? MessageBoxResult.No : choice;
        }
    }
}