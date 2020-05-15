using System;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Base.Objects
{
    /// <summary>
    ///     Container for passing around named <see cref="UserControl" /> and <see cref="ViewModelBase" /> to content hosters
    /// </summary>
    public class VvmContainer : ViewModelBase
    {
        /// <summary>
        ///     Get or set a name for the setup
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Get the <see cref="UserControl" /> that supplies the view
        /// </summary>
        public UserControl View { get; }

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> that serves as the data context
        /// </summary>
        public ViewModelBase ViewModel { get; }

        /// <summary>
        ///     Creates a new <see cref="VvmContainer" /> from view, view model and name
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="name"></param>
        public VvmContainer(UserControl view, ViewModelBase viewModel, string name)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            Name = name;
            View.DataContext = ViewModel;
        }

        /// <summary>
        ///     Creates a new <see cref="VvmContainer" /> with a view and name that uses an <see cref="EmptyViewModel" />
        /// </summary>
        /// <param name="view"></param>
        /// <param name="name"></param>
        public VvmContainer(UserControl view, string name)
            : this(view, new EmptyViewModel(), name)
        {
        }
    }
}