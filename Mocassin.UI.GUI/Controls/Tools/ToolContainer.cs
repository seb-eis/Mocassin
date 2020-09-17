using System;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.Objects;

namespace Mocassin.UI.GUI.Controls.Tools
{
    /// <summary>
    ///     Represents a container for a tool that is provided within the UI
    /// </summary>
    public class ToolContainer : LazyVvmContainer, IDisposable
    {
        /// <summary>
        ///     Get the description string for the tool
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Get the <see cref="ToolViewModel" />
        /// </summary>
        public ToolViewModel ViewModel => (ToolViewModel) Value.ViewModel;

        /// <inheritdoc />
        public ToolContainer(Func<UserControl> viewFactory, Func<ToolViewModel> viewModelFactory, string name, string description)
            : base(viewFactory, viewModelFactory, name)
        {
            Description = description;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!lazyContainer.IsValueCreated) return;
            (lazyContainer.Value.ViewModel as IDisposable)?.Dispose();
            (lazyContainer.Value.View as IDisposable)?.Dispose();
        }
    }
}