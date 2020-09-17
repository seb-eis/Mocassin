using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     Interaktionslogik für AbstractTransitionGridControlView.xaml
    /// </summary>
    public partial class AbstractTransitionGridControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}" /> for the row header
        /// </summary>
        private DragHandler<DataGrid> RowHeaderDragHandler { get; set; }

        /// <inheritdoc />
        public AbstractTransitionGridControlView()
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
            RowHeaderDragHandler.RegisterDragStartPoint(AbstractTransitionDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RowHeaderDragHandler.DeleteDragStartPoint(AbstractTransitionDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            RowHeaderDragHandler.TryDoDragDrop(AbstractTransitionDataGrid, e);
        }
    }
}