using System.Windows.Controls;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl
{
    /// <summary>
    ///     Interaktionslogik für MetropolisTransitionGridControlView.xaml
    /// </summary>
    public partial class MetropolisTransitionGridControlView : UserControl
    {
        public MetropolisTransitionGridControlView()
        {
            InitializeComponent();
        }

        private void TransitionDataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            if (sender is DataGrid dataGrid) dataGrid.SelectedItem = null;
        }
    }
}