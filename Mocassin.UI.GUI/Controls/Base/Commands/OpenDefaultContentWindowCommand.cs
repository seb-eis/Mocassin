using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.Objects;
using Mocassin.UI.GUI.Base.ViewModels.Content;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     An abstract <see cref="AsyncProjectControlCommand" /> to implement content window open commands
    /// </summary>
    public abstract class OpenDefaultContentWindowCommand : ProjectControlCommand
    {
        /// <summary>
        ///     Get the base window description string that
        /// </summary>
        protected virtual string BaseWindowDescription { get; }

        /// <inheritdoc />
        protected OpenDefaultContentWindowCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
            BaseWindowDescription = "Mocassin";
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
            viewModel.DisposeActions.Add(RelayWindowDescriptionChanges(viewModel));
            viewModel.DisposeActions.AddRange(GetAdditionalDisposeActions(container));
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
        ///     Attaches to the <see cref="IProjectAppControl" /> property change events and relays the window description
        ///     changes to the passed view model. Returns an <see cref="Action" /> to unsubscribe from the event
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        protected virtual Action RelayWindowDescriptionChanges(ContentWindowViewModel viewModel)
        {
            void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(ProjectControl.WindowDescription)) viewModel.WindowDescription = BuildWindowDescription();
            }

            ProjectControl.PropertyChanged += OnPropertyChanged;
            return () => ProjectControl.PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        ///     Implementation dependent supply of the new <see cref="VvmContainer" /> that supplies the window content and view
        ///     model
        /// </summary>
        /// <returns></returns>
        protected abstract VvmContainer CreateVvmContainer();

        /// <summary>
        ///     Implementation dependent behavior that provides a set of <see cref="Action" /> delegates that perform cleanup on
        ///     <see cref="VvmContainer" /> components not covered by the basic dispose interfaces
        /// </summary>
        /// <param name="vvmContainer"></param>
        /// <returns></returns>
        protected abstract IEnumerable<Action> GetAdditionalDisposeActions(VvmContainer vvmContainer);
    }
}