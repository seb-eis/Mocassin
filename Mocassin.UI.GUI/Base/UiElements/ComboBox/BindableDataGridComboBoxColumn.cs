using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mocassin.UI.GUI.Base.UiElements.ComboBox
{
    /// <summary>
    ///     A <see cref="DataGridComboBoxColumn" /> implementation that allows non-static item sources as bindings
    /// </summary>
    public class BindableDataGridComboBoxColumn : DataGridComboBoxColumn
    {
        /// <inheritdoc />
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateEditingElement(cell, dataItem);
            CopyItemsSource(element);
            return element;
        }

        /// <inheritdoc />
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var element = base.GenerateElement(cell, dataItem);
            CopyItemsSource(element);
            return element;
        }

        /// <summary>
        ///     Copies the item source of the column to the passed <see cref="DataGridCell" />
        /// </summary>
        /// <param name="element"></param>
        private void CopyItemsSource(FrameworkElement element)
        {
            var binding = BindingOperations.GetBinding(this, ItemsControl.ItemsSourceProperty);
            if (binding == null) return;

            BindingOperations.SetBinding(element, ItemsControl.ItemsSourceProperty, binding);
        }
    }
}