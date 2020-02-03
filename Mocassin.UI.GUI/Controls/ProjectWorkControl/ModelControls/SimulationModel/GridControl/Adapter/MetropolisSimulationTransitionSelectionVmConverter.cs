using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation for wrapping
    ///     <see cref="MetropolisSimulationData" /> instances into host view models for
    ///     <see cref="MetropolisTransitionData" /> references
    /// </summary>
    public class MetropolisSimulationTransitionSelectionVmConverter : HostGraphGuestSelectionVmConverter<MetropolisSimulationData>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(MetropolisSimulationData host)
        {
            return new MetropolisSimulationTransitionSelectionViewModel(host);
        }
    }
}