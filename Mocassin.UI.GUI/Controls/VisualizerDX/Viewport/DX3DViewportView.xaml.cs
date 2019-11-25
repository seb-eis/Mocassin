using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport
{
    /// <summary>
    ///     Interaction logic for DX3DViewportView.xaml
    /// </summary>
    public partial class DX3DViewportView : UserControl
    {
        public DX3DViewportView()
        {
            InitializeComponent();
        }

        private void Viewport3Dx_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ReferenceEquals(sender, Viewport3Dx)) return;
            Viewport3Dx?.SetValue(Viewport3DXExtensions.IsInteractingProperty, true);
        }

        private void Viewport3Dx_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ReferenceEquals(sender, Viewport3Dx)) return;
            Viewport3Dx?.SetValue(Viewport3DXExtensions.IsInteractingProperty, false);
        }
    }
}