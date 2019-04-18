using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ProjectBuilding.DataControl
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> for <see cref="LocalProjectDeployControlView" /> that controls simulation database
    ///     file definition and creation
    /// </summary>
    public class LocalProjectDeployControlViewModel : PrimaryControlViewModel
    {
        private string directoryPath;
        private string fileName;

        /// <summary>
        ///     Get or set the base directory path <see cref="string"/> that is used for project building
        /// </summary>
        public string DirectoryPath
        {
            get => directoryPath;
            set => SetProperty(ref directoryPath, value);
        }

        /// <summary>
        ///     Get or set the file name <see cref="string"/> that is used for project building
        /// </summary>
        public string FileName
        {
            get => fileName;
            set => SetProperty(ref fileName, value);
        }

        /// <inheritdoc />
        public LocalProjectDeployControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }
    }
}