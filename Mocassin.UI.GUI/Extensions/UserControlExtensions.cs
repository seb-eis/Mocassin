using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.GUI.Base.DataContext;

namespace Mocassin.UI.GUI.Extensions
{
    /// <summary>
    ///     Provides Extension methods for <see cref="UserControl"/> event handling
    /// </summary>
    public static class UserControlExtensions
    {
        /// <summary>
        ///     Extension to <see cref="IObjectDropAcceptor"/> drop event relay to data context. The <see cref="ItemsControl"/> will be refreshed if not null
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="itemsControl"></param>
        /// <param name="e"></param>
        public static void RelayDropToContext(this UserControl userControl, ItemsControl itemsControl, DragEventArgs e)
        {
            if (!(userControl.DataContext is IObjectDropAcceptor acceptor) || !acceptor.HandleDropAddCommand.CanExecute(e.Data)) return;
            acceptor.HandleDropAddCommand.Execute(e.Data);
            itemsControl?.Items.Refresh();
            e.Handled = true;
        }

        /// <summary>
        ///     Extension to <see cref="IObjectDropAcceptor"/> drag over relay to data context
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="e"></param>
        /// <param name="effects"></param>
        public static void RelayDragOverToContext(this UserControl userControl, DragEventArgs e, DragDropEffects effects)
        {
            if (userControl.DataContext is IObjectDropAcceptor acceptor)
                e.Effects = acceptor.HandleDropAddCommand.CanExecute(e.Data) ? effects : DragDropEffects.None;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}