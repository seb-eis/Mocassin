using System;
using System.Globalization;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> to wrap <see cref="StateExchangeGroupData" /> into
    ///     host view models for <see cref="StateExchangePairData" /> instances
    /// </summary>
    public class ExchangeGroupExchangePairSelectionVmConverter : HostGraphGuestSelectionVmConverter<StateExchangeGroupData>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(StateExchangeGroupData host)
        {
            return new ExchangeGroupExchangePairSelectionViewModel(host);
        }
    }
}