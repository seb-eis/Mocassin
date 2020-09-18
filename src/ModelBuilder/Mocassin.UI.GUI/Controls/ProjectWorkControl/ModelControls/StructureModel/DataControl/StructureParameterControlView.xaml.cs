using System.Windows.Controls;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    ///     Interaktionslogik für StructureParameterControlView.xaml
    /// </summary>
    public partial class StructureParameterControlView : UserControl
    {
        /// <inheritdoc />
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