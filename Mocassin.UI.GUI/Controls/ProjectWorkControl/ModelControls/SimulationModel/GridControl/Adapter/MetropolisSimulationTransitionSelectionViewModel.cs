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
    ///     <see cref="MetropolisTransitionData" /> to <see cref="MetropolisSimulationData" /> host instances
    /// </summary>
    public sealed class MetropolisSimulationTransitionSelectionViewModel :
        HostGraphModelObjectSelectionViewModel<MetropolisTransition, MetropolisSimulationData>
    {
        /// <inheritdoc />
        public MetropolisSimulationTransitionSelectionViewModel(MetropolisSimulationData hostObject)
            : base(hostObject, true)
        {
            Items = GetTargetCollection(hostObject);
            ProcessDataObjectCommand = GetDropAddObjectCommand<MetropolisSimulationData>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project)
        {
            return project?.ProjectModelData?.TransitionModelData?.MetropolisTransitions;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReference<MetropolisTransition>> GetTargetCollection(
            MetropolisSimulationData sourceObject)
        {
            return sourceObject?.Transitions;
        }
    }
}