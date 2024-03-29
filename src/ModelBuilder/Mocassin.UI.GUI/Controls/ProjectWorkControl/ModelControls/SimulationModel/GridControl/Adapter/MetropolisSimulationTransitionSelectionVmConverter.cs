﻿using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.SimulationModel;
using Mocassin.UI.Data.TransitionModel;

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
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(MetropolisSimulationData host) =>
            new MetropolisSimulationTransitionSelectionViewModel(host);
    }
}