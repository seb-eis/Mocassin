using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for controlling sets of <see cref="ParticleSetGraph" /> of a
    ///     selectable <see cref="MocassinProjectGraph" />
    /// </summary>
    public class ParticleSetControlViewModel : CollectionControlViewModel<ParticleSetGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            var modelGraph = contentSource?.ProjectModelGraph?.ParticleModelGraph;
            Items = modelGraph?.ParticleSets;
        }
    }
}