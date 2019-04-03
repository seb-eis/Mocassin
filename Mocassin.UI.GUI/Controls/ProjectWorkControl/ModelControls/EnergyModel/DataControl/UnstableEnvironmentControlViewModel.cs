using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="UnstableEnvironmentControlView" /> that controls
    ///     unstable environments
    /// </summary>
    public class UnstableEnvironmentControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

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
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            EnvironmentGridControlViewModel.ChangeContentSource(contentSource);
        }
    }
}