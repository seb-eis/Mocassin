using System;
using System.Collections.Generic;
using System.Windows;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TDataObject}" /> for the relation of
    ///     <see cref="StateExchangeGroupData" /> to <see cref="AbstractTransitionData" /> host instances
    /// </summary>
    public sealed class AbstractTransitionExchangeGroupSelectionViewModel : HostGraphModelObjectSelectionViewModel<StateExchangeGroup,
        AbstractTransitionData>
    {
        /// <inheritdoc />
        public AbstractTransitionExchangeGroupSelectionViewModel(AbstractTransitionData hostObject)
            : base(hostObject, false)
        {
            ProcessDataObjectCommand = GetDropAddObjectCommand<StateExchangeGroupData>();
            Items = GetTargetCollection(hostObject);
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project) =>
            project?.ProjectModelData?.TransitionModelData?.StateExchangeGroups;

        /// <inheritdoc />
        protected override ICollection<ModelObjectReference<StateExchangeGroup>> GetTargetCollection(
            AbstractTransitionData sourceObject) =>
            sourceObject?.StateExchangeGroups;

        /// <inheritdoc />
        protected override IEnumerable<Predicate<IDataObject>> GetDropRejectPredicates()
        {
            yield return x => HostCollectionIsFull();
        }

        /// <summary>
        ///     Checks if the host collection is full in the context of the current connector string
        /// </summary>
        /// <returns></returns>
        public bool HostCollectionIsFull() =>
            HostObject.StateExchangeGroups.Count
            >= AbstractTransitionData.ConnectorRegex.Matches(HostObject.ConnectorString).Count + 1;
    }
}