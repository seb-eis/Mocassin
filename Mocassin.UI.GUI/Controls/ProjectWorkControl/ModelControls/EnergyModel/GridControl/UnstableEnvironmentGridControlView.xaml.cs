using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl
{
    /// <summary>
    ///     Interaktionslogik für UnstableEnvironmentGridControlView.xaml
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

            // Workaround for AddItem/EditingItem DeferRefresh exception by enforcing cancel of currently active row edit
            dataGrid.CancelEdit(DataGridEditingUnit.Row);

            context.SynchronizeEnvironmentCollectionCommand.Execute(null);
            dataGrid.Items.Refresh();
            e.Handled = true;
        }

        private void RefreshMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(UnstableEnvironmentDataGrid.DataContext is UnstableEnvironmentGridControlViewModel context)) return;

            // Workaround for AddItem/EditingItem DeferRefresh exception by enforcing cancel of currently active row edit
            UnstableEnvironmentDataGrid.CancelEdit(DataGridEditingUnit.Row);

            context.SynchronizeEnvironmentCollectionCommand.Execute(null);
            UnstableEnvironmentDataGrid.Items.Refresh();
            e.Handled = true;
        }
    }
}