using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Extensions;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Interaktionslogik für ControlTabHostView.xaml
    /// </summary>
    public partial class ControlTabHostView : UserControl
    {
        /// <summary>
        ///     The <see cref="DragHandler{TElement}" /> for <see cref="Grid" /> instances of tab items
        /// </summary>
        private DragHandler<Grid> TabItemDragHandler { get; }

        /// <inheritdoc />
        public ControlTabHostView()
        {
            TabItemDragHandler = new DragHandler<Grid>(arg => new DataObject(arg?.DataContext ?? new object()));
            InitializeComponent();
        }

        private void HeaderItemGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItemDragHandler.RegisterDragStartPoint(sender as Grid, e);
        }

        private void HeaderItemGrid_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabItemDragHandler.DeleteDragStartPoint(sender as Grid, e);
        }

        private void HeaderItemGrid_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            TabItemDragHandler.TryDoDragDrop(sender as Grid, e);
        }

        private void HeaderItemGrid_OnDrop(object sender, DragEventArgs e)
        {
            this.RelayDropToContext(PrimaryTabControl, e);
        }

        private void HeaderItemGrid_OnDragOver(object sender, DragEventArgs e)
        {
            if (!(sender is Grid headerItemGrid && e.Data.GetData(typeof(DynamicControlTabItem)) is DynamicControlTabItem droppedItem))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var insertIndex = (headerItemGrid.DataContext as DynamicControlTabItem)?.GetIndexInHost() ?? -1;
            e.Data.SetData(new TabMoveData(droppedItem, insertIndex));
            this.RelayDragOverToContext(e, DragDropEffects.Move);
        }
    }
}