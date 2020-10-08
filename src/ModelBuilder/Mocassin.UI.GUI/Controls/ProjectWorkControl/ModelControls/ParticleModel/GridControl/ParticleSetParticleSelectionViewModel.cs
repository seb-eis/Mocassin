using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.GridControl
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TDataObject}" /> for the relation of
    ///     <see cref="ParticleData" /> to <see cref="ParticleSetData" /> host instances
    /// </summary>
    public sealed class ParticleSetParticleSelectionViewModel
        : HostGraphModelObjectSelectionViewModel<Particle, ParticleSetData>
    {
        /// <inheritdoc />
        public ParticleSetParticleSelectionViewModel(ParticleSetData hostObject)
            : base(hostObject, true)
        {
            Items = GetTargetCollection(hostObject);
            ProcessDataObjectCommand = GetDropAddObjectCommand<ParticleData>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project) =>
            project?.ProjectModelData?.ParticleModelData?.Particles;

        /// <inheritdoc />
        protected override ICollection<ModelObjectReference<Particle>> GetTargetCollection(ParticleSetData sourceObject) => sourceObject?.Particles;
    }
}