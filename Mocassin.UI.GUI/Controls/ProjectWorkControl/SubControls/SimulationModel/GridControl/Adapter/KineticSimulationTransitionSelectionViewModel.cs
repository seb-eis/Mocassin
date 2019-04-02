using System.Collections.Generic;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.SimulationModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for the relation of
    ///     <see cref="KineticTransitionGraph" /> to <see cref="KineticSimulationGraph" /> host instances
    /// </summary>
    public sealed class KineticSimulationTransitionSelectionViewModel :
        HostGraphModelObjectSelectionViewModel<KineticTransition, KineticSimulationGraph>
    {
        /// <inheritdoc />
        public KineticSimulationTransitionSelectionViewModel(KineticSimulationGraph hostObject)
            : base(hostObject, true)
        {
            DataCollection = GetTargetCollection(hostObject);
            HandleDropAddCommand = GetDropAddObjectCommand<KineticTransitionGraph>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph)
        {
            return projectGraph?.ProjectModelGraph?.TransitionModelGraph?.KineticTransitions;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReferenceGraph<KineticTransition>> GetTargetCollection(
            KineticSimulationGraph sourceObject)
        {
            return sourceObject?.Transitions;
        }
    }
}