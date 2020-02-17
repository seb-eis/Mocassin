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
    ///     <see cref="KineticTransitionData" /> to <see cref="KineticSimulationData" /> host instances
    /// </summary>
    public sealed class KineticSimulationTransitionSelectionViewModel :
        HostGraphModelObjectSelectionViewModel<KineticTransition, KineticSimulationData>
    {
        /// <inheritdoc />
        public KineticSimulationTransitionSelectionViewModel(KineticSimulationData hostObject)
            : base(hostObject, true)
        {
            Items = GetTargetCollection(hostObject);
            ProcessDataObjectCommand = GetDropAddObjectCommand<KineticTransitionData>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project)
        {
            return project?.ProjectModelData?.TransitionModelData?.KineticTransitions;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReference<KineticTransition>> GetTargetCollection(
            KineticSimulationData sourceObject)
        {
            return sourceObject?.Transitions;
        }
    }
}