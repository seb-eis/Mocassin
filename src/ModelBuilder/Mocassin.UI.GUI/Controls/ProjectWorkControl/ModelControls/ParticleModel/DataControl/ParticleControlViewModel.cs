using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for controlling sets of <see cref="ParticleData" /> of a
    ///     selectable <see cref="MocassinProject" />
    /// </summary>
    public class ParticleControlViewModel : CollectionControlViewModel<ParticleData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            Items = contentSource?.ProjectModelData?.ParticleModelData?.Particles;
        }
    }
}