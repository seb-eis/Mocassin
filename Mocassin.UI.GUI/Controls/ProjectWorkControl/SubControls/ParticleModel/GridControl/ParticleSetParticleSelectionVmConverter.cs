using System;
using System.Globalization;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.GridControl
{
    /// <summary>
    ///     The <see cref="HostGraphGuestSelectionVmConverter{THost}" /> implementation to wrap <see cref="ParticleSetGraph" />
    ///     into host view models for <see cref="ParticleGraph" /> instances
    /// </summary>
    public class ParticleSetParticleSelectionVmConverter : HostGraphGuestSelectionVmConverter<ParticleSetGraph>
    {
        /// <inheritdoc />
        protected override IContentSupplier<MocassinProjectGraph> CreateSelectionViewModel(ParticleSetGraph host)
        {
            return  new ParticleSetParticleSelectionViewModel(host);
        }
    }
}