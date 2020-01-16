using System;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Base.ViewModels.Content
{
    /// <summary>
    ///     Represents a <see cref="ViewModelBase"/> implementation for custom content windows
    /// </summary>
    public class ContentWindowViewModel : ViewModelBase, IDisposable
    {
        private string windowDescription;

        /// <summary>
        ///     Get or set an <see cref="Action"/> that performs additional actions when the object is disposed
        /// </summary>
        public Action OnDisposeAction { get; set; } 

        /// <summary>
        ///     Get the <see cref="ContentControl"/> that supplies the content
        /// </summary>
        public ContentControl Content { get; }

        /// <summary>
        ///     Get the <see cref="ViewModelBase"/> that controls the content
        /// </summary>
        public ViewModelBase ViewModel { get; }

        /// <summary>
        ///     Get or set the window description
        /// </summary>
        public string WindowDescription
        {
            get => windowDescription;
            set => SetProperty(ref windowDescription, value);
        }

        /// <inheritdoc />
        public ContentWindowViewModel(ContentControl content, ViewModelBase viewModel)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            Content.DataContext = ViewModel;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            OnDisposeAction?.Invoke();
            (Content as IDisposable)?.Dispose();
            (ViewModel as IDisposable)?.Dispose();
        }
    }
}