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

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole
{
    /// <summary>
    /// Interaktionslogik für OperationReportConsoleView.xaml
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
