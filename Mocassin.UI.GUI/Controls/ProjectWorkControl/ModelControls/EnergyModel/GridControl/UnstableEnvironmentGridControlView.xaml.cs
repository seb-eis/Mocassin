using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl
{
    /// <summary>
    ///     Interaktionslogik für UnstableEnvironmentGridControlView.xaml
    /// </summary>
    public partial class UnstableEnvironmentGridControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}" /> for the row header
        /// </summary>
        private DragHandler<DataGrid> RowHeaderDragHandler { get; set; }

        /// <inheritdoc />
        public UnstableEnvironmentGridControlView()
        {
            InitializeDragHandlers();
            InitializeComponent();
        }

        private void InitializeDragHandlers()
        {
            RowHeaderDragHandler = new DragHandler<DataGrid>(x => new DataObject(x.SelectedItem ?? new object()));
        }

        private void RowHeaderLogo_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RowHeaderDragHandler.RegisterDragStartPoint(UnstableEnvironmentDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RowHeaderDragHandler.DeleteDragStartPoint(UnstableEnvironmentDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            RowHeaderDragHandler.TryDoDragDrop(UnstableEnvironmentDataGrid, e);
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