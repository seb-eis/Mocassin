using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Controls.DxVisualizer.Extensions;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport
{
    /// <summary>
    ///     Interaction logic for DxViewportView.xaml
    /// </summary>
    public partial class DxViewportView : UserControl
    {
        public DxViewportView()
        {
            InitializeComponent();
        }

        private void Viewport3Dx_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ReferenceEquals(sender, Viewport3Dx)) return;
            Viewport3Dx?.SetValue(DxViewportExtensions.IsInteractingProperty, true);
        }

        private void Viewport3Dx_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ReferenceEquals(sender, Viewport3Dx)) return;
            Viewport3Dx?.SetValue(DxViewportExtensions.IsInteractingProperty, false);
        }
    }
}