﻿using System;
using System.Linq;
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
        public Viewport3DView()
        {
            InitializeComponent();
            BackgroundColorComboBox.SelectedItem = typeof(Colors).GetProperties().First(x => x.Name == "Transparent");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            HelixViewport3D.DataContext = null;
        }

        private void QuickActionContextMenu_ResetCameraItem_OnClick(object sender, RoutedEventArgs e)
        {
            HelixViewport3D.ResetCamera();
        }
    }
}