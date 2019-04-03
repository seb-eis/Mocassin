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

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    /// Interaktionslogik für StructureParameterControlView.xaml
    /// </summary>
    public partial class StructureParameterControlView : UserControl
    {
        public StructureParameterControlView()
        {
            InitializeComponent();
        }

        private void SpaceGroupDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is DataGrid dataGrid) || dataGrid.SelectedItem == null) return;
            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }
    }
}
