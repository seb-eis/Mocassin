using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar
{
    /// <summary>
    /// Interaktionslogik für ProjectMenuBarView.xaml
    /// </summary>
    public partial class ProjectMenuBarView : UserControl
    {
        public ProjectMenuBarView()
        {
            InitializeComponent();
        }

        private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_OpenProjectLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Database Files (*.db)|*.db|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            var name = openFileDialog.FileName;
            var control = (ProjectMenuBarViewModel) DataContext;
           control.MainProjectControl.ProjectManagerViewModel.LoadActiveProjectLibrary(name);
        }

        private void MenuItem_CloseProjectLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            var control = (ProjectMenuBarViewModel) DataContext;
            control.MainProjectControl.ProjectManagerViewModel.CloseActiveProjectLibrary();
        }
    }
}
