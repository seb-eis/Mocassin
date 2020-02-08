using System;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer
{
    /// <summary>
    ///     Interaction logic for DxModelSceneView.xaml
    /// </summary>
    public partial class DxModelSceneView : UserControl, IDisposable
    {
        public DxModelSceneView()
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