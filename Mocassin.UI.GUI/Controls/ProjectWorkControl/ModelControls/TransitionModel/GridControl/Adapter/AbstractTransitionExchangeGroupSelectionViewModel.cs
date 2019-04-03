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
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for the relation of
    ///     <see cref="StateExchangeGroupGraph" /> to <see cref="AbstractTransitionGraph" /> host instances
    /// </summary>
    public sealed class AbstractTransitionExchangeGroupSelectionViewModel : HostGraphModelObjectSelectionViewModel<StateExchangeGroup,
        AbstractTransitionGraph>
    {
        /// <inheritdoc />
        public AbstractTransitionExchangeGroupSelectionViewModel(AbstractTransitionGraph hostObject)
            : base(hostObject, false)
        {
            HandleDropAddCommand = GetDropAddObjectCommand<StateExchangeGroupGraph>();
            DataCollection = GetTargetCollection(hostObject);
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph)
        {
            return projectGraph?.ProjectModelGraph?.TransitionModelGraph?.StateExchangeGroups;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReferenceGraph<StateExchangeGroup>> GetTargetCollection(
            AbstractTransitionGraph sourceObject)
        {
            return sourceObject?.StateExchangeGroups;
        }

        /// <inheritdoc />
        protected override IEnumerable<Predicate<IDataObject>> GetDropRejectPredicates()
        {
            yield return x => HostCollectionIsFull();
        }

        /// <summary>
        ///     Checks if the host collection is full in the context of the current connector string
        /// </summary>
        /// <returns></returns>
        public bool HostCollectionIsFull()
        {
            return HostObject.StateExchangeGroups.Count
                   >= AbstractTransitionGraph.ConnectorRegex.Matches(HostObject.ConnectorString).Count + 1;
        }
    }
}