using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.Base.GridControl;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.GridControl
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation to wrap <see cref="ParticleSetData" />
    ///     into host view models for <see cref="ParticleData" /> instances
    /// </summary>
    public class ParticleSetParticleSelectionVmConverter : HostGraphGuestSelectionVmConverter<ParticleSetData>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProject> CreateSelectionViewModel(ParticleSetData host) => new ParticleSetParticleSelectionViewModel(host);
    }
}