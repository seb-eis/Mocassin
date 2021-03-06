﻿using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole;

namespace Mocassin.UI.GUI.Controls.ProjectConsole
{
    /// <summary>
    ///     TThe <see cref="ViewModelBase" /> for the main project console
    /// </summary>
    public class ProjectConsoleTabControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ControlTabHostViewModel" /> that controls the additional tabs of the console control
        /// </summary>
        public ControlTabHostViewModel TabHostViewModel { get; }

        /// <summary>
        ///     Get the <see cref="SubControls.MessageConsole.MessageConsoleViewModel" /> that controls the display of string
        ///     messages
        /// </summary>
        public MessageConsoleViewModel MessageConsoleViewModel { get; }

        /// <inheritdoc />
        public ProjectConsoleTabControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            TabHostViewModel = new ControlTabHostViewModel {TabStripPlacement = Dock.Top};
            MessageConsoleViewModel = new MessageConsoleViewModel(projectControl);
            TabHostViewModel.AddStaticTab("Notifications", MessageConsoleViewModel, new MessageConsoleView());
            TabHostViewModel.SetActiveTabByIndex(0);
        }
    }
}