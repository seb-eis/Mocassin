using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="EnergyModelControlView" /> that controls the energy model
    ///     definition
    /// </summary>
    public class EnergyModelControlViewModel : ProjectGraphControlViewModel
    {
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

        /// <summary>
        ///     Get the <see cref="DefectEnergyControlViewModel"/> that controls the background defect energy set
        /// </summary>
        public DefectEnergyControlViewModel DefectEnergyViewModel { get; }

        /// <inheritdoc />
        public EnergyModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ParameterControlViewModel = new EnergyParameterControlViewModel(projectControl);
            UnstableEnvironmentViewModel = new UnstableEnvironmentControlViewModel(projectControl);
            GroupInteractionViewModel = new GroupInteractionControlViewModel();
            DefectEnergyViewModel = new DefectEnergyControlViewModel(projectControl);
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            ParameterControlViewModel.ChangeContentSource(contentSource);
            UnstableEnvironmentViewModel.ChangeContentSource(contentSource);
            GroupInteractionViewModel.ChangeContentSource(contentSource);
            DefectEnergyViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }
    }
}