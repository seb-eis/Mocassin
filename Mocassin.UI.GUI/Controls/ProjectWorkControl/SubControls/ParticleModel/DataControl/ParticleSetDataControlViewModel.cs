using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.DataControl
{
    /// <summary>
    ///     The <see cref="DataCollectionControlViewModel{T}" /> for controlling sets of <see cref="ParticleSetGraph" /> of a
    ///     selectable <see cref="MocassinProjectGraph" />
    /// </summary>
    public class ParticleSetDataControlViewModel : DataCollectionControlViewModel<ParticleSetGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            var modelGraph = contentSource?.ProjectModelGraph?.ParticleModelGraph;
            DataCollection = modelGraph?.ParticleSets;
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }
    }
}