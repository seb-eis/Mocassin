using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel.DataControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="EnergyModelControlView" /> that controls the energy model
    ///     definition
    /// </summary>
    public class EnergyModelControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="EnergyParameterControlViewModel" /> that controls the stable environment settings
        /// </summary>
        public EnergyParameterControlViewModel ParameterControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="UnstableEnvironmentControlViewModel" /> that controls the unstable environment instances
        /// </summary>
        public UnstableEnvironmentControlViewModel UnstableEnvironmentViewModel { get; }

        /// <summary>
        ///     Get the <see cref="GroupInteractionControlViewModel" /> that controls the interaction group instances
        /// </summary>
        public GroupInteractionControlViewModel GroupInteractionViewModel { get; }

        /// <inheritdoc />
        public EnergyModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ParameterControlViewModel = new EnergyParameterControlViewModel(projectControl);
            UnstableEnvironmentViewModel = new UnstableEnvironmentControlViewModel(projectControl);
            GroupInteractionViewModel = new GroupInteractionControlViewModel();
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
            ParameterControlViewModel.ChangeContentSource(contentSource);
            UnstableEnvironmentViewModel.ChangeContentSource(contentSource);
            GroupInteractionViewModel.ChangeContentSource(contentSource);
        }
    }
}