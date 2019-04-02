using System.Collections.Generic;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for the relation of
    ///     <see cref="StateExchangePairGraph" /> to <see cref="StateExchangeGroupGraph" /> host instances
    /// </summary>
    public sealed class ExchangeGroupExchangePairSelectionViewModel : HostGraphModelObjectSelectionViewModel<StateExchangePair,
        StateExchangeGroupGraph>
    {
        /// <inheritdoc />
        public ExchangeGroupExchangePairSelectionViewModel(StateExchangeGroupGraph hostObject)
            : base(hostObject, true)
        {
            DataCollection = GetTargetCollection(hostObject);
            HandleDropAddCommand = GetDropAddObjectCommand<StateExchangePairGraph>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph)
        {
            return projectGraph?.ProjectModelGraph?.TransitionModelGraph?.StateExchangePairs;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReferenceGraph<StateExchangePair>> GetTargetCollection(
            StateExchangeGroupGraph sourceObject)
        {
            return sourceObject?.StateExchangePairs;
        }
    }
}