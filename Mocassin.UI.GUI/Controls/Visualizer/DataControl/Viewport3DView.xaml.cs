using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mocassin.UI.GUI.Controls.Visualizer.DataControl
{
    /// <summary>
    ///     Interaktionslogik für Viewport3DView.xaml
    /// </summary>
    public partial class Viewport3DView : UserControl, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="PropertyInfo"/> for the default <see cref="Color"/> property
        /// </summary>
        private static PropertyInfo DefaultColorPropertyInfo { get; }

        static Viewport3DView()
        {
            DefaultColorPropertyInfo = typeof(Colors).GetProperties().First(x => x.Name == "Transparent");
        }

        public Viewport3DView()
        {
            InitializeComponent();
            BackgroundColorComboBox.SelectedItem = DefaultColorPropertyInfo;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            HelixViewport3D.DataContext = null;
        }

        /// <summary>
        ///     Action to reset the camera of the viewport
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickActionContextMenu_ResetCameraItem_OnClick(object sender, RoutedEventArgs e)
        {
            HelixViewport3D.ResetCamera();
        }
    }
}