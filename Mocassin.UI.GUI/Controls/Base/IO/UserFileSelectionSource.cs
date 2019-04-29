using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using Mocassin.Framework.Extensions;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Controls.Base.IO
{
    /// <summary>
    ///     Adapter that supplies <see cref="OpenFileDialog"/> tailored to for project file selection
    /// </summary>
    public class UserFileSelectionSource
    {
        /// <summary>
        ///     Get the supported file naming and extension <see cref="string"/> values
        /// </summary>
        public (string Name, string Extension)[] SupportedFileTypes { get; }

        /// <summary>
        ///     Get the project file selection filter <see cref="string"/>
        /// </summary>
        public string FileFilter { get; }

        /// <summary>
        ///     Creates a new <see cref="UserFileSelectionSource"/> with the supported file types
        /// </summary>
        /// <param name="supportedFileTypes"></param>
        public UserFileSelectionSource(params (string Name, string Extension)[] supportedFileTypes)
        {
            SupportedFileTypes = supportedFileTypes ?? throw new ArgumentNullException(nameof(supportedFileTypes));
            FileFilter = BuildFilterString(supportedFileTypes);
        }

        /// <summary>
        ///     Get a file <see cref="string"/> selection from the user
        /// </summary>
        /// <param name="checkFileExists"></param>
        /// <returns></returns>
        public string GetFileSelection(bool checkFileExists = false)
        {
            var openFileDialog = new OpenFileDialog {Filter = FileFilter, CheckFileExists = checkFileExists};
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        /// <summary>
        ///     Tries to get a file <see cref="string"/> selection from the user. Returns false if the selected file does not exist
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="checkFileExists"></param>
        /// <returns></returns>
        public bool TryGetFileSelection(out string selected, bool checkFileExists = false)
        {
            selected = GetFileSelection(checkFileExists);
            return File.Exists(selected) ^ !checkFileExists;
        }

        /// <summary>
        ///     Creates a new <see cref="UserFileSelectionSource"/> tailored to project files
        /// </summary>
        /// <returns></returns>
        public static UserFileSelectionSource CreateForProjectFiles()
        {
            return new UserFileSelectionSource(("Project file", "moc"));
        }

        /// <summary>
        ///     Creates  a new <see cref="UserFileSelectionSource"/> tailored to simulation databases
        /// </summary>
        /// <returns></returns>
        public static UserFileSelectionSource CreateForJobDbFiles()
        {
            return new UserFileSelectionSource(("Simulation library", "msl"));
        }

        /// <summary>
        ///     Converts a set of name and file extension strings into a <see cref="OpenFileDialog"/> selection filter
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
    }
}