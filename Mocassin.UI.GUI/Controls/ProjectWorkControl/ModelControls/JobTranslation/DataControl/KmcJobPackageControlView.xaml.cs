using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.DataControl
{
    /// <summary>
    ///     Interaktionslogik für KmcJobPackageControlView.xaml
    /// </summary>
    public partial class KmcJobPackageControlView : UserControl
    {
        /// <summary>
        ///     Get or set the <see cref="DragHandler{TElement}"/> for the row header
        /// </summary>
        private DragHandler<DataGrid> RowHeaderDragHandler { get; set; }

        public KmcJobPackageControlView()
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
            RowHeaderDragHandler.RegisterDragStartPoint(JobCollectionsDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RowHeaderDragHandler.DeleteDragStartPoint(JobCollectionsDataGrid, e);
        }

        private void RowHeaderLogo_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            RowHeaderDragHandler.TryDoDragDrop(JobCollectionsDataGrid, e);
        }
    }
}