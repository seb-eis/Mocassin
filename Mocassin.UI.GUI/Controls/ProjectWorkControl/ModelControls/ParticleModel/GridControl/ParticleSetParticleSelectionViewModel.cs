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
    ///     <see cref="ParticleGraph" /> to <see cref="ParticleSetGraph" /> host instances
    /// </summary>
    public sealed class ParticleSetParticleSelectionViewModel
        : HostGraphModelObjectSelectionViewModel<Particle, ParticleSetGraph>
    {
        /// <inheritdoc />
        public ParticleSetParticleSelectionViewModel(ParticleSetGraph hostObject)
            : base(hostObject, true)
        {
            Items = GetTargetCollection(hostObject);
            HandleDropAddCommand = GetDropAddObjectCommand<ParticleGraph>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph)
        {
            return projectGraph?.ProjectModelGraph?.ParticleModelGraph?.Particles;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReferenceGraph<Particle>> GetTargetCollection(ParticleSetGraph sourceObject)
        {
            return sourceObject?.Particles;
        }
    }
}