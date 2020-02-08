using System;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Controls.DxVisualizer.Extensions;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport
{
    /// <summary>
    ///     Interaction logic for DxViewportView.xaml
    /// </summary>
    public partial class DxViewportView : UserControl, IDisposable
    {
        public DxViewportView()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var item in Viewport3Dx.Items) item.Dispose();
            Viewport3Dx.Items.Clear();
            Viewport3Dx.DataContext = null;
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