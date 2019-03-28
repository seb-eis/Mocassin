using System;
using System.Collections;
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

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl
{
    /// <summary>
    /// Interaktionslogik für MetropolisTransitionGridControlView.xaml
    /// </summary>
    public partial class MetropolisTransitionGridControlView : UserControl
    {
        public MetropolisTransitionGridControlView()
        {
            InitializeComponent();
        }

        private void TransitionDataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                dataGrid.SelectedItem = null;
            }
        }
    }
}
