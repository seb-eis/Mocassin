using System.Collections.Generic;
using Mocassin.Model.Particles;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.GridControl
{
    /// <summary>
    ///     The <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for the relation of
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
            HandleDropAddCommand = GetDropAddObjectCommand<ParticleData>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelDataObject> GetSourceCollection(MocassinProject project)
        {
            return project?.ProjectModelData?.ParticleModelData?.Particles;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReference<Particle>> GetTargetCollection(ParticleSetData sourceObject)
        {
            return sourceObject?.Particles;
        }
    }
}