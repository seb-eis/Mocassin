using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for the <see cref="UnstableEnvironmentControlView" /> that controls
    ///     unstable environments
    /// </summary>
    public class UnstableEnvironmentControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="UnstableEnvironmentGridControlViewModel" /> that controls unstable environment data grid
        /// </summary>
        public UnstableEnvironmentGridControlViewModel EnvironmentGridControlViewModel { get; }

        /// <inheritdoc />
        public UnstableEnvironmentControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            EnvironmentGridControlViewModel = new UnstableEnvironmentGridControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            EnvironmentGridControlViewModel.ChangeContentSource(contentSource);
        }
    }
}