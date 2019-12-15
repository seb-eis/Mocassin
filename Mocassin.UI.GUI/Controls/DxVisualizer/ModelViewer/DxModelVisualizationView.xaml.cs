using System;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer
{
    /// <summary>
    /// Interaction logic for DxModelVisualizationView.xaml
    /// </summary>
    public partial class DxModelVisualizationView : UserControl, IDisposable
    {
        public DxModelVisualizationView()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DxViewportView.Dispose();
            DxViewportView.DataContext = null;
        }
    }
}
