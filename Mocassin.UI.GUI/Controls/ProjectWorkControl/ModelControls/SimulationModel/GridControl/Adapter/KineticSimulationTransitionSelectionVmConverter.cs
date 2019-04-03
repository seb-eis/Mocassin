using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation for wrapping
    ///     <see cref="KineticSimulationGraph" /> instances into host view models for <see cref="KineticTransitionGraph" />
    ///     references
    /// </summary>
    public class KineticSimulationTransitionSelectionVmConverter : HostGraphGuestSelectionVmConverter<KineticSimulationGraph>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProjectGraph> CreateSelectionViewModel(KineticSimulationGraph host)
        {
            return new KineticSimulationTransitionSelectionViewModel(host);
        }
    }
}