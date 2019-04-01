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
using DataGrid = System.Windows.Controls.DataGrid;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel.GridControl
{
    /// <summary>
    /// Interaktionslogik für UnstableEnvironmentGridControlView.xaml
    /// </summary>
    public partial class UnstableEnvironmentGridControlView : UserControl
    {
        public UnstableEnvironmentGridControlView()
        {
            InitializeComponent();
        }

        private void UnstableEnvironmentDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is DataGrid dataGrid) || !(dataGrid.DataContext is UnstableEnvironmentGridControlViewModel context)) 
                return;
            if (e.Key != Key.F5) return;

            context.SynchronizeEnvironmentCollectionCommand.Execute(null);
            dataGrid.Items.Refresh();
            e.Handled = true;
        }
    }
}