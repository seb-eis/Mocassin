using Mocassin.Mathematics.Coordinates;
using Mocassin.Model.ModelProject;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel"/> that controls object generation and supply for model components to a <see cref="DX3DViewportViewModel"/>
    /// </summary>
    public class DX3DModelVisualizationViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the internal <see cref="IModelProject"/> that handles model data processing
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get the <see cref="IVectorTransformer"/> that manages the transformations of the coordinate context
        /// </summary>
        private IVectorTransformer Transformer => ModelProject.CrystalSystemService.VectorTransformer;

        /// <inheritdoc />
        public DX3DModelVisualizationViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            throw new System.NotImplementedException();
        }
    }
}