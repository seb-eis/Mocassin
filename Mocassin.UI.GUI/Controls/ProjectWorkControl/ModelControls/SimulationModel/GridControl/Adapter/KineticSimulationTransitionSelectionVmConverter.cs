using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation for wrapping
    ///     <see cref="KineticSimulationData" /> instances into host view models for <see cref="KineticTransitionData" />
    ///     references
    /// </summary>
    public class KineticSimulationTransitionSelectionVmConverter : HostGraphGuestSelectionVmConverter<KineticSimulationData>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(KineticSimulationData host)
        {
            return new KineticSimulationTransitionSelectionViewModel(host);
        }
    }
}