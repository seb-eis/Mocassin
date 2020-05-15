using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;
using Mocassin.Framework.Extensions;

namespace Mocassin.UI.GUI.Controls.Base.IO
{
    /// <summary>
    ///     Adapter that supplies <see cref="OpenFileDialog" /> tailored to for project file selection
    /// </summary>
    public class UserFileSelectionSource
    {
        /// <summary>
        ///     Get the supported file naming and extension <see cref="string" /> values
        /// </summary>
        public (string Name, string Extension)[] SupportedFileTypes { get; }

        /// <summary>
        ///     Get the project file selection filter <see cref="string" />
        /// </summary>
        public string FileFilter { get; }

        /// <summary>
        ///     Get a boolean flag if the <see cref="SaveFileDialog" /> is used
        /// </summary>
        public bool UseSaveDialog { get; }

        /// <summary>
        ///     Creates a new <see cref="UserFileSelectionSource" /> with the supported file types
        /// </summary>
        /// <param name="useSaveDialog"></param>
        /// <param name="supportedFileTypes"></param>
        public UserFileSelectionSource(bool useSaveDialog, params (string Name, string Extension)[] supportedFileTypes)
        {
            SupportedFileTypes = supportedFileTypes ?? throw new ArgumentNullException(nameof(supportedFileTypes));
            FileFilter = BuildFilterString(supportedFileTypes);
            UseSaveDialog = useSaveDialog;
        }

        /// <summary>
        ///     Get a file <see cref="string" /> selection from the user
        /// </summary>
        /// <param name="checkFileExists"></param>
        /// <returns></returns>
        public string RequestFileSelection(bool checkFileExists = false)
        {
            var openFileDialog = GetFileDialog(checkFileExists);
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        /// <summary>
        ///     Tries to get a file <see cref="string" /> selection from the user. Returns false if the selected file does not
        ///     exist
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="checkFileExists"></param>
        /// <returns></returns>
        public bool TryRequestFileSelection(out string selected, bool checkFileExists = false)
        {
            selected = RequestFileSelection(checkFileExists);
            return File.Exists(selected) ^ !checkFileExists;
        }

        /// <summary>
        ///     Creates a new <see cref="UserFileSelectionSource" /> tailored to project files
        /// </summary>
        /// <param name="useSaveDialog"></param>
        /// <returns></returns>
        public static UserFileSelectionSource CreateForProjectFiles(bool useSaveDialog)
        {
            return new UserFileSelectionSource(useSaveDialog, ("Project file", "mocprj"));
        }

        /// <summary>
        ///     Creates  a new <see cref="UserFileSelectionSource" /> tailored to simulation databases
        /// </summary>
        /// <param name="useSaveDialog"></param>
        /// <returns></returns>
        public static UserFileSelectionSource CreateForJobDbFiles(bool useSaveDialog)
        {
            return new UserFileSelectionSource(useSaveDialog, ("Simulation library", "msl"));
        }

        /// <summary>
        ///     Converts a set of name and file extension strings into a <see cref="OpenFileDialog" /> selection filter
        /// </summary>
        /// <param name="supportedFileTypes"></param>
        /// <returns></returns>
        public static string BuildFilterString(IEnumerable<(string Name, string Extension)> supportedFileTypes = null)
        {
            if (supportedFileTypes == null) return "All files (*.*)|*.*";

            var builder = new StringBuilder(100);
            foreach (var (name, extension) in supportedFileTypes) builder.Append($"{name} (*.{extension})|*.{extension}|");
            builder.PopBack(1);
            return builder.ToString();
        }

        /// <summary>
        ///     Get a new <see cref="FileDialog" /> based on the settings
        /// </summary>
        /// <param name="checkFileExists"></param>
        /// <returns></returns>
        private FileDialog GetFileDialog(bool checkFileExists)
        {
            var dialog = UseSaveDialog ? (FileDialog) new SaveFileDialog() : new OpenFileDialog();
            dialog.CheckFileExists = checkFileExists;
            dialog.Filter = FileFilter;
            return dialog;
        }
    }
}