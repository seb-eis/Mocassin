using System.Collections.Generic;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.SimulationModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.GridControl.Adapter
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for the relation of
    ///     <see cref="MetropolisTransitionGraph" /> to <see cref="MetropolisSimulationGraph" /> host instances
    /// </summary>
    public sealed class MetropolisSimulationTransitionSelectionViewModel :
        HostGraphModelObjectSelectionViewModel<MetropolisTransition, MetropolisSimulationGraph>
    {
        /// <inheritdoc />
        public MetropolisSimulationTransitionSelectionViewModel(MetropolisSimulationGraph hostObject)
            : base(hostObject, true)
        {
            Items = GetTargetCollection(hostObject);
            HandleDropAddCommand = GetDropAddObjectCommand<MetropolisSimulationGraph>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph)
        {
            return projectGraph?.ProjectModelGraph?.TransitionModelGraph?.MetropolisTransitions;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReferenceGraph<MetropolisTransition>> GetTargetCollection(
            MetropolisSimulationGraph sourceObject)
        {
            return sourceObject?.Transitions;
        }
    }
}