using System;
using System.ComponentModel;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.Objects;
using Mocassin.UI.GUI.Base.ViewModels.Content;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     An abstract <see cref="AsyncProjectControlCommand"/> to implement content window open commands
    /// </summary>
    public abstract class OpenDefaultContentWindowCommand : ProjectControlCommand
    {
        /// <summary>
        ///     Get the base window description string that
        /// </summary>
        protected abstract string BaseWindowDescription { get; }

        /// <inheritdoc />
        protected OpenDefaultContentWindowCommand(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public override void Execute()
        {
            OpenBackgroundWindow();
        }

        /// <summary>
        ///     Creates an opens the new window
        /// </summary>
        protected virtual void OpenBackgroundWindow()
        {
            var container = CreateVvmContainer();
            var viewModel = new ContentWindowViewModel(container.View, container.ViewModel) {WindowDescription = BuildWindowDescription()};
            viewModel.OnDisposeAction = RelayWindowDescriptionChanges(viewModel);
            var window = new ContentWindow {DataContext = viewModel};
            window.Show();
        }

        /// <summary>
        ///     Builds the window description
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildWindowDescription()
        {
            return BaseWindowDescription + " " + ProjectControl.WindowDescription;
        }

        /// <summary>
        ///     Attaches to the <see cref="IMocassinProjectControl" /> property change events and relays the window description
        ///     changes to the passed view model. Returns an <see cref="Action" /> to unsubscribe from the event
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual Action RelayWindowDescriptionChanges(ContentWindowViewModel viewModel)
        {
            void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(ProjectControl.WindowDescription))
                {
                    viewModel.WindowDescription = BuildWindowDescription();
                }
            }

            ProjectControl.PropertyChanged += OnPropertyChanged;
            return () => ProjectControl.PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        ///     Implementation dependent supply of the new <see cref="VvmContainer"/> that supplies the window content and view model
        /// </summary>
        /// <returns></returns>
        protected abstract VvmContainer CreateVvmContainer();
    }
}