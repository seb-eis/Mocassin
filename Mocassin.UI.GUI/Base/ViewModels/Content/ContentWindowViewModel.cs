using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Mocassin.Framework.Extensions;

namespace Mocassin.UI.GUI.Base.ViewModels.Content
{
    /// <summary>
    ///     Represents a <see cref="ViewModelBase" /> implementation for custom content windows
    /// </summary>
    public class ContentWindowViewModel : ViewModelBase, IDisposable
    {
        private string windowDescription;

        /// <summary>
        ///     Get the <see cref="HashSet{T}" /> of <see cref="Action" /> delegates to be called when disposing the view model
        /// </summary>
        public HashSet<Action> DisposeActions { get; }

        /// <summary>
        ///     Get the <see cref="ContentControl" /> that supplies the content
        /// </summary>
        public ContentControl Content { get; }

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> that controls the content
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
            DisposeActions = new HashSet<Action>();
            Content.DataContext = ViewModel;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            DisposeActions.Action(x => x?.Invoke()).Load();
            (Content as IDisposable)?.Dispose();
            (ViewModel as IDisposable)?.Dispose();
        }
    }
}