using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    ///     Interaktionslogik für CellPositionControlView.xaml
    /// </summary>
    public partial class CellPositionControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}" /> for the row header
        /// </summary>
        private DragHandler<DataGrid> RowHeaderDragHandler { get; set; }

        public CellPositionControlView()
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
            RowHeaderDragHandler.RegisterDragStartPoint(PositionDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RowHeaderDragHandler.DeleteDragStartPoint(PositionDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            RowHeaderDragHandler.TryDoDragDrop(PositionDataGrid, e);
        }
    }
}