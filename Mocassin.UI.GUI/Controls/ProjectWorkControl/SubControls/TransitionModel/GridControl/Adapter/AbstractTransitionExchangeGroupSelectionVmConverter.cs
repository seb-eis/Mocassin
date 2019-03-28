using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation for wrapping
    ///     <see cref="AbstractTransitionGraph" /> instances into host view models for <see cref="StateExchangeGroupGraph" />
    ///     references
    /// </summary>
    public class AbstractTransitionExchangeGroupSelectionVmConverter : HostGraphGuestSelectionVmConverter<AbstractTransitionGraph>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProjectGraph> CreateSelectionViewModel(AbstractTransitionGraph host)
        {
            return new AbstractTransitionExchangeGroupSelectionViewModel(host);
        }
    }
}