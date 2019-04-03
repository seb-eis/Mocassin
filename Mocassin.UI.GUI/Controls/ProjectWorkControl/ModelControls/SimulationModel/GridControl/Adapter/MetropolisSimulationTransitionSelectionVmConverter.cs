using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation for wrapping
    ///     <see cref="MetropolisSimulationGraph" /> instances into host view models for
    ///     <see cref="MetropolisTransitionGraph" /> references
    /// </summary>
    public class MetropolisSimulationTransitionSelectionVmConverter : HostGraphGuestSelectionVmConverter<MetropolisSimulationGraph>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProjectGraph> CreateSelectionViewModel(MetropolisSimulationGraph host)
        {
            return new MetropolisSimulationTransitionSelectionViewModel(host);
        }
    }
}