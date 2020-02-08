using System.Collections.Generic;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for the relation of
    ///     <see cref="StateExchangePairData" /> to <see cref="StateExchangeGroupData" /> host instances
    /// </summary>
    public sealed class ExchangeGroupExchangePairSelectionViewModel : HostGraphModelObjectSelectionViewModel<StateExchangePair,
        StateExchangeGroupData>
    {
        /// <inheritdoc />
        public ExchangeGroupExchangePairSelectionViewModel(StateExchangeGroupData hostObject)
            : base(hostObject, true)
        {
            Items = GetTargetCollection(hostObject);
            ProcessDataObjectCommand = GetDropAddObjectCommand<StateExchangePairData>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project)
        {
            return project?.ProjectModelData?.TransitionModelData?.StateExchangePairs;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReference<StateExchangePair>> GetTargetCollection(
            StateExchangeGroupData sourceObject)
        {
            return sourceObject?.StateExchangePairs;
        }
    }
}