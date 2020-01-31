using System;
using System.Windows;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Class to provide <see cref="ViewModelBase" /> of <see cref="UserControl" /> to <see cref="TabControl" /> systems
    /// </summary>
    public class ControlTabItem : ViewModelBase, IDisposable
    {
        private Visibility visibility = Visibility.Visible;

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> of the tab
        /// </summary>
        public ViewModelBase ContentViewModel { get; }

        /// <summary>
        ///     Get the <see cref="Control" /> of the tab
        /// </summary>
        public Control Content { get; }

        /// <summary>
        ///     Get the name for the tab
        /// </summary>
        public string TabName { get; }

        /// <summary>
        ///     Boolean flag that indicates if the tab can be moved manually
        /// </summary>
        public virtual bool IsUserMovable => false;

        /// <summary>
        ///     Get or set the <see cref="System.Windows.Visibility"/> of the tab
        /// </summary>
        public Visibility Visibility
        {
            get => visibility;
            set => SetProperty(ref visibility, value);
        }

        /// <summary>
        ///     Creates a new <see cref="ControlTabItem" /> from tab name, view model and user control
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="contentViewModel"></param>
        /// <param name="content"></param>
        public ControlTabItem(string tabName, ViewModelBase contentViewModel, Control content)
        {
            TabName = tabName ?? throw new ArgumentNullException(nameof(tabName));
            ContentViewModel = contentViewModel ?? throw new ArgumentNullException(nameof(contentViewModel));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Content.DataContext = ContentViewModel;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            (ContentViewModel as IDisposable)?.Dispose();
            (Content as IDisposable)?.Dispose();
        }
    }
}