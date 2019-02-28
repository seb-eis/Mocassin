using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Mocassin.UI.Base.ViewModel;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.MainControls.ProjectConsole.SubControls.MessageConsole;

namespace Mocassin.UI.GUI.MainControls.ProjectConsole
{
    /// <summary>
    ///     TThe <see cref="ViewModel" /> for the main project console
    /// </summary>
    public class ProjectConsoleViewModel : UserControlTabControlViewModel
    {
        public ProjectConsoleViewModel()
        {
            var test = new MessageConsoleViewModel();  
            AppendCollectionItem(new UserControlTabItem("Test", test, new MessageConsoleView()));
            test.AppendCollectionItem("Hallo\n\n");
            test.AppendCollectionItem("Hallo2\n");
        }
    }
}