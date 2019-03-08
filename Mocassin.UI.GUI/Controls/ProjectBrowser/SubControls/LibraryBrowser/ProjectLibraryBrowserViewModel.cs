using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.LibraryBrowser
{
    /// <summary>
    ///     The <see cref="Mocassin.UI.GUI.Base.ViewModels.ViewModel" /> for <see cref="ProjectLibraryBrowserView" />
    /// </summary>
    public class ProjectLibraryBrowserViewModel : PrimaryControlViewModel
    {
        /// <inheritdoc />
        public ProjectLibraryBrowserViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
        }
    }
}