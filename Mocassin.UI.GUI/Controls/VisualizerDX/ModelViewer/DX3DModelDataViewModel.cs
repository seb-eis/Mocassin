using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport;

namespace Mocassin.UI.GUI.Controls.VisualizerDX
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="DX3DModelDataView" /> that controls model visualization
    ///     with SharpDX
    /// </summary>
    public class DX3DModelDataViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="DX3DViewportViewModel" /> for the 3D viewport
        /// </summary>
        public DX3DViewportViewModel ViewportViewModel { get; }

        /// <inheritdoc />
        public DX3DModelDataViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ViewportViewModel = new DX3DViewportViewModel();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ViewportViewModel.Dispose();
            base.Dispose();
        }
    }
}