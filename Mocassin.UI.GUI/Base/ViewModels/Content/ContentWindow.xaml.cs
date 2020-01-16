using System;
using System.ComponentModel;
using System.Windows;

namespace Mocassin.UI.GUI.Base.ViewModels.Content
{
    /// <summary>
    ///     Interaction logic for ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        public ContentWindow()
        {
            InitializeComponent();
        }

        private void ContentWindow_OnClosing(object sender, CancelEventArgs e)
        {
            (DataContext as IDisposable)?.Dispose();
        }
    }
}