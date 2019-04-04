using System.Windows.Controls;
using System.Windows.Input;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole
{
    /// <summary>
    ///     Interaktionslogik für OperationReportConsoleView.xaml
    /// </summary>
    public partial class OperationReportConsoleView : UserControl
    {
        public OperationReportConsoleView()
        {
            InitializeComponent();
        }

        private void ReportDataGrid_OnMouseEnter(object sender, MouseEventArgs e)
        {
            (DataContext as OperationReportConsoleViewModel).IsSoftUpdateStop = true;
        }

        private void ReportDataGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            (DataContext as OperationReportConsoleViewModel).IsSoftUpdateStop = false;
        }
    }
}