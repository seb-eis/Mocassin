using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for controlling sets of <see cref="ParticleSetData" /> of a
    ///     selectable <see cref="MocassinProject" />
    /// </summary>
    public class ParticleSetControlViewModel : CollectionControlViewModel<ParticleSetData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            var modelGraph = contentSource?.ProjectModelData?.ParticleModelData;
            Items = modelGraph?.ParticleSets;
        }
    }
}