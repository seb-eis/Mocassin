using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> to wrap <see cref="StateExchangeGroupData" /> into
    ///     host view models for <see cref="StateExchangePairData" /> instances
    /// </summary>
    public class ExchangeGroupExchangePairSelectionVmConverter : HostGraphGuestSelectionVmConverter<StateExchangeGroupData>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(StateExchangeGroupData host) =>
            new ExchangeGroupExchangePairSelectionViewModel(host);
    }
}