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
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> to wrap <see cref="StateExchangeGroupGraph" /> into
    ///     host view models for <see cref="StateExchangePairGraph" /> instances
    /// </summary>
    public class ExchangeGroupExchangePairSelectionVmConverter : HostGraphGuestSelectionVmConverter<StateExchangeGroupGraph>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProjectGraph> CreateSelectionViewModel(StateExchangeGroupGraph host)
        {
            return new ExchangeGroupExchangePairSelectionViewModel(host);
        }
    }
}