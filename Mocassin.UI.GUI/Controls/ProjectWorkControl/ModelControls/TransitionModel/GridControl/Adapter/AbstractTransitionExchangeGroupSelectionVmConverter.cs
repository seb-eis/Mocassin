using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation for wrapping
    ///     <see cref="AbstractTransitionData" /> instances into host view models for <see cref="StateExchangeGroupData" />
    ///     references
    /// </summary>
    public class AbstractTransitionExchangeGroupSelectionVmConverter : HostGraphGuestSelectionVmConverter<AbstractTransitionData>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(AbstractTransitionData host) =>
            new AbstractTransitionExchangeGroupSelectionViewModel(host);
    }
}