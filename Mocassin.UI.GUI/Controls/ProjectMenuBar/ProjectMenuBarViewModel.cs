using System.Windows.Controls;
using Mocassin.UI.Base.ViewModels;
using Mocassin.UI.Base.ViewModels.MenuBar;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar
{
    /// <summary>
    ///     The <see cref="ViewModel" /> for the <see cref="ProjectMenuBarView" /> that controls the main menu bar
    /// </summary>
    public class ProjectMenuBarViewModel : DynamicMenuBarViewModel
    {
        /// <inheritdoc />
        public ProjectMenuBarViewModel() : base(Dock.Top)
        {
            InitializeDefaultMenuItems();
        }

        /// <inheritdoc />
        protected sealed override void InitializeDefaultMenuItems()
        {
            base.InitializeDefaultMenuItems();
            AddCollectionItem(MakeDefaultFileMenu(null));
        }
    }
}