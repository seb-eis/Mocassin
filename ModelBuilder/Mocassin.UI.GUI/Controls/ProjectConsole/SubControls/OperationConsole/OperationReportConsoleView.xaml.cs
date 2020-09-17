using System.Windows.Controls;
using System.Windows.Input;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole
{
    /// <summary>
    ///     Interaktionslogik für OperationReportConsoleView.xaml
    /// </summary>
    public partial class OperationReportConsoleView : UserControl
    {
        /// <summary>
        /// </summary>
        public OperationReportConsoleView()
        {
            InitializeComponent();
        }

        private void ReportDataGrid_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ((OperationReportConsoleViewModel) DataContext).IsSoftUpdateStop = true;
        }

        private void ReportDataGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ((OperationReportConsoleViewModel) DataContext).IsSoftUpdateStop = false;
        }
    }
}