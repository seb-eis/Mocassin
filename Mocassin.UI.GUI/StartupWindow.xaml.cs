using System;
using System.Windows;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window, IDisposable
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Close();
        }
    }
}