using System.ComponentModel;
using System.Windows;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.FileMenu.Commands;

namespace Mocassin.UI.GUI
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <inheritdoc />
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Event that is called when the main window close is requested. Ensures that the user has a chance to save a project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!(DataContext is IMocassinProjectControl projectControl)) return;
            if (projectControl.OpenProjectLibrary == null) return;

            new SaveExitProgramCommand(projectControl).Execute();
            if (projectControl.OpenProjectLibrary != null) e.Cancel = true;
        }
    }
}