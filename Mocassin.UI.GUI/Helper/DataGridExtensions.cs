using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Mocassin.UI.GUI.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataGridExtensions
    {
        /// <summary>
        ///     Get the <see cref="DataGridRow"/> set of a <see cref="DataGrid"/>
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        public static IEnumerable<DataGridRow> GetRows(this DataGrid dataGrid)
        {
            if (dataGrid.ItemsSource == null) yield break;
            foreach (var item in dataGrid.ItemsSource)
            {
                if (dataGrid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row) yield return row;
            }
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}