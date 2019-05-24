using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     Interaktionslogik für ExchangePairGridControlView.xaml
    /// </summary>
    public partial class ExchangePairGridControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}"/> for the exchange pair <see cref="DataGrid"/>
        /// </summary>
        private DragHandler<DataGrid> ExchangePairDataGridDragHandler { get; set; }

        public ExchangePairGridControlView()
        {
            InitializeDragDropHandlers();
            InitializeComponent();
        }

        private void InitializeDragDropHandlers()
        {
            ExchangePairDataGridDragHandler = new DragHandler<DataGrid>(x => new DataObject(x.SelectedItem ?? new object()));
        }

        private void ExchangePairDataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            ExchangePairDataGrid.SelectedItem = null;
        }

        private void ExchangePairDataGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExchangePairDataGridDragHandler.RegisterDragStartPoint(sender as DataGrid, e);
        }

        private void ExchangePairDataGrid_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ExchangePairDataGridDragHandler.DeleteDragStartPoint(sender as DataGrid, e);
        }

        private void ExchangePairDataGrid_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            ExchangePairDataGridDragHandler.TryDoDragDrop(sender as DataGrid, e);
        }
    }
}