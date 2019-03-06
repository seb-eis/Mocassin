using System.Windows.Controls;
using Mocassin.Framework.Operations;
using Mocassin.UI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.MessageConsole;

namespace Mocassin.UI.GUI.Controls.ProjectConsole
{
    /// <summary>
    ///     TThe <see cref="ViewModel" /> for the main project console
    /// </summary>
    public class ProjectConsoleTabControlViewModel : UserControlTabControlViewModel
    {
        /// <summary>
        ///    Get the <see cref="SubControls.MessageConsole.MessageConsoleViewModel" /> that controls the display of string messages
        /// </summary>
        private MessageConsoleViewModel MessageConsoleViewModel { get; }

        /// <summary>
        /// Get the <see cref="SubControls.MessageConsole.MessageConsoleViewModel" /> that controls the display of string reports
        /// </summary>
        private MessageConsoleViewModel ReportConsoleViewModel { get; }

        /// <summary>
        ///     Create new <see cref="ProjectConsoleTabControlViewModel" />
        /// </summary>
        public ProjectConsoleTabControlViewModel()
        {
            MessageConsoleViewModel = new MessageConsoleViewModel();
            ReportConsoleViewModel = new MessageConsoleViewModel();
            InitializeDefaultTabs();
        }

        /// <summary>
        ///     Display a message <see cref="string" /> in the console
        /// </summary>
        /// <param name="str"></param>
        public void DisplayMessage(string str)
        {
            if (str is null) return;
            MessageConsoleViewModel.AddCollectionItem(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationReport"></param>
        public void Display(IOperationReport operationReport)
        {
            if (operationReport is null) return;
            ReportConsoleViewModel.AddCollectionItem(operationReport.ToString());
        }

        /// <inheritdoc />
        protected sealed override void InitializeDefaultTabs()
        {
            base.InitializeDefaultTabs();
            AddNonClosableTab("Messages", MessageConsoleViewModel, new MessageConsoleView());
            AddNonClosableTab("Operations", ReportConsoleViewModel, new MessageConsoleView());
        }
    }
}