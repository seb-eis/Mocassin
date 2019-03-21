using System;
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
        ///     Copies the item source of the column to the passed <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="dependencyObject"></param>
        private void CopyItemsSource(DependencyObject dependencyObject)
        {
            var binding = BindingOperations.GetBinding(this, ItemsControl.ItemsSourceProperty) 
                          ?? throw new InvalidOperationException("Binding is null");

            BindingOperations.SetBinding(dependencyObject, ItemsControl.ItemsSourceProperty, binding);
        }
    }
}